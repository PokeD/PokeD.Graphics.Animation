#region License
//   Copyright 2011-2016 Kastellanos Nikolaos
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
#endregion

using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace tainicom.Aether.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DefaultCPUVertex : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Byte4 BlendIndices;
        public Vector4 BlendWeights;
        
        #region IVertexType Members
        private static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0),
            new VertexElement(28, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0));

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
        #endregion
        
        public DefaultCPUVertex(Vector3 position, Vector3 normal, Byte4 blendIndices, Vector4 blendWeights)
        {
            Position = position;
            Normal = normal;
            BlendIndices = blendIndices;
            BlendWeights = blendWeights;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Position.GetHashCode() * 397) ^
                       Normal.GetHashCode() ^
                       BlendIndices.GetHashCode() ^
                       BlendWeights.GetHashCode();
            }
        }

        public override string ToString() =>
            $"{{Position:{Position} Normal:{Normal} BlendIndices:{BlendIndices} BlendWeights:{BlendWeights}}}";

        public static bool operator ==(DefaultCPUVertex left, DefaultCPUVertex right) =>
            left.Position == right.Position &&
            left.Normal == right.Normal &&
            left.BlendIndices == right.BlendIndices &&
            left.BlendWeights == right.BlendWeights;
        public static bool operator !=(DefaultCPUVertex left, DefaultCPUVertex right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return this == (DefaultCPUVertex) obj;
        }
    }
}