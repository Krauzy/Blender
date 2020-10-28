using Blender.Properties;
using Blender.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blender
{
    public partial class View : Form
    {
        private string tabflag;
        private bool move;
        private Point p;

        public View()
        {
            InitializeComponent();
            this.Icon = Resources.favico;
            this.tabflag = "Info";
            this.move = false;
            btProj_Click(btProjAram, new EventArgs());
            btTab_Click(btInfo, new EventArgs());
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btMinimized_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btInflate_Click(object sender, EventArgs e)
        {
            innerProj.Visible = !innerProj.Visible;
            if (innerProj.Visible)
            {
                btInflate.Image = Resources.inflate_arrow;
                outerProj.Height += innerProj.Height;
                outerMain.Location = new Point(outerMain.Location.X, outerMain.Location.Y + innerProj.Height);
                btInfo.Location = new Point(btInfo.Location.X, btInfo.Location.Y + innerProj.Height);
                btConfig.Location = new Point(btConfig.Location.X, btConfig.Location.Y + innerProj.Height);
                btSolid.Location = new Point(btSolid.Location.X, btSolid.Location.Y + innerProj.Height);
                outerMain.Height -= innerProj.Height;
                innerInfo.Height -= innerProj.Height;
                innerConfig.Height -= innerProj.Height;
            }
            else
            {
                btInflate.Image = Resources.non_inflate_arrow;
                outerProj.Height -= innerProj.Height;
                outerMain.Location = new Point(outerMain.Location.X, outerMain.Location.Y - innerProj.Height);
                btInfo.Location = new Point(btInfo.Location.X, btInfo.Location.Y - innerProj.Height);
                btConfig.Location = new Point(btConfig.Location.X, btConfig.Location.Y - innerProj.Height);
                btSolid.Location = new Point(btSolid.Location.X, btSolid.Location.Y - innerProj.Height);
                outerMain.Height += innerProj.Height;
                innerInfo.Height += innerProj.Height;
                innerConfig.Height += innerProj.Height;
            }
        }

        private void changeTab()
        {
            switch(tabflag)
            {
                case "Info":
                    btConfig.BackColor = Pallete.LIGHT_DARK;
                    btSolid.BackColor = Pallete.LIGHT_DARK;
                    innerInfo.BringToFront();
                    break;

                case "Config":
                    btInfo.BackColor = Pallete.LIGHT_DARK;
                    btSolid.BackColor = Pallete.LIGHT_DARK;
                    innerConfig.BringToFront();
                    break;

                case "Solid":
                    btConfig.BackColor = Pallete.LIGHT_DARK;
                    btInfo.BackColor = Pallete.LIGHT_DARK;
                    innerSolid.BringToFront();
                    break;
            }
        }

        private void btTab_Click(object sender, EventArgs e)
        {
            ((Button)sender).BackColor = Pallete.BLUE;
            tabflag = ((Button)sender).Name.Substring(2);
            changeTab();
        }

        private void btProj_Click(object sender, EventArgs e)
        {
            ((Button)sender).Font = new Font(btProjSolid.Font, FontStyle.Bold);
            if (((Button)sender).Name == btProjSolid.Name)
            {
                btProjAram.Font = new Font(btProjAram.Font, FontStyle.Regular);
                picSolid.Image = Resources.OK;
                picAram.Image = null;
            }
            else
            {
                btProjSolid.Font = new Font(btProjAram.Font, FontStyle.Regular);
                picAram.Image = Resources.OK;
                picSolid.Image = null;
            }
        }

        private void picProj_Click(object sender, EventArgs e)
        {
            if (((PictureBox)sender).Name == picAram.Name)
                btProj_Click(btProjAram, new EventArgs());
            else
                btProj_Click(btProjSolid, new EventArgs());
        }

        private void pnMove_MouseDown(object sender, MouseEventArgs e)
        {
            this.move = true;
            p = new Point(e.X, e.Y);
        }

        private void pnMove_MouseMove(object sender, MouseEventArgs e)
        {
            if(this.move)
            {
                this.Location = new Point(this.Location.X + (e.X - p.X), this.Location.Y + (e.Y - p.Y));
                Console.WriteLine(this.Location);
            }
        }

        private void pnMove_MouseUp(object sender, MouseEventArgs e)
        {
            this.move = false;
        }

        private void btOpen_Click(object sender, EventArgs e)
        {
            pic3D.Image = new Bitmap(pic3D.Width, pic3D.Height);
            OpenFileDialog dialog = new OpenFileDialog();
            if(dialog.ShowDialog() == DialogResult.OK)
            {

            }
        }
    }
}
