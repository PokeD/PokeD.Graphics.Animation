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

using Microsoft.Xna.Framework.Content;

namespace tainicom.Aether.Animation.Content
{
    public class SkeletalClipReader : ContentTypeReader<SkeletalClip>
    {
        protected override SkeletalClip Read(ContentReader input, SkeletalClip existingInstance)
        {
            var animationClip = existingInstance;

            if (existingInstance == null)
            {
                var duration = ReadDuration(input);
                var keyframes = ReadKeyframes(input, null);
                animationClip = new SkeletalClip(duration, keyframes);
            }
            else
            {
                animationClip.Duration = ReadDuration(input);
                ReadKeyframes(input, animationClip.Keyframes);
            }

            return animationClip;                       
        }
        
        private static TimeSpan ReadDuration(ContentReader input) => new TimeSpan(input.ReadInt64());

        private static SkeletalKeyframe[] ReadKeyframes(ContentReader input, SkeletalKeyframe[] existingInstance)
        {
            var keyframes = existingInstance;

            var count = input.ReadInt32();
            if (keyframes == null)
                keyframes = new SkeletalKeyframe[count];
            
            for (var i = 0; i < count; i++)
            {
                keyframes[i].Bone = input.ReadInt32();
                keyframes[i].Time = new TimeSpan(input.ReadInt64());
                keyframes[i].Transform = input.ReadMatrix();
                //keyframes[i].Transform.M14 = 0;
                //keyframes[i].Transform.M24 = 0;
                //keyframes[i].Transform.M34 = 0;
                //keyframes[i].Transform.M44 = 1;
            }

            return keyframes;
        }      
    }
}