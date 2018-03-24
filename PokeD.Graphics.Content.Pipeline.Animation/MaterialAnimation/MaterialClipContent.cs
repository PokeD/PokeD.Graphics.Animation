using System;

using Microsoft.Xna.Framework.Content.Pipeline;

namespace PokeD.Graphics.Content.Pipeline.MaterialAnimation
{
    public class MaterialClipContent : ContentItem
    {
        public TimeSpan Duration { get; }
        public MaterialKeyframeContent[] Keyframes { get; }

        internal MaterialClipContent(TimeSpan duration, MaterialKeyframeContent[] keyframes)
        {
            Duration = duration;
            Keyframes = keyframes;
        }
    }
}