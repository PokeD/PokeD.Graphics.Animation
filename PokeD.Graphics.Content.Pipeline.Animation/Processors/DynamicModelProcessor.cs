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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using tainicom.Aether.Content.Pipeline.Graphics;

namespace tainicom.Aether.Content.Pipeline.Serialization
{
    [ContentProcessor(DisplayName = "Dynamic Model - Aether")]
    public abstract class DynamicModelProcessor : ModelProcessor, IContentProcessor
    {
        // used to avoid creating clones/duplicates of the same VertexBufferContent
        private Dictionary<VertexBufferContent, DynamicVertexBufferContent> VertexBufferCache { get; } = new Dictionary<VertexBufferContent, DynamicVertexBufferContent>();
        private Dictionary<Collection<int>, DynamicIndexBufferContent> IndexBufferCache { get; } = new Dictionary<Collection<int>, DynamicIndexBufferContent>();

        Type IContentProcessor.OutputType => typeof(DynamicModelContent);

        [DefaultValue(DynamicModelContent.BufferType.Dynamic)]
        public virtual DynamicModelContent.BufferType VertexBufferType { get; set; } = DynamicModelContent.BufferType.Dynamic;

        [DefaultValue(DynamicModelContent.BufferType.Dynamic)]
        public virtual DynamicModelContent.BufferType IndexBufferType { get; set; } = DynamicModelContent.BufferType.Dynamic;

        object IContentProcessor.Process(object input, ContentProcessorContext context)
        {
            var model = Process(input as NodeContent, context);
            var dynamicModel = new DynamicModelContent(model);
            
            foreach(var mesh in dynamicModel.Meshes)
            {
                foreach(var part in mesh.MeshParts)
                {
                    ProcessVertexBuffer(dynamicModel, context, part);
                    ProcessIndexBuffer(dynamicModel, context, part);
                }
            }

            return dynamicModel;
        }

        protected virtual void ProcessVertexBuffer(DynamicModelContent dynamicModel, ContentProcessorContext context, DynamicModelMeshPartContent part)
        {
            if(VertexBufferType != DynamicModelContent.BufferType.Default)
            {
                // Replace the default VertexBufferContent with CpuAnimatedVertexBufferContent.
                if (!VertexBufferCache.TryGetValue(part.VertexBuffer, out var vb))
                {
                    vb = part.VertexBuffer;
                    vb.IsWriteOnly = VertexBufferType == DynamicModelContent.BufferType.DynamicWriteOnly;
                    VertexBufferCache[part.VertexBuffer] = vb;
                }
                part.VertexBuffer = vb;
            }
        }

        protected virtual void ProcessIndexBuffer(DynamicModelContent dynamicModel, ContentProcessorContext context, DynamicModelMeshPartContent part)
        {
            if(IndexBufferType != DynamicModelContent.BufferType.Default)
            {
                if (!IndexBufferCache.TryGetValue(part.IndexBuffer, out var ib))
                {
                    ib = part.IndexBuffer;
                    ib.IsWriteOnly = IndexBufferType == DynamicModelContent.BufferType.DynamicWriteOnly;
                    IndexBufferCache[part.IndexBuffer] = ib;
                }
                part.IndexBuffer = ib;
            }
        }
    }
}