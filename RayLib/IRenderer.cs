using System;

namespace RayLib
{
    public interface IRenderer<TActiveRenderer> where TActiveRenderer : IActiveRenderer
    {
        void Draw(Action<TActiveRenderer> action);
    }

    public interface IActiveRenderer
    {
        IActiveRenderer PlotPoint(int x, int y, uint argb);
    }
}
