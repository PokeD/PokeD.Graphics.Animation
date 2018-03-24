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

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace tainicom.Aether.Animation
{
    public abstract class AnimatedDynamicVertexBuffer : DynamicVertexBuffer
    {
        protected AnimatedDynamicVertexBuffer(GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage bufferUsage) :
            base(graphicsDevice, vertexDeclaration, vertexCount, bufferUsage) { }

        public abstract void UpdateVertices(int startIndex, int elementCount, Matrix[] boneTransforms = null, Matrix[] materialTransform = null);
    }

    public abstract class AnimatedDynamicVertexBuffer<TCPUVertex, TGPUVertex> : AnimatedDynamicVertexBuffer where TCPUVertex : IVertexType where TGPUVertex : IVertexType
    {
        protected TCPUVertex[] CPUVertices;
        protected TGPUVertex[] GPUVertices;

        protected AnimatedDynamicVertexBuffer(GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage bufferUsage) :
            base(graphicsDevice, vertexDeclaration, vertexCount, bufferUsage) { }

        public void SetCPUVertices(TCPUVertex[] vertices) => CPUVertices = vertices;
        public void SetGPUVertices(TGPUVertex[] vertices) => GPUVertices = vertices;
    }
}