using System.Drawing;
using Svg;

namespace LogoGen.Rendering
{
    class GenericBoundable : ISvgBoundable
    {
        public GenericBoundable(RectangleF rect)
        {
            Bounds = rect;
        }
        public GenericBoundable(float x, float y, float width, float height)
        {
            Bounds = new RectangleF(x, y, width, height);
        }

        public PointF Location => Bounds.Location;
        public SizeF Size => Bounds.Size;
        public RectangleF Bounds { get; }
    }
}