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
using System.Linq;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace tainicom.Aether.Content.Pipeline.Graphics 
{    
    public class DynamicModelContent
    {
        [Flags]
        public enum BufferType
        {
            /// <summary>
            /// Use the default BufferReader
            /// </summary>
            Default = int.MinValue,

            /// <summary>
            /// Deserialize a Dynamic Buffer
            /// </summary> 
            Dynamic = 0,
            
            /// <summary>
            /// Deserialize a Dynamic Buffer with BufferUsage.WriteOnly
            /// </summary>
            DynamicWriteOnly = 0x01,
        }

        protected internal ModelContent Source { get; }
        public BufferType VertexBufferType = BufferType.Dynamic;
        public BufferType IndexBufferType = BufferType.Dynamic;

        /// <summary>
        /// Gets the collection of bones that are referenced by this model.
        /// </summary>
        public ModelBoneContentCollection Bones => Source.Bones;

        /// <summary>
        /// Gets the collection of meshes that are associated with this model.
        /// </summary>
        [ContentSerializerIgnore]
        public List<DynamicModelMeshContent> Meshes { get; }

        /// <summary>
        /// Gets the root bone of this model.
        /// </summary>
        [ContentSerializerIgnore]
        public ModelBoneContent Root => Source.Root;

        /// <summary>
        /// Gets a user defined tag object.
        /// </summary>
        [ContentSerializer(SharedResource = true)]
        public object Tag { get => Source.Tag; set => Source.Tag = value; }

        public DynamicModelContent(ModelContent source)
        {
            Source = source;
            Meshes = source.Meshes.Select(mesh => new DynamicModelMeshContent(mesh)).ToList();
        }
    }
}