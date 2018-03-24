using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using PokeD.Graphics.Content.Pipeline.Processors;

namespace PokeD.Graphics.Content.Pipeline.Serialization
{
    [ContentTypeWriter]
    public class ModelAnimationsWriter : ContentTypeWriter<ModelAnimationsContent>
    {
        protected override void Write(ContentWriter writer, ModelAnimationsContent value)
        {
            writer.WriteObject(value.SkeletalAnimations);
            writer.WriteObject(value.MaterialAnimations);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform) => "PokeD.Graphics.Content.ContentReaders.ModelAnimationsReader, PokeD.Graphics.Animation";
    }
}