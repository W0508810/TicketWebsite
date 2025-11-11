using ScreenSaverSim;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ScreenSaverSim
{
    public class Triangle : Shape
    {
        private float rotationAngle = 0;
        private PointF[] trianglePoints = new PointF[3];

        public Triangle(int x, int y, int width, int height, Color color)
            : base(x, y, width, height, color)
        {
            UpdateTrianglePoints();
        }

        private void UpdateTrianglePoints()
        {
            trianglePoints[0] = new PointF(x + width / 2, y);
            trianglePoints[1] = new PointF(x, y + height);
            trianglePoints[2] = new PointF(x + width, y + height);
        }

        public override void Draw(Graphics g)
        {
            using (Brush brush = new SolidBrush(colour))
            {
                // Visual state change 3: rotation
                Matrix originalTransform = g.Transform;
                g.TranslateTransform(x + width / 2, y + height / 2);
                g.RotateTransform(rotationAngle);
                g.TranslateTransform(-(x + width / 2), -(y + height / 2));

                g.FillPolygon(brush, trianglePoints);
                g.Transform = originalTransform;
            }
        }

        public override void Update()
        {
            Move();
            UpdateTrianglePoints();
            rotationAngle += 2f; // Rotate 2 degrees per frame
            if (rotationAngle >= 360) rotationAngle = 0;
        }
    }
}