using System;

using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace PokeD.Graphics.Content.Pipeline.MaterialAnimation
{
    /// <summary>Provides properties for maintaining an animation.</summary>
    public class MaterialAnimationContent : NodeContent
    {
        /// <summary>
        /// Gets the collection of animation data channels. Each channel describes the movement of a single bone or rigid object.
        /// </summary>
        public MaterialAnimationChannelDictionary Channels { get; } = new MaterialAnimationChannelDictionary();

        /// <summary>Gets or sets the total length of the animation.</summary>
        public TimeSpan Duration { get; set; }
    }
}