using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blender
{
    public partial class Help : Form
    {
        private bool move;
        private Point p;

        public Help()
        {
            move = false;
            InitializeComponent();
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pnMove_MouseDown(object sender, MouseEventArgs e)
        {
            move = true;
            p = new Point(e.X, e.Y);
        }

        private void pnMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (move)
                this.Location = new Point(this.Location.X + (e.X - p.X), this.Location.Y + (e.Y - p.Y));
        }

        private void pnMove_MouseUp(object sender, MouseEventArgs e)
        {
            move = false;
        }
    }
}
