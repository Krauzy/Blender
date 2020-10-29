using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Blender.Tools
{
    class DMA : IDisposable
    {
        private Bitmap bitmap;
        private int[] bits;
        private bool disposed;
        private int width;
        private int height;
        private GCHandle handle;

        public DMA(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.bits = new int[width * height];
            this.handle = GCHandle.Alloc(this.bits, GCHandleType.Pinned);
            this.bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, this.handle.AddrOfPinnedObject());
        }

        public Bitmap Bitmap { get => this.bitmap; set => this.bitmap = value; }
        public int[] Bits { get => this.bits; set => this.bits = value; }
        public bool Disposed { get => this.disposed; set => this.disposed = value; }
        public int Width { get => this.width; set => this.width = value; }
        public int Height { get => this.height; set => this.height = value; }
        public GCHandle Handle { get => this.handle; set => this.handle = value; }

        public void SetPixel (int x, int y, Color color)
        {
            int index = x + y * width;
            int handle_color = color.ToArgb();
            this.bits[index] = handle_color;
        }

        public Color GetPixel (int x, int y)
        {
            int index = x + y * width;
            int handle_color = this.bits[index];
            return Color.FromArgb(handle_color);
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                this.bitmap.Dispose();
                this.handle.Free();
            }
        }
    }
}
