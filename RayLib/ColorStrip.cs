using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RayLib
{
    public class ColorStrip : IEnumerable<GameColor>
    {
        public List<GameColor> Strip { get; set; }

        public int Count => Strip.Count;

        public GameColor this[int index]
            => Strip[index];

        public ColorStrip()
            => Strip = new List<GameColor>();

        public ColorStrip(IEnumerable<GameColor> strip)
            => Strip = strip.ToList();

        public void Add(int a, int r, int g, int b)
            => Strip.Add(new(a, r, g, b));

        public ColorStrip Zip(ColorStrip strip)
            => new(Strip.Zip(strip, (s1, s2) => s1.IsWhiteTransparent ? s2 : s1));

        #region IEnumerable<GameColor>
        public IEnumerator<GameColor> GetEnumerator() => ((IEnumerable<GameColor>)Strip).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Strip).GetEnumerator();
        #endregion
    }
}
