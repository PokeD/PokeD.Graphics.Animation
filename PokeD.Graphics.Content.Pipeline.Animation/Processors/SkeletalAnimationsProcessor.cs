#region License
//   Copyright 2011-2016 Kastellanos Nikolaos
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;

using tainicom.Aether.Content.Pipeline.Animation;

namespace tainicom.Aether.Content.Pipeline.Processors
{
    [ContentProcessor(DisplayName = "Skeletal Animation - Aether")]
    public class SkeletalAnimationsProcessor : ContentProcessor<NodeContent, SkeletalAnimationsContent>
    {
        [DefaultValue(SkinnedEffect.MaxBones)]
        [DisplayName("Max Bones")]
        [Description("Set the maximum supported bones. Note that the default Skinning effect supports only 72 bones.")]
        public virtual int MaxBones { get; set; } = SkinnedEffect.MaxBones;

        [DefaultValue(0)]
        [DisplayName("Generate Keyframes Frequency")]
        [Description("0=no, 30=30fps, 60=60fps")]
        public virtual int GenerateKeyframesFrequency { get; set; } = 0;

        [DefaultValue(false)]
        [DisplayName("Fix BoneRoot from MG importer")]
        [Description("MonoGame converts some NodeContent into BoneContent. If true, revert that to get the original Skeleton and add the real boneroot to the root node.")]
        public virtual bool FixRealBoneRoot { get; set; } = false;

        public override SkeletalAnimationsContent Process(NodeContent input, ContentProcessorContext context)
        {
            context.Logger.LogMessage("Processing Skeletal Animation");
            try
            {
                if (FixRealBoneRoot)
                    MGFixRealBoneRoot(input, context);

                ValidateMesh(input, context, null);

                // Find the skeleton.
                var skeleton = MeshHelper.FindSkeleton(input);

                if (skeleton == null)
                    throw new InvalidContentException("Input skeleton not found.");

                // We don't want to have to worry about different parts of the model being
                // in different local coordinate systems, so let's just bake everything.
                FlattenTransforms(input, skeleton);

                // Read the bind pose and skeleton hierarchy data.
                var bones = MeshHelper.FlattenSkeleton(skeleton);

                if (bones.Count > MaxBones)
                    throw new InvalidContentException($"Skeleton has {bones.Count} bones, but the maximum supported is {MaxBones}.");

                var bindPose = new List<Matrix>();
                var invBindPose = new List<Matrix>();
                var skeletonHierarchy = new List<int>();
                var boneNames = new List<string>();

                foreach (var bone in bones)
                {
                    bindPose.Add(bone.Transform);
                    invBindPose.Add(Matrix.Invert(bone.AbsoluteTransform));
                    skeletonHierarchy.Add(bones.IndexOf(bone.Parent as BoneContent));
                    boneNames.Add(bone.Name);
                }

                // Convert animation data to our runtime format.
                var clips = ProcessAnimations(input, context, skeleton.Animations, bones, GenerateKeyframesFrequency);

                return new SkeletalAnimationsContent(bindPose, invBindPose, skeletonHierarchy, boneNames, clips);
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }
        
        /// <summary>
        /// MonoGame converts some NodeContent into BoneContent.
        /// Here we revert that to get the original Skeleton and  
        /// add the real boneroot to the root node.
        /// </summary>
        private static void MGFixRealBoneRoot(NodeContent input, ContentProcessorContext context)
        {
            for (var i = input.Children.Count - 1; i >= 0; i--)
            {
                var node = input.Children[i];
                if (node is BoneContent &&
                    node.AbsoluteTransform == Matrix.Identity &&
                    node.Children.Count ==1 &&
                    node.Children[0] is BoneContent &&
                    node.Children[0].AbsoluteTransform == Matrix.Identity
                    )
                {
                    //dettach real boneRoot
                    var realBoneRoot = node.Children[0];
                    node.Children.RemoveAt(0);
                    //copy animation from node to boneRoot
                    foreach (var animation in node.Animations)
                        realBoneRoot.Animations.Add(animation.Key, animation.Value);
                    // convert fake BoneContent back to NodeContent
                    input.Children[i] = new NodeContent()
                    {
                        Name = node.Name,
                        Identity = node.Identity,
                        Transform = node.Transform,                        
                    };
                    foreach (var animation in node.Animations)
                        input.Children[i].Animations.Add(animation.Key, animation.Value);
                    foreach (var opaqueData in node.OpaqueData)
                        input.Children[i].OpaqueData.Add(opaqueData.Key, opaqueData.Value);
                    //attach real boneRoot to the root node
                    input.Children.Add(realBoneRoot);

                    break;
                }
            }
        }

        /// <summary>
        /// Makes sure this mesh contains the kind of data we know how to animate.
        /// </summary>
        private static void ValidateMesh(NodeContent node, ContentProcessorContext context, string parentBoneName)
        {
            if (node is MeshContent mesh)
            {
                // Validate the mesh.
                if (parentBoneName != null)
                    context.Logger.LogWarning(null, null, $"Mesh {mesh.Name} is a child of bone {parentBoneName}. AnimatedModelProcessor does not correctly handle meshes that are children of bones.");

                if (!MeshHasSkinning(mesh))
                {
                    context.Logger.LogWarning(null, null, $"Mesh {mesh.Name} has no skinning information, so it has been deleted.");

                    mesh.Parent.Children.Remove(mesh);
                    return;
                }
            }
            else if (node is BoneContent)
                parentBoneName = node.Name; // If this is a bone, remember that we are now looking inside it.

            // Recurse (iterating over a copy of the child collection,
            // because validating children may delete some of them).
            foreach (var child in new List<NodeContent>(node.Children))
                ValidateMesh(child, context, parentBoneName);
        }

        /// <summary>
        /// Checks whether a mesh contains skininng information.
        /// </summary>
        private static bool MeshHasSkinning(MeshContent mesh) => 
            mesh.Geometry.All(geometry => geometry.Vertices.Channels.Contains(VertexChannelNames.Weights()) || geometry.Vertices.Channels.Contains("BlendWeight0"));

        /// <summary>
        /// Bakes unwanted transforms into the model geometry,
        /// so everything ends up in the same coordinate system.
        /// </summary>
        private static void FlattenTransforms(NodeContent node, BoneContent skeleton)
        {
            foreach (var child in node.Children)
            {
                // Don't process the skeleton, because that is special.
                if (child == skeleton)
                    continue;

                // Bake the local transform into the actual geometry.
                MeshHelper.TransformScene(child, child.Transform);

                // Having baked it, we can now set the local
                // coordinate system back to identity.
                child.Transform = Matrix.Identity;

                // Recurse.
                FlattenTransforms(child, skeleton);
            }
        }
        
        /// <summary>
        /// Converts an intermediate format content pipeline AnimationContentDictionary
        /// object to our runtime AnimationClip format.
        /// </summary>
        private static Dictionary<string, SkeletalClipContent> ProcessAnimations(NodeContent input, ContentProcessorContext context, AnimationContentDictionary animations, IList<BoneContent> bones, int generateKeyframesFrequency)
        {
            // Build up a table mapping bone names to indices.
            var boneMap = new Dictionary<string, int>();

            for (var i = 0; i < bones.Count; i++)
            {
                var boneName = bones[i].Name;

                if (!string.IsNullOrEmpty(boneName))
                    boneMap.Add(boneName, i);
            }

            // Convert each animation in turn.
            var animationClips = new Dictionary<string, SkeletalClipContent>();

            foreach (var animation in animations)
            {
                var clip = ProcessAnimation(input, context, animation.Value, boneMap, generateKeyframesFrequency);

                animationClips.Add(animation.Key, clip);
            }

            if (animationClips.Count == 0)
                context.Logger.LogWarning(null, null, "Input file does not contain any animations.");

            return animationClips;
        }
        
        /// <summary>
        /// Converts an intermediate format content pipeline AnimationContent
        /// object to our runtime AnimationClip format.
        /// </summary>
        private static SkeletalClipContent ProcessAnimation(NodeContent input, ContentProcessorContext context, AnimationContent animation, Dictionary<string, int> boneMap, int generateKeyframesFrequency)
        {
            var keyframes = new List<SkeletalKeyframeContent>();

            // For each input animation channel.
            foreach (var channel in
                animation.Channels)
            {
                // Look up what bone this channel is controlling.

                if (!boneMap.TryGetValue(channel.Key, out var boneIndex))
                {
                    //throw new InvalidContentException(string.Format("Found animation for bone '{0}', which is not part of the skeleton.", channel.Key));
                    context.Logger.LogWarning(null, null, "Found animation for bone '{0}', which is not part of the skeleton.", channel.Key);

                    continue;
                }

                foreach (var keyframe in channel.Value)
                    keyframes.Add(new SkeletalKeyframeContent(boneIndex, keyframe.Time, keyframe.Transform));
            }

            // Sort the merged keyframes by time.
            keyframes.Sort(CompareKeyframeTimes);

            //System.Diagnostics.Debugger.Launch();
            if (generateKeyframesFrequency > 0)
                keyframes = InterpolateKeyframes(animation.Duration, keyframes, generateKeyframesFrequency);

            if (keyframes.Count == 0)
                throw new InvalidContentException("Animation has no keyframes.");

            if (animation.Duration <= TimeSpan.Zero)
                throw new InvalidContentException("Animation has a zero duration.");

            return new SkeletalClipContent(animation.Duration, keyframes.ToArray());
        }

        private static int CompareKeyframeTimes(SkeletalKeyframeContent a, SkeletalKeyframeContent b)
        {
            var cmpTime = a.Time.CompareTo(b.Time);
            if (cmpTime == 0)
                return a.Bone.CompareTo(b.Bone);
            return cmpTime;
        }

        private static List<SkeletalKeyframeContent> InterpolateKeyframes(TimeSpan duration, List<SkeletalKeyframeContent> keyframes, int generateKeyframesFrequency)
        {
            if (generateKeyframesFrequency <= 0)
                return keyframes;

            var keyframeCount = keyframes.Count;

            // find bones
            var bonesSet = new HashSet<int>();
            var maxBone = 0;
            for (var i = 0; i < keyframeCount; i++)
            {
                var bone = keyframes[i].Bone;
                maxBone = Math.Max(maxBone, bone);
                bonesSet.Add(bone);
            }
            var boneCount = bonesSet.Count;

            // split bones 
            var boneFrames = new List<SkeletalKeyframeContent>[maxBone + 1];
            for (var i = 0; i < keyframeCount; i++)
            {
                var bone = keyframes[i].Bone;
                if (boneFrames[bone] == null) boneFrames[bone] = new List<SkeletalKeyframeContent>();
                boneFrames[bone].Add(keyframes[i]);
            }

            //            
            System.Diagnostics.Debug.WriteLine($"Duration: {duration}");
            System.Diagnostics.Debug.WriteLine($"keyframeCount: {keyframeCount}" );

            for (var b = 0; b < boneFrames.Length; b++)
            {
                var keySpan = TimeSpan.FromTicks((long)((1f / (float) generateKeyframesFrequency) * TimeSpan.TicksPerSecond));
                boneFrames[b] = InterpolateFramesBone(b, boneFrames[b], keySpan);
            }

            var frames = keyframeCount / boneCount;

            var checkDuration = TimeSpan.FromSeconds((float) (frames - 1) / (float) generateKeyframesFrequency);
            if (duration == checkDuration) return keyframes;

            // -- TODO: Weird fix investigate never
            for (var i = 0; i < boneFrames.Length; i++)
                if (boneFrames[i] == null)
                    boneFrames[i] = new List<SkeletalKeyframeContent>();
            // --

            var newKeyframes = new List<SkeletalKeyframeContent>();
            for (var b = 0; b < boneFrames.Length; b++)
            for (var k = 0; k < boneFrames[b].Count; ++k)
                newKeyframes.Add(boneFrames[b][k]);
            newKeyframes.Sort(CompareKeyframeTimes);

            return newKeyframes;
        }

        private static List<SkeletalKeyframeContent> InterpolateFramesBone(int bone, List<SkeletalKeyframeContent> frames, TimeSpan keySpan)
        {
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine($"Bone: {bone}");
            if (frames == null)
            {
                System.Diagnostics.Debug.WriteLine("Frames: null");
                return frames;
            }
            System.Diagnostics.Debug.WriteLine($"Frames: {frames.Count}");
            System.Diagnostics.Debug.WriteLine($"MinTime: {frames[0].Time}");
            System.Diagnostics.Debug.WriteLine($"MaxTime: {frames[frames.Count - 1].Time}");

            for (var i = 0; i < frames.Count - 1; ++i)
                InterpolateFrames(bone, frames, keySpan, i);

            return frames;
        }

        private static void InterpolateFrames(int bone, List<SkeletalKeyframeContent> frames, TimeSpan keySpan, int i)
        {
            var a = i;
            var b = i + 1;
            var diff = frames[b].Time - frames[a].Time;
            if (diff > keySpan)
            {
                var newTime = frames[a].Time + keySpan;
                var amount = (float) (keySpan.TotalSeconds / diff.TotalSeconds);

                frames[a].Transform.Decompose(out var pScale, out var pRotation, out var pTranslation);

                frames[b].Transform.Decompose(out var iScale, out var iRotation, out var iTranslation);

                //lerp
                Vector3.Lerp(ref pScale, ref iScale, amount, out var scale);
                Quaternion.Lerp(ref pRotation, ref iRotation, amount, out var rotationQuaternion);
                Vector3.Lerp(ref pTranslation, ref iTranslation, amount, out var translation);

                Matrix.CreateFromQuaternion(ref rotationQuaternion, out var rotation);

                var newMatrix = new Matrix
                {
                    M11 = scale.X * rotation.M11,
                    M12 = scale.X * rotation.M12,
                    M13 = scale.X * rotation.M13,
                    M14 = 0,
                    M21 = scale.Y * rotation.M21,
                    M22 = scale.Y * rotation.M22,
                    M23 = scale.Y * rotation.M23,
                    M24 = 0,
                    M31 = scale.Z * rotation.M31,
                    M32 = scale.Z * rotation.M32,
                    M33 = scale.Z * rotation.M33,
                    M34 = 0,
                    M41 = translation.X,
                    M42 = translation.Y,
                    M43 = translation.Z,
                    M44 = 1
                };

                frames.Insert(b, new SkeletalKeyframeContent(bone, newTime, newMatrix));
            }
        }
    }
}