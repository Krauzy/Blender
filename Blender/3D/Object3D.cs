using Blender.Tools;
using Blender.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blender._3D
{
    class Object3D
    {
        public const int XY = 0;
        public const int XZ = 1;
        public const int YZ = 2;
        public const int X_AXIS = 0;
        public const int Y_AXIS = 1;
        public const int Z_AXIS = 2;
        //
        private List<Edge> edges = new List<Edge>();
        private List<Edge> current = new List<Edge>();
        private List<Face> faces = new List<Face>();
        private List<Arr> normal = new List<Arr>();
        private Arr light = new Arr(10.0, 10.0, 10.0);
        private double[,] buffer;
        private double[,] MA = new double[4, 4];
        public int dx = 286;
        public int dy = 296;

        public Object3D (List<Edge> edges, List<Face> faces)
        {
            this.faces = faces;
            this.edges = edges;
            this.MA = this.NewMatrix();
            this.Att();
        }

        private void Buffer (int width, int height)
        {
            try
            {
                this.buffer = new double[width, height];
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        buffer[i, j] = int.MinValue;
            }
            catch(OutOfMemoryException ex)
            {
                Console.Write(ex.StackTrace);
            }
        }

        private void Normalizing()
        {
            foreach (Face f in this.faces)
                f.Resolve(this.current);
            Edge e = new Edge();
            for (int i = 0; i < this.current.Count; i++)
            {
                e.X = e.Y = e.Z = 0;
                foreach (Face f in this.faces)
                    if (f.Contains(i))
                    {
                        e.X += f.Normal.X;
                        e.Y += f.Normal.Y;
                        e.Z += f.Normal.Z;
                    }
                double sum = Math.Sqrt(Methods.Squared(e.X) + Methods.Squared(e.Y) + Methods.Squared(e.Z));
                if (sum.ToString().Equals("NaN") || sum == 0)
                    this.current[i].Normal = new Arr();
                else
                    this.current[i].Normal = new Arr(e.X / sum, e.Y / sum, e.Z / sum);
            }
        }

        private double[,] NewMatrix ()
        {
            double[,] M = new double[4, 4];
            M[0, 0] = M[1, 1] = M[2, 2] = M[3, 3] = 1;
            return M;
        }

        private void Att()
        {
            this.current.Clear();
            for (int i = 0; i < this.edges.Count; i++)
            {
                Edge ex = new Edge();
                Edge e = edges[i];
                ex.X = e.X * MA[0, 0] + e.Y * MA[0, 1] + e.Z * MA[0, 2] + MA[0, 3];
                ex.Y = e.X * MA[1, 0] + e.Y * MA[1, 1] + e.Z * MA[1, 2] + MA[1, 3];
                ex.Z = e.X * MA[2, 0] + e.Y * MA[2, 1] + e.Z * MA[2, 2] + MA[2, 3];
                this.current.Add(ex);
            }
        }

        public void Draw(DMA bitmap, Color color, bool visible, int plane = XY)
        {
            this.Buffer(bitmap.Width, bitmap.Height);
            Edge a, b;
            foreach (Face f in this.faces)
            {
                for (int i = 0; i < f.Count(); i++)
                {
                    switch(plane)
                    {
                        case XY:
                            a = new Edge(this.current[f.Get(i)].X + dx, this.current[f.Get(i)].Y + dy, current[f.Get(i)].Z);
                            b = (i == f.Count() - 1)
                                ? new Edge(this.current[f.Get(0)].X + dx, this.current[f.Get(0)].Y + dy, this.current[f.Get(0)].Z)
                                : new Edge(this.current[f.Get(i + 1)].X + dx, this.current[f.Get(i + 1)].Y + dy, this.current[f.Get(i + 1)].Z);
                            if (visible)
                            {
                                if (f.Resolve(this.current))
                                    Methods.Line(a, b, bitmap, color);
                            }
                            else
                                Methods.Line(a, b, bitmap, color);
                            break;

                        case XZ:
                            a = new Edge(this.current[f.Get(i)].X + dx, this.current[f.Get(i)].Z + dy, current[f.Get(i)].Z);
                            b = (i == f.Count() - 1)
                                ? new Edge(this.current[f.Get(0)].X + dx, this.current[f.Get(0)].Z + dy, this.current[f.Get(0)].Z)
                                : new Edge(this.current[f.Get(i + 1)].X + dx, this.current[f.Get(i + 1)].Z + dy, this.current[f.Get(i + 1)].Z);
                            if (visible)
                            {
                                f.Resolve(this.current);
                                if (f.Normal.Y >= 0)
                                    Methods.Line(a, b, bitmap, color);
                            }
                            else
                                Methods.Line(a, b, bitmap, color);
                            break;

                        case YZ:
                            a = new Edge(this.current[f.Get(i)].Y + dx, this.current[f.Get(i)].Z + dy, current[f.Get(i)].Z);
                            b = (i == f.Count() - 1)
                                ? new Edge(this.current[f.Get(0)].Y + dx, this.current[f.Get(0)].Z + dy, this.current[f.Get(0)].Z)
                                : new Edge(this.current[f.Get(i + 1)].Y + dx, this.current[f.Get(i + 1)].Z + dy, this.current[f.Get(i + 1)].Z);
                            if (visible)
                            {
                                f.Resolve(this.current);
                                if (f.Normal.X >= 0)
                                    Methods.Line(a, b, bitmap, color);
                            }
                            else
                                Methods.Line(a, b, bitmap, color);
                            break;
                    }
                }
            }
        }

        public void Flat (DMA bitmap, Color color)
        {
            this.Buffer(bitmap.Width, bitmap.Height);
            foreach (Face f in this.faces)
            {
                f.Resolve(this.current);
                f.Flat(bitmap, color, this.current, this.buffer, this.light);
            }
        }

        public void Phong (DMA bitmap, Color color)
        {
            this.Normalizing();
            this.Buffer(bitmap.Width, bitmap.Height);
            foreach (Face f in this.faces)
                f.Phong(bitmap, color, this.current, this.buffer, this.light);
        }

        public void Gouraud (DMA bitmap, Color color)
        {
            this.Normalizing();
            this.Buffer(bitmap.Width, bitmap.Height);
            foreach (Face f in this.faces)
                f.Gouraud(bitmap, color, this.current, this.buffer, this.light);
        }

        public void Cabinet (DMA bitmap, Color color, bool visible)
        {
            double alfa = (Math.PI * 63.4) / 180;
            Edge a, b;
            foreach (Face f in this.faces)
            {
                for (int i = 0; i < f.Count(); i++)
                {
                    a = new Edge(this.current[f.Get(i)].X + dx, this.current[f.Get(i)].Y + dy, current[f.Get(i)].Z);
                    b = (i == f.Count() - 1)
                        ? new Edge(this.current[f.Get(0)].X + dx, this.current[f.Get(0)].Y + dy, this.current[f.Get(0)].Z)
                        : new Edge(this.current[f.Get(i + 1)].X + dx, this.current[f.Get(i + 1)].Y + dy, this.current[f.Get(i + 1)].Z);
                    a.X += a.Z * Math.Cos(alfa) * 0.5;
                    a.Y += a.Z * Math.Sin(alfa) * 0.5;
                    b.X += b.Z * Math.Cos(alfa) * 0.5;
                    b.Y += b.Z * Math.Sin(alfa) * 0.5;
                    Methods.Line(a, b, bitmap, color);
                }
            }
        }

        public void Cavaleira (DMA bitmap, Color color, bool visible)
        {
            double alfa = (Math.PI * 45) / 180;
            Edge a, b;
            foreach (Face f in this.faces)
            {
                for (int i = 0; i < f.Count(); i++)
                {
                    a = new Edge(this.current[f.Get(i)].X + dx, this.current[f.Get(i)].Y + dy, current[f.Get(i)].Z);
                    b = (i == f.Count() - 1)
                        ? new Edge(this.current[f.Get(0)].X + dx, this.current[f.Get(0)].Y + dy, this.current[f.Get(0)].Z)
                        : new Edge(this.current[f.Get(i + 1)].X + dx, this.current[f.Get(i + 1)].Y + dy, this.current[f.Get(i + 1)].Z);
                    a.X += a.Z * Math.Cos(alfa);
                    a.Y += a.Z * Math.Sin(alfa);
                    b.X += b.Z * Math.Cos(alfa);
                    b.Y += b.Z * Math.Sin(alfa);
                    Methods.Line(a, b, bitmap, color);
                }
            }
        }

        public void Outlook(DMA bitmap, Color color, int look, bool visible)
        {
            int d = look;
            Edge a, b;
            foreach (Face f in this.faces)
            {
                for (int i = 0; i < f.Count(); i++)
                {
                    a = new Edge(this.current[f.Get(i)].X + dx, this.current[f.Get(i)].Y + dy, current[f.Get(i)].Z);
                    b = (i == f.Count() - 1)
                        ? new Edge(this.current[f.Get(0)].X + dx, this.current[f.Get(0)].Y + dy, this.current[f.Get(0)].Z)
                        : new Edge(this.current[f.Get(i + 1)].X + dx, this.current[f.Get(i + 1)].Y + dy, this.current[f.Get(i + 1)].Z);
                    a.X = a.X * d / (a.Z += d);
                    a.Y = a.Y * d / a.Z;
                    b.X = b.X * d / (b.Z += d);
                    b.Y = b.Y * d / b.Z;
                    a.X += dx;
                    a.Y += dy;
                    b.X += dx;
                    b.Y += dy;
                    Methods.Line(a, b, bitmap, color);
                }
            }
        }

        private void Multiply (double[,] M)
        {
            double sum;
            double[,] temp = NewMatrix();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    sum = 0;
                    for (int k = 0; k < 4; k++)
                        sum += M[i, k] * this.MA[k, j];
                    temp[i, j] = sum;
                }
            }
            this.MA = temp;
        }

        public void Transalation (double Tx, double Ty, double Tz)
        {
            double[,] M = NewMatrix();
            M[0, 3] = Tx;
            M[1, 3] = Ty;
            M[2, 3] = Tz;
            this.Multiply(M);
            this.Att();
        }

        public void Rotation (double R, int axis = X_AXIS)
        {
            R *= Math.PI / 180;
            double[,] M = NewMatrix();
            switch(axis)
            {
                case X_AXIS:
                    M[1, 1] = Math.Cos(R);
                    M[1, 2] = -Math.Sin(R);
                    M[2, 1] = Math.Sin(R);
                    M[2, 2] = Math.Cos(R);
                    break;

                case Y_AXIS:
                    M[0, 0] = Math.Cos(R);
                    M[2, 0] = -Math.Sin(R);
                    M[0, 2] = Math.Sin(R);
                    M[2, 2] = Math.Cos(R);
                    break;

                case Z_AXIS:
                    M[0, 0] = Math.Cos(R);
                    M[0, 1] = -Math.Sin(R);
                    M[1, 0] = Math.Sin(R);
                    M[1, 1] = Math.Cos(R);
                    break;
            }
            double mx = 0;
            double my = 0;
            double mz = 0;
            for (int i = 0; i < this.current.Count; i++)
            {
                mx += this.current[i].X;
                my += this.current[i].Y;
                mz += this.current[i].Z;
            }
            mx /= this.current.Count;
            my /= this.current.Count;
            mz /= this.current.Count;

            this.Transalation(-mx, -my, 0);
            this.Multiply(M);
            this.Transalation(mx, my, 0);
            this.Att();
        }

        public void Light(int x, int y, int z)
        {
            light.X = x - this.dx;
            light.Y = y - this.dy;
            light.Z += z;
        }

        public void Scala (double Sx, double Sy, double Sz)
        {
            double[,] M = NewMatrix();
            M[0, 0] = Sx;
            M[1, 1] = Sy;
            M[2, 2] = Sz;
            double mx = 0;
            double my = 0;
            double mz = 0;

            for (int i = 0; i < this.current.Count; i++)
            {
                mx += this.current[i].X;
                my += this.current[i].Y;
                mz += this.current[i].Z;
            }
            mx /= this.current.Count;
            my /= this.current.Count;
            mz /= this.current.Count;

            this.Transalation(-mx, -my, 0);
            this.Multiply(M);
            this.Transalation(mx, my, 0);
            this.Att();
        }
    }
}
