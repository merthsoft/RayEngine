using RayLib.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RayLib
{
    public record RayTexture : IEnumerable<ColorStrip>
    {
        public static RayTexture Empty { get; } = new(0, 0);

        private (int w, int h) size;
        public (int w, int h) Size
        { 
            get => size;
            set
            {
                size = value;
                //ColorStrips.Resize(size.h, new(Enumerable.Repeat(Color.Black, size.w)));
                //foreach (var c in ColorStrips)
                //    c.Resize(size.w);
            }
        }
        public List<ColorStrip> ColorStrips { get; } = new();

        public ColorStrip this[int col]
            => ColorStrips[col];

        public uint this[int x, int y]
        {
            get => ColorStrips[x][y];
            set => ColorStrips[x][y] = value;
        }

        protected RayTexture()
            => Size = (0, 0);

        public RayTexture(int w, int h)
            => Size = (w, h);

        public RayTexture((int w, int h) size, IEnumerable<ColorStrip> strips)
            : this(size.w, size.h)
            => ColorStrips.AddRange(strips);

        private ColorStrip NewStrip()
        {
            var ret = new ColorStrip();
            ColorStrips.Add(ret);
            return ret;
        }

        public void Add(uint color)
        {
            var strip = ColorStrips.LastOrDefault();
            if (strip == null || strip.Count == Size.h)
                strip = NewStrip();
            strip.Add(color);
        }

        public RayTexture Zip(RayTexture other)
            => new(Size, ColorStrips.Zip(other.ColorStrips, (cs1, cs2) => cs1.Zip(cs2)));

        #region IEnumerable<ColorStrip>
        public IEnumerator<ColorStrip> GetEnumerator() => ((IEnumerable<ColorStrip>)ColorStrips).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ColorStrips).GetEnumerator();
        #endregion
    }
}
