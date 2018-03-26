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

using System;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

using tainicom.Aether.Animation;

namespace tainicom.Aether.Graphics.Content
{
    public class DefaultAnimatedDynamicVertexBufferReader : ContentTypeReader<DefaultAnimatedDynamicVertexBuffer>
    {
        protected override DefaultAnimatedDynamicVertexBuffer Read(ContentReader input, DefaultAnimatedDynamicVertexBuffer buffer)
        {
            var graphicsDeviceService = (IGraphicsDeviceService) input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
            var device = graphicsDeviceService.GraphicsDevice;

            // read standard VertexBuffer
            var declaration = input.ReadRawObject<VertexDeclaration>();
            var vertexCount = (int) input.ReadUInt32();

            //read data                      
            var channels = declaration.GetVertexElements();
            var cpuVertices = new DefaultCPUVertex[vertexCount];
            var gpuVertices = new VertexPositionNormalTexture[vertexCount];

            for (var i = 0; i < vertexCount; i++)
            {
                foreach (var channel in channels)
                {
                    switch (channel.VertexElementUsage)
                    {
                        case VertexElementUsage.Position:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Vector3);
                            var pos = input.ReadVector3();
                            cpuVertices[i].Position = pos;
                            gpuVertices[i].Position = pos;
                            break;

                        case VertexElementUsage.Normal:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Vector3);
                            var nor = input.ReadVector3();
                            cpuVertices[i].Normal = nor;
                            gpuVertices[i].Normal = nor;
                            break;

                        case VertexElementUsage.Color:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Color);
                            var col = input.ReadColor();
                            break;

                        case VertexElementUsage.Tangent:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Vector3);
                            var tan = input.ReadVector3();
                            break;

                        case VertexElementUsage.Binormal:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Vector3);
                            var bin = input.ReadVector3();
                            break;

                        case VertexElementUsage.TextureCoordinate:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Vector2);
                            var tex = input.ReadVector2();
                            gpuVertices[i].TextureCoordinate = tex;
                            break;

                        case VertexElementUsage.BlendWeight:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Vector4);
                            var wei = input.ReadVector4();
                            cpuVertices[i].BlendWeights = wei;
                            break;

                        case VertexElementUsage.BlendIndices:
                            System.Diagnostics.Debug.Assert(channel.VertexElementFormat == VertexElementFormat.Byte4);
                            var ind = new Byte4(input.ReadByte(), input.ReadByte(), input.ReadByte(), input.ReadByte());
                            cpuVertices[i].BlendIndices = ind; 
                            break;

                        default:
                            throw new Exception();
                    }
                }
            }

            // read extras
            var isWriteOnly = input.ReadBoolean();

            if (buffer == null)
            {
                var usage = isWriteOnly ? BufferUsage.WriteOnly : BufferUsage.None;
                buffer = new DefaultAnimatedDynamicVertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertexCount, usage);
            }

            buffer.SetData(gpuVertices, 0, vertexCount);
            buffer.SetGPUVertices(gpuVertices);
            buffer.SetCPUVertices(cpuVertices);

            return buffer;
        }
    }
}