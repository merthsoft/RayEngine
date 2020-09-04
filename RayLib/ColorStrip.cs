using System.Collections;
using System.Collections.Generic;

namespace RayLib
{
    public class ColorStrip : IEnumerable<(int a, int r, int g, int b)>
    {
        public List<(int a, int r, int g, int b)> Strip { get; set; } = new();

        public int Count => Strip.Count;

        public (int a, int r, int g, int b) this[int index]
            => Strip[index];

        public void Add(int a, int r, int g, int b)
            => Strip.Add((a, r, g, b));

        #region IEnumerable<(int a, int r, int g, int b)>
        public IEnumerator<(int a, int r, int g, int b)> GetEnumerator() => ((IEnumerable<(int a, int r, int g, int b)>)Strip).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Strip).GetEnumerator();
        #endregion
    }
}
