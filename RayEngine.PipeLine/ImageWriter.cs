using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using SixLabors.ImageSharp;

namespace RayEngine.Pipeline
{
    public class ImageWriter : ContentTypeWriter<Image>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform) => "RayEngine.Pipeline.ImageReader";
        protected override void Write(ContentWriter output, Image value)
            => value.SaveAsBmp(output.BaseStream);
    }
}
