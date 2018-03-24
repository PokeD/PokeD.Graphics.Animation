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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PokeD.Graphics.Content.ContentReaders;
using PokeD.Graphics.MaterialAnimation;

namespace tainicom.Aether.Animation
{
    public static class ModelExtensions
    {
        public static SkeletalAnimations GetSkeletalAnimations(this Model model)
        {
            var animations = model.Tag as ModelAnimations;
            return animations.SkeletalAnimations;
        }

        public static MaterialAnimations GetMaterialAnimations(this Model model)
        {
            var animations = model.Tag as ModelAnimations;
            return animations.MaterialAnimations;
        }

        public static bool TryUpdateVertices(this ModelMeshPart meshPart, Matrix[] boneTransforms = null, Matrix[] materialTransform = null)
        {
            if (meshPart.VertexBuffer is AnimatedDynamicVertexBuffer animatedVertexBuffer)
            {
                animatedVertexBuffer.UpdateVertices(meshPart.VertexOffset, meshPart.NumVertices, boneTransforms, materialTransform);
                return true;
            }
            return false;
        }
    }
}