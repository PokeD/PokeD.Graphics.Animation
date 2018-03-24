using System;

namespace PokeD.Graphics.MaterialAnimation
{
    public class MaterialClip
    {
        public TimeSpan Duration { get; internal set; }
        public MaterialKeyframe[] Keyframes { get; }

        internal MaterialClip(TimeSpan duration, MaterialKeyframe[] keyframes)
        {
            Duration = duration;
            Keyframes = keyframes;
        }
    }
}