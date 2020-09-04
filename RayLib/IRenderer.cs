using System;

namespace RayLib
{
    public interface IRenderer
    {
        void Draw(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane, Action<IActiveRenderer> action);
    }

    public interface IActiveRenderer
    {
        IActiveRenderer DrawFilledCircle(float x, float y, float radius, int r, int g, int b);
        IActiveRenderer DrawFilledRectangle(float x, float y, float width, float height, int r, int g, int b);
        IActiveRenderer DrawLine(float x1, float y1, float x2, float y2, int r, int g, int b);
        IActiveRenderer PlotPoint(float x, float y, int a, int r, int g, int b);
        IActiveRenderer DrawText(object text, float x, float y, int r, int g, int b);
    }
}
