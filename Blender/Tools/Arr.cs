using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blender.Tools
{
    class Arr
    {
        private double x;
        private double y;
        private double z;

        public Arr(double x = 0, double y = 0, double z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double X { get => this.x; set => this.x = value; }
        public double Y { get => this.y; set => this.y = value; }
        public double Z { get => this.z; set => this.z = value; }
    }
}
