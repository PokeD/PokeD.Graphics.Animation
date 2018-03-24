using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using PokeD.Graphics.MaterialAnimation;

namespace PokeD.Graphics.Content.ContentReaders
{
    public class MaterialClipReader : ContentTypeReader<MaterialClip>
    {
        protected override MaterialClip Read(ContentReader input, MaterialClip existingInstance)
        {
            var animationClip = existingInstance;

            if (existingInstance == null)
            {
                var duration = ReadDuration(input);
                var keyframes = ReadKeyframes(input, null);
                animationClip = new MaterialClip(duration, keyframes);
            }
            else
            {
                animationClip.Duration = ReadDuration(input);
                ReadKeyframes(input, animationClip.Keyframes);
            }

            return animationClip;
        }

        private static TimeSpan ReadDuration(ContentReader reader) => new TimeSpan(reader.ReadInt64());

        private static MaterialKeyframe[] ReadKeyframes(ContentReader reader, MaterialKeyframe[] existingInstance)
        {
            var keyframes = existingInstance;

            var count = reader.ReadInt32();
            if (keyframes == null)
                keyframes = new MaterialKeyframe[count];

            for (var i = 0; i < count; i++)
            {
                keyframes[i].Material = reader.ReadString();
                keyframes[i].Time = new TimeSpan(reader.ReadInt64());
                var transformCount = reader.ReadInt32();
                keyframes[i].Transforms = new Matrix[transformCount];
                for (var j = 0; j < keyframes[i].Transforms.Length; j++)
                    keyframes[i].Transforms[j] = reader.ReadMatrix();

                keyframes[i].OpaqueData = reader.ReadObject<Dictionary<string, object>>();
            }

            return keyframes;
        }
    }
}