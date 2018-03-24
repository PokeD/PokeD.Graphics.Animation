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

using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

using PokeD.Graphics.Content.Pipeline.Processors;

using tainicom.Aether.Content.Pipeline.Serialization;
using tainicom.Aether.Content.Pipeline.Graphics;

namespace tainicom.Aether.Content.Pipeline.Processors
{
    [ContentProcessor(DisplayName = "CPU Animated Model - Aether")]
    public class CPUAnimatedModelProcessor : DynamicModelProcessor, IContentProcessor
    {
        // used to avoid creating clones/duplicates of the same VertexBufferContent
        protected Dictionary<VertexBufferContent, DynamicVertexBufferContent> VertexBufferCache { get; } = new Dictionary<VertexBufferContent, DynamicVertexBufferContent>();


        [DefaultValue(DynamicModelContent.BufferType.DynamicWriteOnly)]
        public override DynamicModelContent.BufferType VertexBufferType { get => base.VertexBufferType; set => base.VertexBufferType = value; }
        
        [DefaultValue(DynamicModelContent.BufferType.Default)]
        public override DynamicModelContent.BufferType IndexBufferType { get => base.IndexBufferType; set => base.IndexBufferType = value; }

        [DisplayName("Max Bones"), DefaultValue(SkinnedEffect.MaxBones)]
        public virtual int MaxBones { get; set; } = SkinnedEffect.MaxBones;

        [DisplayName("Generate Keyframes Frequency"), DefaultValue(0)] // (0=no, 30=30fps, 60=60fps)
        public virtual int GenerateKeyframesFrequency { get; set; }

        [DisplayName("Fix BoneRoot from MG importer"), DefaultValue(false)]
        public virtual bool FixRealBoneRoot { get; set; }

        public CPUAnimatedModelProcessor()
        {
            VertexBufferType = DynamicModelContent.BufferType.DynamicWriteOnly;
            IndexBufferType  = DynamicModelContent.BufferType.Default;
        }

        object IContentProcessor.Process(object input, ContentProcessorContext context)
        {
            var model = base.Process((NodeContent) input, context);
            var outputModel = new DynamicModelContent(model);
            
            foreach(var mesh in outputModel.Meshes)
            {
                foreach(var part in mesh.MeshParts)
                {
                    ProcessVertexBuffer(outputModel, context, part);
                    ProcessIndexBuffer(outputModel, context, part);
                }
            }

            var skeletalAnimationsProcessor = new SkeletalAnimationsProcessor
            {
                MaxBones = MaxBones,
                GenerateKeyframesFrequency = GenerateKeyframesFrequency,
                FixRealBoneRoot = FixRealBoneRoot
            };
            var skeletalAnimations = skeletalAnimationsProcessor.Process((NodeContent) input, context);

            var materalAnimationsProcessor = new MaterialAnimationsProcessor();
            var materalAnimations = materalAnimationsProcessor.Process((NodeContent) input, context);

            outputModel.Tag = new ModelAnimationsContent()
            {
                Identity = ((NodeContent) input).Identity,
                SkeletalAnimations = skeletalAnimations,
                MaterialAnimations = materalAnimations
            };
            return outputModel;
        }

        protected override void ProcessVertexBuffer(DynamicModelContent dynamicModel, ContentProcessorContext context, DynamicModelMeshPartContent part)
        {
            if (VertexBufferType != DynamicModelContent.BufferType.Default)
            {
                // Replace the default VertexBufferContent with CpuAnimatedVertexBufferContent.
                if (!VertexBufferCache.TryGetValue(part.VertexBuffer, out var vb))
                {
                    vb = new DefaultAnimatedDynamicVertexBufferContent(part.VertexBuffer) { IsWriteOnly = VertexBufferType == DynamicModelContent.BufferType.DynamicWriteOnly };
                    VertexBufferCache[part.VertexBuffer] = vb;
                }
                part.VertexBuffer = vb;
            }
        }
    }
}