using System;

namespace RayLib
{
    public interface IRenderer
    {
        void Draw(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane, Action<IActiveRenderer> action);
    }

    public interface IActiveRenderer
    {
        IActiveRenderer DrawLine(float x1, float y1, float x2, float y2, int a, int r, int g, int b);
        IActiveRenderer PlotPoint(float x, float y, int a, int r, int g, int b);
        IActiveRenderer DrawText(object text, float x, float y, int a, int r, int g, int b);
    }
}
