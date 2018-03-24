# PokeD.Graphics.Animation

Portion of code taken from [tainicom.Aether.Animation](https://github.com/tainicom/Aether.Extras/tree/master/Animation)

## Importers
* [Skeletal Animation](https://github.com/PokeD/PokeD.Graphics.Animation/blob/master/PokeD.Graphics.Content.Pipeline.Animation/Processors/SkeletalAnimationsProcessor.cs) - Import animations from a [Model](https://github.com/MonoGame/MonoGame/blob/master/MonoGame.Framework/Graphics/Model.cs).
* [Material Animation (WIP)](https://github.com/PokeD/PokeD.Graphics.Animation/blob/master/PokeD.Graphics.Content.Pipeline.Animation/Processors/MaterialAnimationsProcessor.cs) - Import animations from a [Model](https://github.com/MonoGame/MonoGame/blob/master/MonoGame.Framework/Graphics/Model.cs).
* [GPU Animated Model](https://github.com/PokeD/PokeD.Graphics.Animation/blob/master/PokeD.Graphics.Content.Pipeline.Animation/Processors/CPUAnimatedModelProcessor.cs) - Import an animated [Model](https://github.com/MonoGame/MonoGame/blob/master/MonoGame.Framework/Graphics/Model.cs).
* [CPU Animated Model](https://github.com/PokeD/PokeD.Graphics.Animation/blob/master/PokeD.Graphics.Content.Pipeline.Animation/Processors/CPUAnimatedModelProcessor.cs) - Import an animated [Model](https://github.com/MonoGame/MonoGame/blob/master/MonoGame.Framework/Graphics/Model.cs). to be animated by the CPU. Based on [DynamicModelProcessor](https://github.com/PokeD/PokeD.Graphics.Animation/blob/master/PokeD.Graphics.Content.Pipeline.Animation/Processors/DynamicModelProcessor.cs), the imported asset is of type [Microsoft.Xna.Framework.Graphics.Model](https://github.com/MonoGame/MonoGame/blob/master/MonoGame.Framework/Graphics/Model.cs). where the [VertexBuffer](https://github.com/MonoGame/MonoGame/blob/master/MonoGame.Framework/Graphics/Vertices/VertexBuffer.cs) is replaced by a [DefaultAnimatedDynamicVertexBuffer](https://github.com/PokeD/PokeD.Graphics.Animation/blob/master/PokeD.Graphics.Animation/SkeletalAnimation/DefaultAnimatedDynamicVertexBuffer.cs), it inherits from [DynamicVertexBuffer](https://github.com/MonoGame/MonoGame/blob/master/MonoGame.Framework/Graphics/Vertices/DynamicVertexBuffer.cs).
  
  
## Example
-Import 3D model with [GPU Animated Model](https://github.com/PokeD/PokeD.Graphics.Animation/blob/master/PokeD.Graphics.Content.Pipeline.Animation/Processors/GPUAnimatedModelProcessor.cs) or [CPU Animated Model](https://github.com/PokeD/PokeD.Graphics.Animation/blob/master/PokeD.Graphics.Content.Pipeline.Animation/Processors/CPUAnimatedModelProcessor.cs) Processor.  
Use [SkinnedEffect](https://github.com/MonoGame/MonoGame/blob/master/MonoGame.Framework/Graphics/Effect/SkinnedEffect.cs) for GPU and [BasicEffect](https://github.com/MonoGame/MonoGame/blob/master/MonoGame.Framework/Graphics/Effect/BasicEffect.cs) or [AlphaTestEffect](https://github.com/MonoGame/MonoGame/blob/master/MonoGame.Framework/Graphics/Effect/AlphaTestEffect.cs) for CPU based animation.

-Load as any 3D Model:
```csharp
_model = Content.Load<Model>("animatedModel");
```
-Load the Skeletal and Material animation from model:
```csharp
_skeletalAnimations = _model.GetSkeletalAnimations();
_skeletalAnimations.SetClip("ClipName");
  
_materialAnimations = _model.GetMaterialAnimations();
_materialAnimations.SetClip("ClipName");
```
```csharp
_skeletalAnimations = _model.GetSkeletalAnimations();
var skeletalClip = _skeletalAnimations.Clips["ClipName"];
_skeletalAnimations.SetClip(skeletalClip);
  
_materialAnimations = _model.GetMaterialAnimations();
var materialClip = _materialAnimations.Clips["ClipName"];
_materialAnimations.SetClip(materialClip);
```
-Update animation on every frame:
```csharp
_skeletalAnimations.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
_materialAnimations.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
```
-Draw GPU Skeletal animation:
```csharp
foreach (ModelMesh mesh in _model.Meshes)
{
  foreach (var meshPart in mesh.MeshParts)
  {
    meshPart.effect.SetBoneTransforms(_animations.AnimationTransforms);
    // set effect parameters, lights, etc          
  }
  mesh.Draw();
}
```
-Draw GPU Material animation:  
MonoGame's shaders do not support it, create one yourself.
-Draw CPU Skeletal and Material animation:
```csharp
foreach (ModelMesh mesh in _model.Meshes)
{
  foreach (var meshPart in mesh.MeshParts)
  {
    meshPart.UpdateVertices(_skeletalAnimations.AnimationTransforms, _materialAnimations.AnimationTransforms);
    // set effect parameters, lights, etc
  }
  mesh.Draw();
}
```
