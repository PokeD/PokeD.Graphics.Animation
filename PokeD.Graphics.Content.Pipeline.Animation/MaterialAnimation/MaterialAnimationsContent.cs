using System.Collections.Generic;

using Microsoft.Xna.Framework.Content.Pipeline;

namespace PokeD.Graphics.Content.Pipeline.MaterialAnimation
{
    public class MaterialAnimationsContent : ContentItem
    {
        public Dictionary<string, MaterialClipContent> Clips { get; }
        
        internal MaterialAnimationsContent(Dictionary<string, MaterialClipContent> clips)
        {
            Clips = clips;
        }
    }
}