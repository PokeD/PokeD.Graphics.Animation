using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using PokeD.Graphics.Content.Pipeline.MaterialAnimation;

namespace PokeD.Graphics.Content.Pipeline.Serialization
{
    [ContentTypeWriter]
    public class MaterialClipWriter : ContentTypeWriter<MaterialClipContent>
    {
        protected override void Write(ContentWriter output, MaterialClipContent value)
        {
            WriteDuration(output, value.Duration);
            WriteKeyframes(output, value.Keyframes);
        }

        private static void WriteDuration(ContentWriter output, TimeSpan duration)
        {
            output.Write(duration.Ticks);
        }

        private static void WriteKeyframes(ContentWriter output, IList<MaterialKeyframeContent> keyframes)
        {
            output.Write(keyframes.Count);
            foreach (var keyframe in keyframes)
            {
                output.Write(keyframe.Material);
                output.Write(keyframe.Time.Ticks);
                output.Write(keyframe.Transforms.Length);
                foreach (var transform in keyframe.Transforms)
                    output.Write(transform);

                var dic = new Dictionary<string, object>();
                foreach (var keyPair in keyframe.OpaqueData)
                    dic.Add(keyPair.Key, keyPair.Value);
                
                output.WriteObject(dic);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform) => "PokeD.Graphics.Content.ContentReaders.MaterialClipReader, PokeD.Graphics.Animation";
    }
}