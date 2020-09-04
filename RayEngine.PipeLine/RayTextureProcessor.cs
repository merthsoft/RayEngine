using Microsoft.Xna.Framework.Content.Pipeline;
using RayLib;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RayEngine
{
    [ContentProcessor(DisplayName = ("RayTexture Processor - RayEngine"))]
    public class RayTextureProcessor : ContentProcessor<Image<Rgba32>, RayTexture>
    {
        public override RayTexture Process(Image<Rgba32> input, ContentProcessorContext context)
        {
            var ret = new RayTexture(input.Width, input.Height);
            for (int x = 0; x < input.Width; x++)
                for (int y = 0; y < input.Height; y++)
                    ret.Add(input[x, y].A, input[x, y].R, input[x, y].G, input[x, y].B);
            return ret;
        }
    }
}
