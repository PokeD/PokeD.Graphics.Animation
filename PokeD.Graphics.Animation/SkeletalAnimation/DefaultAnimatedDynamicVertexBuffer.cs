#region License
//   Copyright 2015-2016 Kastellanos Nikolaos
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using tainicom.Aether.Graphics;

namespace tainicom.Aether.Animation
{
    public class DefaultAnimatedDynamicVertexBuffer : AnimatedDynamicVertexBuffer<DefaultCPUVertex, VertexPositionNormalTexture>
    {
        public DefaultAnimatedDynamicVertexBuffer(GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage bufferUsage) :
            base(graphicsDevice, vertexDeclaration, vertexCount, bufferUsage) { }

        public override void UpdateVertices(int startIndex, int elementCount, Matrix[] boneTransforms = null, Matrix[] materialTransform = null)
        {
            var transformSum = Matrix.Identity;

            // skin all of the vertices
            for (var i = startIndex; i < startIndex + elementCount; i++)
            {
                if (boneTransforms != null)
                {
                    var w1 = CPUVertices[i].BlendWeights.X;
                    var w2 = CPUVertices[i].BlendWeights.Y;
                    var w3 = CPUVertices[i].BlendWeights.Z;
                    var w4 = CPUVertices[i].BlendWeights.W;

                    var indices = CPUVertices[i].BlendIndices.ToVector4();

                    var m1 = boneTransforms[(int) indices.X];
                    var m2 = boneTransforms[(int) indices.Y];
                    var m3 = boneTransforms[(int) indices.Z];
                    var m4 = boneTransforms[(int) indices.W];

                    transformSum.M11 = m1.M11 * w1 + m2.M11 * w2 + m3.M11 * w3 + m4.M11 * w4;
                    transformSum.M12 = m1.M12 * w1 + m2.M12 * w2 + m3.M12 * w3 + m4.M12 * w4;
                    transformSum.M13 = m1.M13 * w1 + m2.M13 * w2 + m3.M13 * w3 + m4.M13 * w4;

                    transformSum.M21 = m1.M21 * w1 + m2.M21 * w2 + m3.M21 * w3 + m4.M21 * w4;
                    transformSum.M22 = m1.M22 * w1 + m2.M22 * w2 + m3.M22 * w3 + m4.M22 * w4;
                    transformSum.M23 = m1.M23 * w1 + m2.M23 * w2 + m3.M23 * w3 + m4.M23 * w4;

                    transformSum.M31 = m1.M31 * w1 + m2.M31 * w2 + m3.M31 * w3 + m4.M31 * w4;
                    transformSum.M32 = m1.M32 * w1 + m2.M32 * w2 + m3.M32 * w3 + m4.M32 * w4;
                    transformSum.M33 = m1.M33 * w1 + m2.M33 * w2 + m3.M33 * w3 + m4.M33 * w4;

                    transformSum.M41 = m1.M41 * w1 + m2.M41 * w2 + m3.M41 * w3 + m4.M41 * w4;
                    transformSum.M42 = m1.M42 * w1 + m2.M42 * w2 + m3.M42 * w3 + m4.M42 * w4;
                    transformSum.M43 = m1.M43 * w1 + m2.M43 * w2 + m3.M43 * w3 + m4.M43 * w4;

                    // Support the 4 Bone Influences - Position then Normal
                    Vector3.Transform(ref CPUVertices[i].Position, ref transformSum, out GPUVertices[i].Position);
                    Vector3.TransformNormal(ref CPUVertices[i].Normal, ref transformSum, out GPUVertices[i].Normal);
                }
            }

            // put the vertices into our vertex buffer
            SetData(GPUVertices, 0, VertexCount, SetDataOptions.NoOverwrite);
        }
    }
}