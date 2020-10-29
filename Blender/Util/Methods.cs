using Blender.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blender.Util
{
    class Methods
    {
        public static double Squared(double number)
        {
            return Math.Pow(number, 2);
        }

        public static void Line (Edge a, Edge b, DMA bitmap, Color color)
        {
            int declive = 1;
            int dx;
            int dy;
            int incNE;
            int incE;
            int d;
            int x;
            int y;
            dx = (int)(b.X - a.X);
            dy = (int)(b.Y - a.Y);
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (a.X > b.X)
                {
                    Line(b, a, bitmap, color);
                    return;
                }
                if (a.Y > b.Y)
                {
                    declive = -1;
                    dy = -dy;
                }
                incE = 2 * dy;
                incNE = 2 * (dy - dx);
                d = incNE;
                y = (int)a.Y;
                for (x = (int)a.X; x <= b.X; ++x)
                {
                    if (x >= 0 && x < bitmap.Width && y >= 0 && y < bitmap.Height)
                        bitmap.SetPixel(x, y, color);
                    if (d < 0)
                        d += incE;
                    else
                    {
                        d += incNE;
                        y += declive;
                    }
                }
            }
            else
            {
                if (a.Y > b.Y)
                {
                    Line(b, a, bitmap, color);
                    return;
                }

                if (a.X > b.X)
                {
                    declive = -1;
                    dx = -dx;
                }

                incE = 2 * dx;
                incNE = 2 * (dx - dy);
                d = incNE;
                x = (int)a.X;
                for (y = (int)a.Y; y <= b.Y; ++y)
                {
                    if (x >= 0 && x < bitmap.Width && y >= 0 && y < bitmap.Height)
                        bitmap.SetPixel(x, y, color);
                    if (d < 0)
                        d += incE;
                    else
                    {
                        d += incNE;
                        x += declive;
                    }
                }
            }
        }
    }
}
