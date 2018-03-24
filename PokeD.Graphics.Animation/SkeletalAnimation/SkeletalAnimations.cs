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

using Microsoft.Xna.Framework;

namespace tainicom.Aether.Animation
{
    public class SkeletalAnimations
    {
        internal readonly Matrix[] BindPose;
        internal readonly Matrix[] InvBindPose; // TODO: convert those from List<T> to simple T[] arrays.
        internal readonly int[] SkeletonHierarchy;
        internal readonly Dictionary<string, int> BoneMap;

        private int _currentKeyframe;


        public Dictionary<string, SkeletalClip> Clips { get; }
        public SkeletalClip CurrentClip { get; private set; }
        public TimeSpan CurrentTime { get; private set; }

        /// <summary>
        /// The current bone transform matrices, relative to their parent bones.
        /// </summary>
        public Matrix[] BoneTransforms { get; }

        /// <summary>
        /// The current bone transform matrices, in absolute format.
        /// </summary>
        public Matrix[] WorldTransforms { get; }

        /// <summary>
        /// The current bone transform matrices, relative to the animation bind pose.
        /// </summary>
        public Matrix[] AnimationTransforms { get; }


        internal SkeletalAnimations(Matrix[] bindPose, Matrix[] invBindPose, int[] skeletonHierarchy, Dictionary<string, int> boneMap, Dictionary<string, SkeletalClip> clips)
        {
            BindPose = bindPose;
            InvBindPose = invBindPose;
            SkeletonHierarchy = skeletonHierarchy;
            BoneMap = boneMap;
            Clips = clips;
            
            // initialize
            BoneTransforms = new Matrix[BindPose.Length];
            WorldTransforms = new Matrix[BindPose.Length];
            AnimationTransforms = new Matrix[BindPose.Length];
        }

        public void SetClip(string clipName) => SetClip(Clips[clipName]);
        public void SetClip(SkeletalClip clip)
        {
            //if (clip == null)
            //    throw new ArgumentNullException(nameof(clip));

            CurrentClip = clip;
            CurrentTime = TimeSpan.Zero;
            _currentKeyframe = 0;

            // Initialize bone transforms to the bind pose.
            BindPose.CopyTo(BoneTransforms, 0);
        }
        
        public int GetBoneIndex(string boneName)
        {
            if (!BoneMap.TryGetValue(boneName, out var boneIndex))
                boneIndex = -1;
            return boneIndex;
        }

        public void Update(TimeSpan time, bool relativeToCurrentTime, Matrix rootTransform)
        {
            UpdateBoneTransforms(time, relativeToCurrentTime);
            UpdateWorldTransforms(rootTransform);
            UpdateAnimationTransforms();
        }

        public void UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime)
        {
            // Update the animation position.
            if (relativeToCurrentTime)
            {
                time += CurrentTime;

                // If we reached the end, loop back to the start.
                while (time >= CurrentClip.Duration)
                    time -= CurrentClip.Duration;
            }

            if (time < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(time));
            if (time > CurrentClip.Duration)
                throw new ArgumentOutOfRangeException(nameof(time));

            // If the position moved backwards, reset the keyframe index.
            if (time < CurrentTime)
            {
                _currentKeyframe = 0;
                BindPose.CopyTo(BoneTransforms, 0);
            }

            CurrentTime = time;

            // Read keyframe matrices.
            var keyframes = CurrentClip.Keyframes;
            while (_currentKeyframe < keyframes.Length)
            {
                var keyframe = keyframes[_currentKeyframe];

                // Stop when we've read up to the current time position.
                if (keyframe.Time > CurrentTime)
                    break;

                // Use this keyframe.
                BoneTransforms[keyframe.Bone] = keyframe.Transform;

                _currentKeyframe++;
            }
        }

        public void UpdateWorldTransforms(Matrix rootTransform)
        {
            // Root bone.
            Matrix.Multiply(ref BoneTransforms[0], ref rootTransform, out WorldTransforms[0]);

            // Child bones.
            for (var bone = 1; bone < WorldTransforms.Length; bone++)
            {
                var parentBone = SkeletonHierarchy[bone];

                Matrix.Multiply(ref BoneTransforms[bone], ref WorldTransforms[parentBone], out WorldTransforms[bone]);
            }
        }

        public void UpdateAnimationTransforms()
        {
            for (var bone = 0; bone < AnimationTransforms.Length; bone++)
                Matrix.Multiply(ref InvBindPose[bone], ref WorldTransforms[bone], out AnimationTransforms[bone]);
        }
    }
}