using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace PokeD.Graphics.Content.Pipeline.MaterialAnimation
{
    public sealed class MaterialAnimationKeyframe : ContentItem, IComparable<MaterialAnimationKeyframe>
    {
        public string Material { get; set; }
        public TimeSpan Time { get; set; }
        public Matrix[] Transforms { get; set; }

        public MaterialAnimationKeyframe(TimeSpan time)
        {
            Time = time;
        }

        public int CompareTo(MaterialAnimationKeyframe other) => Time.CompareTo(other.Time);
    }
}