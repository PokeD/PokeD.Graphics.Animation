#region License
//   Copyright 2016 Kastellanos Nikolaos
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

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace tainicom.Aether.Content.Pipeline.Graphics
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicModelMeshPartContent
    {
        private ModelMeshPartContent Source { get; }


        public int VertexOffset => Source.VertexOffset;

        public int NumVertices => Source.NumVertices;

        public int StartIndex => Source.StartIndex;

        public int PrimitiveCount => Source.PrimitiveCount;
        
        [ContentSerializerIgnore]
        public DynamicVertexBufferContent VertexBuffer { get; set; }

        [ContentSerializerIgnore]
        public DynamicIndexBufferContent IndexBuffer { get; set; }

        [ContentSerializer(SharedResource = true)]
        public MaterialContent Material => Source.Material;

        [ContentSerializer(SharedResource = true)]
        public object Tag => Source.Tag;


        public DynamicModelMeshPartContent(ModelMeshPartContent source)
        {
            Source = source;
            VertexBuffer = new DynamicVertexBufferContent(source.VertexBuffer);
            IndexBuffer = new DynamicIndexBufferContent(source.IndexBuffer);
        }
    }
}