using System.Collections.Generic;

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using PokeD.Graphics.Content.Pipeline.MaterialAnimation;

namespace PokeD.Graphics.Content.Pipeline.Serialization
{
    [ContentTypeWriter]
    public class MaterialAnimationsWriter : ContentTypeWriter<MaterialAnimationsContent>
    {
        protected override void Write(ContentWriter writer, MaterialAnimationsContent value)
        {
            WriteClips(writer, value.Clips);
        }

        private static void WriteClips(ContentWriter output, Dictionary<string, MaterialClipContent> clips)
        {
            var count = clips.Count;
            output.Write(count);

            foreach (var clip in clips)
            {
                output.Write(clip.Key);
                output.WriteObject(clip.Value);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform) => "PokeD.Graphics.Content.ContentReaders.MaterialAnimationsReader, PokeD.Graphics.Animation";
    }
}