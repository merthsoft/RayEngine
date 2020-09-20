using System;

namespace RayLib
{
    public interface IRenderer
    {
        void Draw(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane, Action<IActiveRenderer> action);
    }

    public interface IActiveRenderer
    {
        IActiveRenderer DrawHorizonalLine(int x, int y, int w, uint argb);
        IActiveRenderer PlotPoint(int x, int y, uint argb);
        IActiveRenderer DrawText(object text, int x, int y, int a, int r, int g, int b);
    }
}
