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

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using tainicom.Aether.Content.Pipeline.Animation;

namespace tainicom.Aether.Content.Pipeline.Serialization
{
    [ContentTypeWriter]
    public class SkeletalClipWriter : ContentTypeWriter<SkeletalClipContent>
    {
        protected override void Write(ContentWriter output, SkeletalClipContent value)
        {
            WriteDuration(output, value.Duration);
            WriteKeyframes(output, value.Keyframes);
        }

        private static void WriteDuration(ContentWriter output, TimeSpan duration)
        {
            output.Write(duration.Ticks);
        }

        private static void WriteKeyframes(ContentWriter output, IList<SkeletalKeyframeContent> keyframes)
        {
            var count = keyframes.Count;
            output.Write(count);

            for (var i = 0; i < count; i++)
            {
                var keyframe = keyframes[i];
                output.Write(keyframe.Bone);
                output.Write(keyframe.Time.Ticks);
                output.Write(keyframe.Transform);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform) => "tainicom.Aether.Animation.Content.SkeletalClipReader, PokeD.Graphics.Animation";
    }
}