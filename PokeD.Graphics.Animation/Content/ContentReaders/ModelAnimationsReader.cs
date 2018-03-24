using Microsoft.Xna.Framework.Content;

using PokeD.Graphics.MaterialAnimation;

using tainicom.Aether.Animation;

namespace PokeD.Graphics.Content.ContentReaders
{
    public class ModelAnimationsReader : ContentTypeReader<ModelAnimations>
    {
        protected override ModelAnimations Read(ContentReader reader, ModelAnimations existingInstance)
        {
            var skeletalAnimations = reader.ReadObject<SkeletalAnimations>();
            var materialAnimations = reader.ReadObject<MaterialAnimations>();

            return new ModelAnimations()
            {
                SkeletalAnimations = skeletalAnimations,
                MaterialAnimations = materialAnimations
            };
        }
    }
}