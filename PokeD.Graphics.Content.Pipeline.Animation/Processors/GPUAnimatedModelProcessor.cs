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
using System.ComponentModel;

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

using PokeD.Graphics.Content.Pipeline.Processors;

namespace tainicom.Aether.Content.Pipeline.Processors
{
    [ContentProcessor(DisplayName = "GPU Animated Model - Aether")]
    public class GPUAnimatedModelProcessor : ModelProcessor
    {
        [DisplayName("MaxBones"), DefaultValue(SkinnedEffect.MaxBones)]
        public virtual int MaxBones { get; set; } = SkinnedEffect.MaxBones;

        [DisplayName("Generate Keyframes Frequency"), DefaultValue(0)] // (0=no, 30=30fps, 60=60fps)
        public virtual int GenerateKeyframesFrequency { get; set; }

        [DisplayName("Fix BoneRoot from MG importer"), DefaultValue(false)]
        public virtual bool FixRealBoneRoot { get; set; }

        [DefaultValue(MaterialProcessorDefaultEffect.SkinnedEffect)]
        public override MaterialProcessorDefaultEffect DefaultEffect { get => base.DefaultEffect; set => base.DefaultEffect = value; }

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            context.Logger.LogMessage("Processing CPU Animated Model");
            try
            {
                var skeletalAnimationsProcessor = new SkeletalAnimationsProcessor
                {
                    MaxBones = MaxBones,
                    GenerateKeyframesFrequency = GenerateKeyframesFrequency,
                    FixRealBoneRoot = FixRealBoneRoot
                };
                var skeletalAnimations = skeletalAnimationsProcessor.Process(input, context);

                var materalAnimationsProcessor = new MaterialAnimationsProcessor();
                var materalAnimations = materalAnimationsProcessor.Process(input, context);


                var model = base.Process(input, context);
                model.Tag = new ModelAnimationsContent()
                {
                    Identity = input.Identity,
                    SkeletalAnimations = skeletalAnimations,
                    MaterialAnimations = materalAnimations
                };
                return model;
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }
    }
}