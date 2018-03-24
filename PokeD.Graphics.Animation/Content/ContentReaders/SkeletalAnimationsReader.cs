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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace tainicom.Aether.Animation.Content
{
    public class SkeletalAnimationsReader : ContentTypeReader<SkeletalAnimations>
    {
        protected override SkeletalAnimations Read(ContentReader input, SkeletalAnimations existingInstance)
        {
            var animations = existingInstance;

            if (existingInstance == null)
            {
                var clips = ReadAnimationClips(input, null);
                var bindPose = ReadBindPose(input, null);
                var invBindPose = ReadInvBindPose(input, null);
                var skeletonHierarchy = ReadSkeletonHierarchy(input, null);
                var boneMap = ReadBoneMap(input, null);
                animations = new SkeletalAnimations(bindPose, invBindPose, skeletonHierarchy, boneMap, clips);
            }
            else
            {
                ReadAnimationClips(input, animations.Clips);
                ReadBindPose(input, animations.BindPose);
                ReadInvBindPose(input, animations.InvBindPose);
                ReadSkeletonHierarchy(input, animations.SkeletonHierarchy);
                ReadBoneMap(input, animations.BoneMap);
            }

            return animations;
        }

        private static Dictionary<string, SkeletalClip> ReadAnimationClips(ContentReader input, Dictionary<string, SkeletalClip> existingInstance)
        {
            var animationClips = existingInstance;

            var count = input.ReadInt32();
            if (animationClips == null)
                animationClips = new Dictionary<string, SkeletalClip>(count);

            for (var i = 0; i < count; i++)
            {
                var key = input.ReadString();
                var val = input.ReadObject<SkeletalClip>();
                if (existingInstance == null)
                    animationClips.Add(key, val);
                else
                    animationClips[key] = val;
            }

            return animationClips;
        }

        private static Matrix[] ReadBindPose(ContentReader input, Matrix[] existingInstance)
        {
            var bindPose = existingInstance;

            var count = input.ReadInt32();
            if (bindPose == null)
                bindPose = new Matrix[count];

            for (var i = 0; i < count; i++)
            {
                var val = input.ReadMatrix();
                if (existingInstance == null)
                    bindPose[i] = val;
                else
                    bindPose[existingInstance.Length + i] = val; // TODO
            }

            return bindPose;
        }

        private static Matrix[] ReadInvBindPose(ContentReader input, Matrix[] existingInstance)
        {
            var invBindPose = existingInstance;

            var count = input.ReadInt32();
            if (invBindPose == null)
                invBindPose = new Matrix[count];

            for (var i = 0; i < count; i++)
            {
                var val = input.ReadMatrix();
                if (existingInstance == null)
                    invBindPose[i] = val;
                else
                    invBindPose[existingInstance.Length + i] = val; // TODO
            }

            return invBindPose;
        }

        private static int[] ReadSkeletonHierarchy(ContentReader input, int[] existingInstance)
        {
            var skeletonHierarchy = existingInstance;

            var count = input.ReadInt32();
            if (skeletonHierarchy == null)
                skeletonHierarchy = new int[count];

            for (var i = 0; i < count; i++)
            {
                var val = input.ReadInt32();
                if (existingInstance == null)
                    skeletonHierarchy[i] = val;
                else
                    skeletonHierarchy[existingInstance.Length + i] = val; // TODO
            }

            return skeletonHierarchy;
        }

        private static Dictionary<string, int> ReadBoneMap(ContentReader input, Dictionary<string, int> existingInstance)
        {
            var boneMap = existingInstance;

            var count = input.ReadInt32();
            if (boneMap == null)
                boneMap = new Dictionary<string, int>(count);

            for (var boneIndex = 0; boneIndex < count; boneIndex++)
            {
                var key = input.ReadString();
                if (existingInstance == null)
                    boneMap.Add(key, boneIndex);
                else
                    boneMap[key] = boneIndex;
            }

            return boneMap;
        }       
    }    
}