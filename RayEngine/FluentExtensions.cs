using System;

namespace RayEngine
{
    public static class FluentExtensions
    {
        public static T FluentWrap<T>(this T item, Action<T> action)
        {
            action(item);
            return item;
        }
    }
}
