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

using System.IO;
using System.Reflection;

using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace tainicom.Aether.Content.Pipeline.Graphics
{
    public class DynamicVertexBufferContent : VertexBufferContent 
    {
        public bool IsWriteOnly = false;

        public DynamicVertexBufferContent(VertexBufferContent source, int size = 0) : base(size)
        {
            Identity = source.Identity;
            Name = source.Name;
            foreach (var keyPair in source.OpaqueData)
                OpaqueData.Add(keyPair.Key, keyPair.Value);
            VertexDeclaration = source.VertexDeclaration;
            CopyStream(source);
        }
        private void CopyStream(VertexBufferContent source)
        {
            var vertexBufferType = typeof(VertexBufferContent);
            var vertexBufferStreamField = vertexBufferType.GetField("stream", BindingFlags.Instance | BindingFlags.NonPublic);
            var vertexBufferStream = (MemoryStream) vertexBufferStreamField.GetValue(source);
            var dynamicVertexBufferStream = new MemoryStream(vertexBufferStream.ToArray());
            vertexBufferStreamField.SetValue(this, dynamicVertexBufferStream);
        }
    }
}