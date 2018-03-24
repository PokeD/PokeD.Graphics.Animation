using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;

using PokeD.Graphics.MaterialAnimation;

namespace PokeD.Graphics.Content.ContentReaders
{
    public class MaterialAnimationsReader : ContentTypeReader<MaterialAnimations>
    {
        protected override MaterialAnimations Read(ContentReader input, MaterialAnimations existingInstance)
        {
            var animations = existingInstance;

            if (existingInstance == null)
            {
                var clips = ReadAnimationClips(input, null);
                animations = new MaterialAnimations(clips);
            }
            else
            {
                ReadAnimationClips(input, animations.Clips);
            }

            return animations;
        }

        private static Dictionary<string, MaterialClip> ReadAnimationClips(ContentReader input, Dictionary<string, MaterialClip> existingInstance)
        {
            var animationClips = existingInstance;

            var count = input.ReadInt32();
            if (animationClips == null)
                animationClips = new Dictionary<string, MaterialClip>(count);

            for (var i = 0; i < count; i++)
            {
                var key = input.ReadString();
                var val = input.ReadObject<MaterialClip>();
                if (existingInstance == null)
                    animationClips.Add(key, val);
                else
                    animationClips[key] = val;
            }

            return animationClips;
        }
    }
}