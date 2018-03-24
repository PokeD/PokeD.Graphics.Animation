using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace PokeD.Graphics.Content.Pipeline.MaterialAnimation
{
    public class MaterialKeyframeContent : ContentItem
    {
        public string Material { get; }
        public TimeSpan Time { get; }
        public Matrix[] Transforms { get; }
        
        public MaterialKeyframeContent(string name, MaterialAnimationKeyframe keyframe)
        {
            Material = name;
            Time = keyframe.Time;
            Transforms = keyframe.Transforms;
        }
    }
}