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
using Microsoft.Xna.Framework.Content.Pipeline;

namespace tainicom.Aether.Content.Pipeline.Animation
{
    public class SkeletalAnimationsContent : ContentItem
    {
        public List<Matrix> BindPose { get; }
        public List<Matrix> InvBindPose { get; }
        public List<int> SkeletonHierarchy { get; }
        public List<string> BoneNames { get; }
        public Dictionary<string, SkeletalClipContent> Clips { get; }


        internal SkeletalAnimationsContent(List<Matrix> bindPose, List<Matrix> invBindPose, List<int> skeletonHierarchy, List<string> boneNames, Dictionary<string, SkeletalClipContent> clips)
        {
            BindPose = bindPose;
            InvBindPose = invBindPose;
            SkeletonHierarchy = skeletonHierarchy;
            BoneNames = boneNames;
            Clips = clips;
        }
    }
}