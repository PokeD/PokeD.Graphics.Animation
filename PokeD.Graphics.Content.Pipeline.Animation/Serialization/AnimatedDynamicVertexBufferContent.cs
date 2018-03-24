using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using tainicom.Aether.Content.Pipeline.Graphics;

namespace PokeD.Graphics.Content.Pipeline.Serialization
{
    public class AnimatedDynamicVertexBufferContent : DynamicVertexBufferContent
    {
        public AnimatedDynamicVertexBufferContent(VertexBufferContent source, int size = 0) : base(source, size) { }
    }
}