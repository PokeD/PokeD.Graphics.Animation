using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace PokeD.Graphics.MaterialAnimation
{
    public struct MaterialKeyframe
    {
        public string Material { get; internal set; }
        public TimeSpan Time { get; internal set; }
        public Matrix[] Transforms { get; internal set; }

        public Dictionary<string, object> OpaqueData { get; internal set; }

        public MaterialKeyframe(string material, TimeSpan time, Matrix[] transforms, Dictionary<string, object> opaqueData)
        {
            Material = material;
            Time = time;
            Transforms = transforms;
            OpaqueData = opaqueData;
        }
    }
}