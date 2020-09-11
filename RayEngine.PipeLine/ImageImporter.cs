using Microsoft.Xna.Framework.Content.Pipeline;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RayEngine
{
    [ContentImporter(".bmp", ".gif", ".png", ".jpg",
        DefaultProcessor = "RayTextureProcessor", DisplayName = "Image Importer - RayEngine")]
    public class ImageImporter : ContentImporter<Image>
    {
        public override Image Import(string filename, ContentImporterContext context)
            => Image.Load<Rgba32>(filename);
    }
}
