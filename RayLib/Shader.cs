namespace RayLib
{
    public delegate TColor Shader<TColor>(int x, int y, double distance, double viewAngleDegrees, TColor color);
}
