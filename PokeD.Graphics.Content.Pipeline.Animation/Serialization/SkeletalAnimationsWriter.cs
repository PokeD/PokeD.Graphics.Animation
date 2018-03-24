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

using System.Collections.Generic;

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using tainicom.Aether.Content.Pipeline.Animation;

namespace tainicom.Aether.Content.Pipeline.Serialization
{
    [ContentTypeWriter]
    public class SkeletalAnimationsWriter : ContentTypeWriter<SkeletalAnimationsContent>
    {
        protected override void Write(ContentWriter output, SkeletalAnimationsContent value)
        {
            WriteClips(output, value.Clips);
            WriteBindPose(output, value.BindPose);
            WriteInvBindPose(output, value.InvBindPose);
            WriteSkeletonHierarchy(output, value.SkeletonHierarchy);
            WriteBoneNames(output, value.BoneNames);
        }

        private static void WriteClips(ContentWriter output, Dictionary<string, SkeletalClipContent> clips)
        {
            var count = clips.Count;
            output.Write(count);

            foreach (var clip in clips)
            {
                output.Write(clip.Key);
                output.WriteObject(clip.Value);
            }            
        }

        private static void WriteBindPose(ContentWriter output, List<Microsoft.Xna.Framework.Matrix> bindPoses)
        {
            var count = bindPoses.Count;
            output.Write(count);

            for (var i = 0; i < count; i++)
                output.Write(bindPoses[i]);
        }

        private static void WriteInvBindPose(ContentWriter output, List<Microsoft.Xna.Framework.Matrix> invBindPoses)
        {
            var count = invBindPoses.Count;
            output.Write(count);

            for (var i = 0; i < count; i++)
                output.Write(invBindPoses[i]);
        }

        private static void WriteSkeletonHierarchy(ContentWriter output, List<int> skeletonHierarchy)
        {
            var count = skeletonHierarchy.Count;
            output.Write(count);

            for (var i = 0; i < count; i++)
                output.Write(skeletonHierarchy[i]);
        }
    
        private static void WriteBoneNames(ContentWriter output, List<string> boneNames)
        {
            var count = boneNames.Count;
            output.Write(count);
            
            for (var boneIndex = 0; boneIndex < count; boneIndex++)
            {
                var boneName = boneNames[boneIndex];
                output.Write(boneName);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform) => "tainicom.Aether.Animation.Content.SkeletalAnimationsReader, PokeD.Graphics.Animation";
    }
}