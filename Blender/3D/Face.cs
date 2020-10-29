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
    class Face
    {
        private List<int> faces;
        private Arr look = new Arr(0, 0, 1);
        private Arr normal;

        public Face (List<int> faces)
        {
            this.faces = faces;
        }

        public Arr Normal 
        { 
            get => this.normal; 
            set => this.normal = value; 
        }

        public int Get (int index)
        {
            return faces[index];
        }

        public int Count ()
        {
            return faces.Count;
        }

        public bool Contains (int item)
        {
            return faces.Contains(item);
        }

        public bool Resolve (List<Edge> edges)
        {
            Arr AB = new Arr(
                edges[this.Get(1)].X - edges[this.Get(0)].X,
                edges[this.Get(1)].Y - edges[this.Get(0)].Y,
                edges[this.Get(1)].Z - edges[this.Get(0)].Z);
            Arr AC = new Arr(
                edges[this.Get(2)].X - edges[this.Get(0)].X,
                edges[this.Get(2)].Y - edges[this.Get(0)].Y,
                edges[this.Get(2)].Z - edges[this.Get(0)].Z);
            Arr normal = new Arr();
            normal.X = AB.Y * AC.Z - AB.Z * AC.Y;
            normal.Y = AB.Z * AC.X - AB.X * AC.Z;
            normal.Z = AB.X * AC.Y - AB.Y * AC.X;
            double mod = Math.Sqrt(Methods.Squared(normal.X) + Methods.Squared(normal.Y) + Methods.Squared(normal.Z));            
            if (mod == 0 || mod.ToString().Equals("NaN"))
                normal.X = normal.Y = normal.Z = 0;
            else
            {
                normal.X /= mod;
                normal.Y /= mod;
                normal.Z /= mod;
            }
            if (normal.X.ToString().Equals("NaN") || normal.Y.ToString().Equals("NaN") || normal.Z.ToString().Equals("NaN"))
                normal.X = 1;
            this.normal = new Arr(normal.X, normal.Y, normal.Z);
            return (normal.Z >= 0);
        }

        public Edge Max (List<Edge> edges)
        {
            Edge temp = new Edge(int.MinValue, int.MinValue, int.MinValue);
            foreach(Edge ed in edges)
            {
                if (temp.X < ed.X)
                    temp.X = ed.X;
                if (temp.Y < ed.Y)
                    temp.Y = ed.Y;
                if (temp.Z < ed.Z)
                    temp.Z = ed.Z;
            }
            return temp;
        }

        public Edge Min (List<Edge> edges) 
        {
            Edge temp = new Edge(int.MaxValue, int.MaxValue, int.MaxValue);
            for (int i = 0; i < faces.Count(); i++)
            {
                if (edges[this.Get(i)].Y < temp.Y)
                    temp.Y = edges[this.Get(i)].Y;
            }
            return temp;
        }

        public void Flat (DMA bitmap, Color color, List<Edge> edges, double[,] buffer, Arr light)       // Algoritmo Flat
        {
            if (this.Resolve(edges))
            {
                // variáveis
                int cx = 425;
                int cy = 375;
                Edge max = this.Max(edges),
                min = this.Min(edges),
                h = new Edge(),
                la = new Edge(0.1, 0.1, 0.1),
                ld = new Edge(0.5, 0.5, 0.5),
                le = new Edge(0.5, 0.5, 0.5),
                ka = new Edge(1, 0.9, 0.9),
                kd = new Edge(color.R / 255, color.G / 255, color.B / 255),
                ke = new Edge(0.5, 0.5, 0.5);
                int exp = 10;
                double dif, esp;
                int values = bitmap.Height;
                List<ET>[] ET = new List<ET>[values];
                int i;

                //
                for (i = 0; i < this.faces.Count; i++)
                {
                    Edge ma;
                    Edge mi;
                    if (i == this.faces.Count() - 1)
                    {
                        if (edges[this.Get(i)].Y >= edges[this.Get(0)].Y)
                        {
                            ma = edges[this.Get(i)];
                            mi = edges[this.Get(0)];
                        }
                        else
                        {
                            ma = edges[this.Get(0)];
                            mi = edges[this.Get(i)];
                        }
                    }
                    else
                    {
                        if (edges[this.Get(i)].Y >= edges[this.Get(i + 1)].Y)
                        {
                            ma = edges[this.Get(i)];
                            mi = edges[this.Get(i + 1)];
                        }
                        else
                        {
                            ma = edges[this.Get(i + 1)];
                            mi = edges[this.Get(i)];
                        }
                    }
                    int index = (int)mi.Y + cy;
                    if (index < 0)
                        index = 0;
                    else if (index >= bitmap.Height)
                        index = bitmap.Height - 1;

                    if (ET[index] == null)
                        ET[index] = new List<ET>();
                    double dx = ma.X - mi.X;
                    double dy = ma.Y - mi.Y;
                    double dz = ma.Z - mi.Z;
                    double incx = dy == 0 ? 0 : dx / dy;
                    ET[index].Add(new ET((int)ma.Y + cy, (int)mi.Y + cy, (int)mi.X + cx,
                        incx, 1, 1, 1, mi.Z, 1, 1, 1, dy == 0 ? 0 : dz / dy));
                }
                double mod = Math.Sqrt(Methods.Squared(light.X) + Methods.Squared(light.Y) + Methods.Squared(light.Z));
                light.Z /= mod;
                light.Y /= mod;
                light.Z /= mod;
                h.X = light.X + look.X;
                h.Y = light.Y + look.Y;
                h.Z = light.Z + look.Z;
                mod = Math.Sqrt(Methods.Squared(h.X) + Methods.Squared(h.Y) + Methods.Squared(h.Z));
                h.X /= mod;
                h.Y /= mod;
                h.Z /= mod;
                dif = light.X * this.normal.X + light.Y * this.normal.Y + light.Z * this.normal.Z;
                esp = Math.Pow(h.X * this.normal.X + h.Y * this.normal.Y + h.Z * this.normal.Z, exp);
                color = Color.FromArgb(
                    (int)(Math.Abs(la.X * ka.X + ld.X * kd.X * dif + le.X * ke.X * esp) * 255),
                    (int)(Math.Abs(la.Y * ka.Y + ld.Y * kd.Y * dif + le.Y * ke.Y * esp) * 255),
                    (int)(Math.Abs(la.Z * ka.Z + ld.Z * kd.Z * dif + le.Z * ke.Z * esp) * 255)
                );
                List<ET> AET = new List<ET>();
                i = 0;
                while (ET[i] == null)
                    i++;
                foreach (ET node in ET[i])
                    AET.Add(node);
                do
                {
                    for (int j = AET.Count - 1; j >= 0; j--)
                        if (AET[j].YMax <= i)
                            AET.RemoveAt(j);
                    if (AET.Count > 0)
                    {
                        AET.Sort((a, b) => a.XMin.CompareTo(b.XMin));
                        for (int j = 0; j < AET.Count; j += 2)
                        {
                            int x1 = (int)AET[j].XMin;
                            int x2 = (int)AET[j + 1].XMin;
                            int y = i;
                            double dx = x2 - x1;
                            double incZ = (AET[j + 1].ZMin - AET[j].ZMin) / dx;
                            double z = AET[j].ZMin;
                            double r = AET[j].RMin;
                            double g = AET[j].GMin;
                            double b = AET[j].BMin;
                            double incR = (AET[j + 1].RMin - AET[j].RMin) / dx;
                            double incG = (AET[j + 1].GMin - AET[j].GMin) / dx;
                            double incB = (AET[j + 1].BMin - AET[j].BMin) / dx;
                            while (x1++ <= x2)
                            {
                                if (y < bitmap.Height && y >= 0 && x1 < bitmap.Width && x1 >= 0 && z > buffer[x1, y])
                                {
                                    dif = light.Z * r + light.Y * g + light.Z * b;
                                    esp = Math.Pow(h.X * r + h.Y * g + h.Z * b, exp);
                                    color = Color.FromArgb(
                                        (int)(Math.Abs(la.X * ka.X + ld.X * kd.X * dif + le.X * ke.X * esp) * 255),
                                        (int)(Math.Abs(la.Y * ka.Y + ld.Y * kd.Y * dif + le.Y * ke.Y * esp) * 255),
                                        (int)(Math.Abs(la.Z * ka.Z + ld.Z * kd.Z * dif + le.Z * ke.Z * esp) * 255)
                                    );
                                    bitmap.SetPixel(x1, y, color);
                                    buffer[x1, y] = z;
                                }
                                z += incZ;
                                r += incR;
                                g += incG;
                                b += incB;
                            }
                        }
                        foreach (ET node in AET)
                            node.Att();
                        i++;
                        if (i < (int)values && ET[i] != null)
                            foreach (ET node in ET[i])
                                AET.Add(node);
                    }
                } while (AET.Count > 0);
            }
        }

        public void Gouraud (DMA bitmap, Color color, List<Edge> edges, double[,] buffer, Arr light)
        {
            if (this.Resolve(edges))
            {
                // variáveis
                int cx = 425;
                int cy = 375;
                List<Color> colors = new List<Color>();
                Edge max = this.Max(edges),
                min = this.Min(edges),
                h = new Edge(),
                la = new Edge(0.1, 0.1, 0.1),
                ld = new Edge(0.5, 0.5, 0.5),
                le = new Edge(0.5, 0.5, 0.5),
                ka = new Edge(1, 0.9, 0.9),
                kd = new Edge(color.R / 255, color.G / 255, color.B / 255),
                ke = new Edge(0.5, 0.5, 0.5);
                int exp = 10;
                double dif, esp;
                int values = bitmap.Height;
                List<ET>[] ET = new List<ET>[values];
                int i;
                double mod = Math.Sqrt(Methods.Squared(light.X) + Methods.Squared(light.Y) + Methods.Squared(light.Z));
                light.Z /= mod;
                light.Y /= mod;
                light.Z /= mod;
                h.X = light.X + look.X;
                h.Y = light.Y + look.Y;
                h.Z = light.Z + look.Z;
                mod = Math.Sqrt(Methods.Squared(h.X) + Methods.Squared(h.Y) + Methods.Squared(h.Z));
                h.X /= mod;
                h.Y /= mod;
                h.Z /= mod;
                for (i = 0; i < this.faces.Count(); i++)
                {
                    dif = light.X * edges[this.Get(i)].Normal.X + light.Y * edges[this.Get(i)].Normal.Y + light.Z * edges[this.Get(i)].Normal.Z;
                    esp = Math.Pow(h.X * edges[this.Get(i)].Normal.X + h.Y * edges[this.Get(i)].Normal.Y + h.Z * edges[this.Get(i)].Normal.Z, exp);
                    colors.Add(
                        Color.FromArgb(
                            (int)(Math.Abs(la.X * ka.X + ld.X * kd.X * dif + le.X * ke.X * esp) * 255),
                            (int)(Math.Abs(la.Y * ka.Y + ld.Y * kd.Y * dif + le.Y * ke.Y * esp) * 255),
                            (int)(Math.Abs(la.Z * ka.Z + ld.Z * kd.Z * dif + le.Z * ke.Z * esp) * 255)));
                }
                int idMax;
                int idMin;
                for (i = 0; i < this.faces.Count(); i++)
                {
                    Edge ma;
                    Edge mi;
                    if (i == this.faces.Count() - 1)
                    {
                        if(edges[this.Get(i)].Y >= edges[this.Get(0)].Y)
                        {
                            ma = edges[this.Get(i)];
                            mi = edges[this.Get(0)];
                            idMin = 0;
                            idMax = i;
                        }
                        else
                        {
                            ma = edges[this.Get(0)];
                            mi = edges[this.Get(i)];
                            idMin = i;
                            idMax = 0;
                        }
                    }
                    else
                    {
                        if (edges[this.Get(i)].Y >= edges[this.Get(i + 1)].Y)
                        {
                            ma = edges[this.Get(i)];
                            mi = edges[this.Get(i + 1)];
                            idMin = i + 1;
                            idMax = 1;
                        }
                        else
                        {
                            ma = edges[this.Get(i + 1)];
                            mi = edges[this.Get(i)];
                            idMin = 1;
                            idMax = i + 1;
                        }
                    }
                    int index = (int)mi.Y + cy;
                    if (index < 0)
                        index = 0;
                    else if (index >= bitmap.Height)
                        index = bitmap.Height - 1;
                    ET[index] = ET[index] == null ? new List<ET>() : ET[index];
                    double dx = ma.X - mi.X;
                    double dy = ma.Y - mi.Y;
                    double dz = ma.Z - mi.Z;
                    double incx = dy == 0 ? 0 : dx / dy;
                    ET[index].Add(
                        new ET(
                            (int)ma.Y + cy,
                            (int)mi.Y + cy,
                            (int)mi.X + cx,
                            incx,
                            colors[idMin].R,
                            colors[idMin].G,
                            colors[idMin].B,
                            mi.Z,
                            (colors[idMax].R - colors[idMin].R) / dy,
                            (colors[idMax].G - colors[idMin].G) / dy,
                            (colors[idMax].B - colors[idMin].B) / dy,
                            dy == 0 ? 0 : dz / dy
                        )
                    );
                }
                List<ET> AET = new List<ET>();
                i = 0;
                while (ET[i] == null)
                    i++;
                foreach (ET node in ET[i])
                    AET.Add(node);
                do
                {
                    for (int j = AET.Count - 1; j >= 0; j--)
                        if (AET[j].YMax <= i)
                            AET.RemoveAt(j);
                    if (AET.Count > 0)
                    {
                        AET.Sort((a, b) => a.XMin.CompareTo(b.XMin));
                        for (int j = 0; j < AET.Count; j += 2)
                        {
                            int x1 = (int)AET[j].XMin;
                            int x2 = (int)AET[j + 1].XMin;
                            int y = 1;
                            double dx = x2 - x1;
                            double incZ = (AET[j + 1].ZMin - AET[j].ZMin) / dx;
                            double z = AET[j].ZMin;
                            double r = AET[j].RMin;
                            double g = AET[j].GMin;
                            double b = AET[j].BMin;
                            double incR = (AET[j + 1].RMin - AET[j].RMin) / dx;
                            double incG = (AET[j + 1].GMin - AET[j].GMin) / dx;
                            double incB = (AET[j + 1].BMin - AET[j].BMin) / dx;
                            while (x1++ <= x2)
                            {
                                if (y < bitmap.Height && y >= 0 && x1 < bitmap.Width && x1 >= 0 && z > buffer[x1, y])
                                {
                                    bitmap.SetPixel(x1, y, Color.FromArgb(
                                        (int)(r > 255 ? 255 : r < 0 ? 0 : r),
                                        (int)(g > 255 ? 255 : g < 0 ? 0 : g),
                                        (int)(b > 255 ? 255 : b < 0 ? 0 : b))
                                    );
                                    buffer[x1, y] = z;
                                }
                                z += incZ;
                                r += incR;
                                g += incG;
                                b += incB;
                            }
                        }
                        foreach (ET node in AET)
                            node.Att();
                        i++;
                        if (i < bitmap.Height && ET[i] != null)
                            foreach (ET node in ET[i])
                                AET.Add(node);
                    }
                } while (AET.Count > 0); 
            }
        }

        public void Phong (DMA bitmap, Color color, List<Edge> edges, double[,] buffer, Arr light)
        {
            if (this.Resolve(edges))
            {
                // variáveis
                int cx = 425;
                int cy = 375;
                Edge max = this.Max(edges),
                min = this.Min(edges),
                h = new Edge(),
                la = new Edge(0.1, 0.1, 0.1),
                ld = new Edge(0.5, 0.5, 0.5),
                le = new Edge(0.5, 0.5, 0.5),
                ka = new Edge(1, 0.9, 0.9),
                kd = new Edge(color.R / 255, color.G / 255, color.B / 255),
                ke = new Edge(0.5, 0.5, 0.5);
                int exp = 10;
                double dif, esp;
                int values = bitmap.Height;
                List<ET>[] ET = new List<ET>[values];
                int i;
                double mod = Math.Sqrt(Methods.Squared(light.X) + Methods.Squared(light.Y) + Methods.Squared(light.Z));
                light.Z /= mod;
                light.Y /= mod;
                light.Z /= mod;
                h.X = light.X + look.X;
                h.Y = light.Y + look.Y;
                h.Z = light.Z + look.Z;
                mod = Math.Sqrt(Methods.Squared(h.X) + Methods.Squared(h.Y) + Methods.Squared(h.Z));
                h.X /= mod;
                h.Y /= mod;
                h.Z /= mod;
                for (i = 0; i < this.faces.Count(); i++)
                {
                    Edge ma;
                    Edge mi;
                    if (i == this.faces.Count() - 1)
                    {
                        if (edges[this.Get(i)].Y >= edges[this.Get(0)].Y)
                        {
                            ma = edges[this.Get(i)];
                            mi = edges[this.Get(0)];
                        }
                        else
                        {
                            ma = edges[this.Get(0)];
                            mi = edges[this.Get(i)];
                        }
                    }
                    else
                    {
                        if (edges[this.Get(i)].Y >= edges[this.Get(i + 1)].Y)
                        {
                            ma = edges[this.Get(i)];
                            mi = edges[this.Get(i + 1)];
                        }
                        else
                        {
                            ma = edges[this.Get(i + 1)];
                            mi = edges[this.Get(i)];
                        }
                    }
                    int index = (int)mi.Y + cy;
                    if (index < 0)
                        index = 0;
                    else if (index >= bitmap.Height)
                        index = bitmap.Height - 1;
                    ET[index] = ET[index] == null ? new List<ET>() : ET[index];
                    double dx = ma.X - mi.X;
                    double dy = ma.Y - mi.Y;
                    double dz = ma.Z - mi.Z;
                    double incx = dy == 0 ? 0 : dx / dy;
                    ET[index].Add(
                        new ET(
                            (int)ma.Y + cy,
                            (int)mi.Y + cy,
                            (int)mi.X + cx,
                            incx,
                            mi.Normal.X,
                            mi.Normal.Y,
                            mi.Normal.Z,
                            mi.Z,
                            (ma.Normal.X - mi.Normal.X) / dy,
                            (ma.Normal.Y - mi.Normal.Y) / dy,
                            (ma.Normal.Z - mi.Normal.Z) / dy,
                            dy == 0 ? 0 : dz / dy
                        )
                    );
                }
                List<ET> AET = new List<ET>();
                i = 0;
                while (ET[i] == null)
                    i++;
                foreach (ET node in ET[i])
                    AET.Add(node);
                do
                {
                    for (int j = AET.Count - 1; j >= 0; j--)
                        if (AET[j].YMax <= i)
                            AET.RemoveAt(j);
                    if (AET.Count > 0)
                    {
                        AET.Sort((a, b) => a.XMin.CompareTo(b.XMin));
                        for (int j = 0; j < AET.Count; j += 2)
                        {
                            int x1 = (int)AET[j].XMin;
                            int x2 = (int)AET[j + 1].XMin;
                            int y = 1;
                            double dx = x2 - x1;
                            double incZ = (AET[j + 1].ZMin - AET[j].ZMin) / dx;
                            double z = AET[j].ZMin;
                            double r = AET[j].RMin;
                            double g = AET[j].GMin;
                            double b = AET[j].BMin;
                            double incR = (AET[j + 1].RMin - AET[j].RMin) / dx;
                            double incG = (AET[j + 1].GMin - AET[j].GMin) / dx;
                            double incB = (AET[j + 1].BMin - AET[j].BMin) / dx;
                            while (x1++ <= x2)
                            {
                                if (y < bitmap.Height && y >= 0 && x1 < bitmap.Width && x1 >= 0 && z > buffer[x1, y])
                                {
                                    dif = light.X * r + light.Y * g + light.Z * b;
                                    esp = Math.Pow(h.X * r + h.Y * g + h.Z * b, exp);
                                    color = Color.FromArgb(
                                        (int)(Math.Abs(la.X * ka.X + ld.X * kd.X * dif + le.X * ke.X * esp) * 255),
                                        (int)(Math.Abs(la.Y * ka.Y + ld.Y * kd.Y * dif + le.Y * ke.Y * esp) * 255),
                                        (int)(Math.Abs(la.Z * ka.Z + ld.Z * kd.Z * dif + le.Z * ke.Z * esp) * 255)
                                    );
                                }
                                z += incZ;
                                r += incR;
                                g += incG;
                                b += incB;
                            }
                        }
                        foreach (ET node in AET)
                            node.Att();
                        i++;
                        if (i < bitmap.Height && ET[i] != null)
                            foreach (ET node in ET[i])
                                AET.Add(node);
                    }
                } while (AET.Count > 0);
            }
        }
    }
}
