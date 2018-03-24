using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace PokeD.Graphics.MaterialAnimation
{
    public class MaterialAnimations
    {
        private int _currentKeyframe;
        private MaterialKeyframe _keyframe;
        public string CurrentMaterial => _keyframe.Material;

        public Dictionary<string, MaterialClip> Clips { get; }
        public MaterialClip CurrentClip { get; private set; }
        public TimeSpan CurrentTime { get; private set; }

        public Matrix[] WorldTransforms { get; }

        public Matrix[] AnimationTransforms { get; }

        internal MaterialAnimations(Dictionary<string, MaterialClip> clips)
        {
            Clips = clips;

            WorldTransforms = new Matrix[3];
            AnimationTransforms = new Matrix[3];
        }

        public void SetClip(string clipName) => SetClip(Clips[clipName]);
        public void SetClip(MaterialClip clip)
        {
            //if (clip == null)
            //    throw new ArgumentNullException(nameof(clip));

            CurrentClip = clip;
            CurrentTime = TimeSpan.Zero;
            _currentKeyframe = 0;
        }

        public void Update(TimeSpan time, bool relativeToCurrentTime, Matrix rootTransform)
        {
            UpdateBoneTransforms(time, relativeToCurrentTime);
            UpdateTransforms(rootTransform);
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
            }

            CurrentTime = time;

            // Read keyframe matrices.
            var keyframes = CurrentClip.Keyframes;
            while (_currentKeyframe < keyframes.Length)
            {
                _keyframe = keyframes[_currentKeyframe];

                // Stop when we've read up to the current time position.
                if (_keyframe.Time > CurrentTime)
                    break;

                _currentKeyframe++;
            }
        }

        public void UpdateTransforms(Matrix rootTransform)
        {
            Matrix.Multiply(ref _keyframe.Transforms[0], ref rootTransform, out AnimationTransforms[0]);
            Matrix.Multiply(ref _keyframe.Transforms[1], ref rootTransform, out AnimationTransforms[1]);
            Matrix.Multiply(ref _keyframe.Transforms[2], ref rootTransform, out AnimationTransforms[2]);
        }
    }
}