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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using tainicom.Aether.Content.Pipeline.Graphics;

namespace tainicom.Aether.Content.Pipeline.Serialization
{
    [ContentTypeWriter]
    public class DynamicModelWriter : ContentTypeWriter<DynamicModelContent>
    {
        /// <summary>
        /// Write a Model xnb, compatible with the XNB Container Format.
        /// </summary>
        protected override void Write(ContentWriter output, DynamicModelContent model)
        {   
            WriteBones(output, model.Bones);
            WriteMeshes(output, model, model.Meshes);
            WriteBoneReference(output, model.Bones.Count, model.Source.Root);
            output.WriteObject(model.Source.Tag);
        }


        private static void WriteBones(ContentWriter output, ModelBoneContentCollection bones)
        {
            var bonesCount = bones.Count;
            output.Write((uint) bonesCount);

            foreach (var bone in bones)
            {
                output.WriteObject(bone.Name);
                output.Write(bone.Transform);
            }

            foreach (var bone in bones)
            {
                WriteBoneReference(output, bonesCount, bone.Parent);

                output.Write((uint) bone.Children.Count);
                foreach (var child in bone.Children)
                    WriteBoneReference(output, bonesCount, child);
            }
        }

        // The BoneReference type varies in size depending on the number of bones in the model. 
        // If bone count is less than 255 this value is serialized as a Byte, otherwise it is UInt32. 
        // If the reference value is zero the bone is null, otherwise (bone reference - 1) is an index into the model bone list.
        private static void WriteBoneReference(ContentWriter output, int bonesCount, ModelBoneContent bone)
        {
            if (bone == null)
                output.Write((byte) 0);
            else if (bonesCount < 255)
                output.Write((byte) (bone.Index + 1));
            else
                output.Write((uint) (bone.Index + 1));
        }

        private static void WriteMeshes(ContentWriter output, DynamicModelContent model, List<DynamicModelMeshContent> meshes)
        {
            output.Write((uint) meshes.Count);

            var bonesCount = model.Bones.Count;
            foreach (var mesh in meshes)
            {
                output.WriteObject(mesh.Name); 
                WriteBoneReference(output, bonesCount, mesh.ParentBone);
                WriteBoundingSphere(output, mesh.BoundingSphere);
                output.WriteObject(mesh.Tag);
                
                WriteParts(output, model, mesh.MeshParts);
            }
        }

        private static void WriteBoundingSphere(ContentWriter output, BoundingSphere value)
        {
            output.Write(value.Center);
            output.Write(value.Radius);
        }

        private static void WriteParts(ContentWriter output, DynamicModelContent model, List<DynamicModelMeshPartContent> parts)
        {
            output.Write((uint) parts.Count);

            foreach (var part in parts)
            {
                output.Write((uint) part.VertexOffset);
                output.Write((uint) part.NumVertices);
                output.Write((uint) part.StartIndex);
                output.Write((uint) part.PrimitiveCount);
                output.WriteObject(part.Tag);

                output.WriteSharedResource(part.VertexBuffer);
                output.WriteSharedResource(part.IndexBuffer);
                output.WriteSharedResource(part.Material);
            }
        }
        
        public override string GetRuntimeType(TargetPlatform targetPlatform) => "Microsoft.Xna.Framework.Graphics.Model";

        public override string GetRuntimeReader(TargetPlatform targetPlatform) => "Microsoft.Xna.Framework.Content.ModelReader, Microsoft.Xna.Framework.Graphics";
    }
}