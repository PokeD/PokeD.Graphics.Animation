using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using tainicom.Aether.Content.Pipeline.Graphics;

namespace PokeD.Graphics.Content.Pipeline.Serialization
{
    [ContentTypeWriter]
    public class AnimatedDynamicVertexBufferWriter : ContentTypeWriter<AnimatedDynamicVertexBufferContent>
    {
        protected override void Write(ContentWriter output, AnimatedDynamicVertexBufferContent buffer)
        {
            WriteVertexBuffer(output, buffer);

            output.Write(buffer.IsWriteOnly);
        }

        private static void WriteVertexBuffer(ContentWriter output, DynamicVertexBufferContent buffer)
        {
            var vertexCount = buffer.VertexData.Length / buffer.VertexDeclaration.VertexStride;
            output.WriteRawObject(buffer.VertexDeclaration);
            output.Write((uint)vertexCount);
            output.Write(buffer.VertexData);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform) => "PokeD.Graphics.Content.ContentReaders.H3DAnimatedDynamicVertexBufferReader, PokeD.Graphics.Animation";
    }
}