using System.Collections.Generic;
using System.Linq;

namespace RayLib.Extensions
{
    public static class ListExtensions
    {
        public static void Resize<T>(this List<T> list, int size, T @default = default)
        {
            int count = list.Count;

            if (size < count)
            {
                list.RemoveRange(size, count - size);
            }
            else if (size > count)
            {
                if (size > list.Capacity)
                    list.Capacity = size;

                list.AddRange(Enumerable.Repeat(@default, size - count));
            }
        }
    }
}
