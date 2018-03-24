using Microsoft.Xna.Framework.Content.Pipeline;

using PokeD.Graphics.Content.Pipeline.MaterialAnimation;

using tainicom.Aether.Content.Pipeline.Animation;

namespace PokeD.Graphics.Content.Pipeline.Processors
{
    public class ModelAnimationsContent : ContentItem
    {
        public SkeletalAnimationsContent SkeletalAnimations;
        public MaterialAnimationsContent MaterialAnimations;
    }
}