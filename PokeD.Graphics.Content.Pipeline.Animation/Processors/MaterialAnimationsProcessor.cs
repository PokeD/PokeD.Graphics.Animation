using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

using PokeD.Graphics.Content.Pipeline.Extensions;
using PokeD.Graphics.Content.Pipeline.MaterialAnimation;

namespace PokeD.Graphics.Content.Pipeline.Processors
{
    [ContentProcessor(DisplayName = "Material Animation - PokeD.Graphics")]
    public class MaterialAnimationsProcessor : ContentProcessor<NodeContent, MaterialAnimationsContent>
    {
        [DefaultValue(0)]
        [DisplayName("Generate Keyframes Frequency")]
        [Description("0=no, 30=30fps, 60=60fps")]
        public virtual int GenerateKeyframesFrequency { get; set; } = 0;

        public override MaterialAnimationsContent Process(NodeContent input, ContentProcessorContext context)
        {
            // Gather all the nodes in tree traversal order.
            var nodes = input.AsEnumerable().SelectDeep(n => n.Children).ToList();

            var materialAnimations = nodes.FindAll(n => n is MaterialAnimationContent).Cast<MaterialAnimationContent>().ToList();
            

            var clips = ProcessAnimations(input, context, materialAnimations, GenerateKeyframesFrequency);

            return new MaterialAnimationsContent(clips);
        }

        private static Dictionary<string, MaterialClipContent> ProcessAnimations(NodeContent input, ContentProcessorContext context, List<MaterialAnimationContent> animations, int generateKeyframesFrequency)
        {
            // Convert each animation in turn.
            var animationClips = new Dictionary<string, MaterialClipContent>();

            foreach (var animation in animations)
            {
                var clip = ProcessAnimation(input, context, animation, generateKeyframesFrequency);

                animationClips.Add(animation.Name, clip);
            }

            if (animationClips.Count == 0)
            {
                //throw new InvalidContentException("Input file does not contain any animations.");
                context.Logger.LogWarning(null, null, "Input file does not contain any material animations.");
            }

            return animationClips;
        }

        private static MaterialClipContent ProcessAnimation(NodeContent input, ContentProcessorContext context, MaterialAnimationContent animation, int generateKeyframesFrequency)
        {
            var keyframes = new List<MaterialKeyframeContent>();

            // For each input animation channel.
            foreach (var channel in animation.Channels)
            {
                foreach (var keyframe in channel.Value)
                    keyframes.Add(new MaterialKeyframeContent(keyframe.Material, keyframe));
            }

            // Sort the merged keyframes by time.
            //keyframes.Sort(CompareKeyframeTimes);

            //System.Diagnostics.Debugger.Launch();
            //if (generateKeyframesFrequency > 0)
            //    keyframes = InterpolateKeyframes(animation.Duration, keyframes, generateKeyframesFrequency);

            if (keyframes.Count == 0)
                throw new InvalidContentException("Animation has no keyframes.");

            if (animation.Duration <= TimeSpan.Zero)
                throw new InvalidContentException("Animation has a zero duration.");

            return new MaterialClipContent(animation.Duration, keyframes.ToArray())
            {
                Name = animation.Name,
                Identity = animation.Identity
            };
        }

        /*
        private static List<MaterialAnimationKeyframe> InterpolateKeyframes(TimeSpan duration, List<MaterialAnimationKeyframe> keyframes, int generateKeyframesFrequency)
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
            var boneFrames = new List<MaterialAnimationKeyframe>[maxBone + 1];
            for (var i = 0; i < keyframeCount; i++)
            {
                var material = keyframes[i].Material;
                if (boneFrames[bone] == null) boneFrames[bone] = new List<MaterialAnimationKeyframe>();
                boneFrames[bone].Add(keyframes[i]);
            }

            //            
            System.Diagnostics.Debug.WriteLine("Duration: " + duration);
            System.Diagnostics.Debug.WriteLine("keyframeCount: " + keyframeCount);

            for (var b = 0; b < boneFrames.Length; b++)
            {
                var keySpan = TimeSpan.FromTicks((long)((1f / (float)generateKeyframesFrequency) * TimeSpan.TicksPerSecond));
                boneFrames[b] = InterpolateFramesBone(b, boneFrames[b], keySpan);
            }

            var frames = keyframeCount / boneCount;

            var checkDuration = TimeSpan.FromSeconds((float)(frames - 1) / (float)generateKeyframesFrequency);
            if (duration == checkDuration) return keyframes;

            // -- TODO: Weird fix investigate never
            for (var i = 0; i < boneFrames.Length; i++)
                if (boneFrames[i] == null)
                    boneFrames[i] = new List<MaterialAnimationKeyframe>();
            // --

            var newKeyframes = new List<MaterialAnimationKeyframe>();
            for (var b = 0; b < boneFrames.Length; b++)
                for (var k = 0; k < boneFrames[b].Count; ++k)
                    newKeyframes.Add(boneFrames[b][k]);
            newKeyframes.Sort(CompareKeyframeTimes);

            return newKeyframes;
        }

        private static List<MaterialKeyframeContent> InterpolateFramesBone(int bone, List<MaterialKeyframeContent> frames, TimeSpan keySpan)
        {
            System.Diagnostics.Debug.WriteLine("");
            System.Diagnostics.Debug.WriteLine("Bone: " + bone);
            if (frames == null)
            {
                System.Diagnostics.Debug.WriteLine("Frames: " + "null");
                return frames;
            }
            System.Diagnostics.Debug.WriteLine("Frames: " + frames.Count);
            System.Diagnostics.Debug.WriteLine("MinTime: " + frames[0].Time);
            System.Diagnostics.Debug.WriteLine("MaxTime: " + frames[frames.Count - 1].Time);

            for (var i = 0; i < frames.Count - 1; ++i)
                InterpolateFrames(bone, frames, keySpan, i);

            return frames;
        }

        private static void InterpolateFrames(int bone, List<MaterialKeyframeContent> frames, TimeSpan keySpan, int i)
        {
            var a = i;
            var b = i + 1;
            var diff = frames[b].Time - frames[a].Time;
            if (diff > keySpan)
            {
                var newTime = frames[a].Time + keySpan;
                var amount = (float)(keySpan.TotalSeconds / diff.TotalSeconds);

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

                frames.Insert(b, new MaterialKeyframeContent(bone, newTime, newMatrix));
            }
        }
        */
    }
}