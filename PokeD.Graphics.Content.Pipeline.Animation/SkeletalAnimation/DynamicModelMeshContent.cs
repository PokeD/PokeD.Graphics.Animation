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

using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace tainicom.Aether.Content.Pipeline.Graphics
{
    public class DynamicModelMeshContent
    {   
        private ModelMeshContent Source { get; }

        /// <summary>
        /// Gets the mesh name.
        /// </summary>
        public string Name => Source.Name;

        /// <summary>
        /// Gets the parent bone.
        /// </summary>
        [ContentSerializerIgnore]
        public ModelBoneContent ParentBone => Source.ParentBone;

        /// <summary>
        /// Gets the bounding sphere for this mesh.
        /// </summary>
        public BoundingSphere BoundingSphere => Source.BoundingSphere;

        /// <summary>
        /// Gets the children mesh parts associated with this mesh.
        /// </summary>
        [ContentSerializerIgnore]
        public List<DynamicModelMeshPartContent> MeshParts { get; }

        /// <summary>
        /// Gets a user defined tag object.
        /// </summary>
        [ContentSerializer(SharedResource = true)]
        public object Tag { get => Source.Tag; }


        public DynamicModelMeshContent(ModelMeshContent source)
        {
            Source = source;
            MeshParts = source.MeshParts.Select(meshPart => new DynamicModelMeshPartContent(meshPart)).ToList();
        }
    }
}