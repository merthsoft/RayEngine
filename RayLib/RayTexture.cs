﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RayLib
{
    public record RayTexture : IEnumerable<ColorStrip>
    {
        public static RayTexture Empty { get; } = new();

        public Vector2d Size { get; set; }
        public List<ColorStrip> ColorStrips { get; } = new();

        public ColorStrip this[int col]
            => ColorStrips[col];

        public (int a, int r, int g, int b) this[int x, int y]
            => ColorStrips[x][y];

        public RayTexture()
            => Size = (64, 64);

        public RayTexture(int w, int h)
            => Size = (w, h);

        private ColorStrip NewStrip()
        {
            var ret = new ColorStrip();
            ColorStrips.Add(ret);
            return ret;
        }

        public void Add(int a, int r, int g, int b)
        {
            var strip = ColorStrips.LastOrDefault();
            if (strip == null || strip.Count() == Size.Y)
                strip = NewStrip();
            strip.Add(a, r, g, b);
        }

        #region IEnumerable<ColorStrip>
        public IEnumerator<ColorStrip> GetEnumerator() => ((IEnumerable<ColorStrip>)ColorStrips).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)ColorStrips).GetEnumerator();
        #endregion
    }
}
