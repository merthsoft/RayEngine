using Microsoft.Xna.Framework.Content;
using SixLabors.ImageSharp;

namespace RayEngine.Pipeline
{
    public class ImageReader : ContentTypeReader<Image>
    {
        protected override Image Read(ContentReader input, Image existingInstance)
            => existingInstance ?? Image.Load(input.BaseStream);
    }
}
