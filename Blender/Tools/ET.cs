using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blender.Tools
{
    class ET
    {
        private double yMax;
        private double yMin;
        private double xMin;
        private double xInc;
        private double rMin;
        private double gMin;
        private double bMin;
        private double zMin;
        private double rInc;
        private double gInc;
        private double bInc;
        private double zInc;

        public ET(double yMax = 0, double yMin = 0, double xMin = 0, double xInc = 0, double rMin = 0, double gMin = 0, double bMin = 0, double zMin = 0, double rInc = 0, double gInc = 0, double bInc = 0, double zInc = 0)
        {
            this.yMax = yMax;
            this.yMin = yMin;
            this.xMin = xMin;
            this.xInc = xInc;
            this.rMin = rMin;
            this.gMin = gMin;
            this.bMin = bMin;
            this.zMin = zMin;
            this.rInc = rInc;
            this.gInc = gInc;
            this.bInc = bInc;
            this.zInc = zInc;
        }

        public double YMax { get => this.yMax; set => this.yMax = value; }
        public double YMin { get => this.yMin; set => this.yMin = value; }
        public double XMin { get => this.xMin; set => this.xMin = value; }
        public double XInc { get => this.xInc; set => this.xInc = value; }
        public double RMin { get => this.rMin; set => this.rMin = value; }
        public double GMin { get => this.gMin; set => this.gMin = value; }
        public double BMin { get => this.bMin; set => this.bMin = value; }
        public double ZMin { get => this.zMin; set => this.zMin = value; }
        public double RInc { get => this.rInc; set => this.rInc = value; }
        public double GInc { get => this.gInc; set => this.gInc = value; }
        public double BInc { get => this.bInc; set => this.bInc = value; }
        public double ZInc { get => this.zInc; set => this.zInc = value; }

        public void Att()
        {
            this.xMin += xInc;
            this.zMin += zInc;
            this.rMin += rInc;
            this.gMin += gInc;
            this.bMin += bInc;
        }
    }

}
