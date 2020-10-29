using Blender._3D;
using Blender.Properties;
using Blender.Tools;
using Blender.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        private Object3D object3D;
        private DMA bitmap;
        private Point mouse;

        //


        public View()
        {
            InitializeComponent();
            this.Icon = Resources.favico;
            this.tabflag = "Info";
            this.move = false;
            btProj_Click(btProjAram, new EventArgs());
            btTab_Click(btInfo, new EventArgs());
            this.pic3D.MouseWheel += pic3D_scroll;
        }

        private void pic3D_scroll (object sender, MouseEventArgs e)
        {
            if (object3D != null)
            {
                if (e.Delta > 0)
                    object3D.Scala(1.1, 1.1, 1.1);
                else
                    object3D.Scala(0.9, 0.9, 0.9);
                this.LoadImageBox();
            }
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

        private void LoadImageBox()
        {
            if(bitmap != null)
                bitmap.Dispose();
            bitmap = new DMA(pic3D.Width, pic3D.Height);
            object3D.Draw(bitmap, Pallete.BLUE, false, Object3D.XY);
            pic3D.Image = bitmap.Bitmap;
        }

        private void btOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Objeto 3D|*.obj";
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    List<Edge> edges = new List<Edge>();
                    List<Face> faces = new List<Face>();
                    StreamReader reader = new StreamReader(dialog.FileName);
                    string[] split;
                    string row;
                    string[] s;
                    while ((row = reader.ReadLine()) != null)
                    {
                        if (row != string.Empty)
                        {
                            if(row[0] == 'v' && row[1] == ' ')
                            {
                                row = row.Remove(0, 2);
                                row = row.Replace('.', ',');
                                split = row.Split(' ');
                                edges.Add(new Edge(double.Parse(split[0]), double.Parse(split[1]), double.Parse(split[2])));
                            }
                            else if (row[0] == 'f')
                            {
                                row = row.Remove(0, 2);
                                split = row.Split(' ');
                                List<int> indice = new List<int>();
                                for (int i = 0; i < split.Length; i++)
                                {
                                    split[i] = split[i].Substring(0, split[i].IndexOf('/'));
                                    indice.Add(int.Parse(split[i]) - 1);
                                }
                                faces.Add(new Face(indice));
                            }
                        }
                    }
                    object3D = new Object3D(edges, faces);
                    this.LoadImageBox();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message + "\n\nDetails: " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    object3D = null;
                }
            }
        }

        private void pic3D_MouseMove(object sender, MouseEventArgs e)
        {
            if (object3D != null)
            {
                switch(e.Button)
                {
                    case MouseButtons.Left:
                        if (e.X != mouse.X || e.Y != mouse.Y)
                        {
                            object3D.Transalation(e.X - mouse.X, e.Y - mouse.Y, 0);
                            this.LoadImageBox();
                            pic3D.Refresh();
                            this.mouse = e.Location;
                        }
                        break;

                    case MouseButtons.Right:
                        if (e.X != mouse.X || e.Y != mouse.Y)
                        {
                            object3D.Rotation(e.Y - mouse.Y, Object3D.X_AXIS);
                            object3D.Rotation(e.X - mouse.X, Object3D.Y_AXIS);
                            this.LoadImageBox();
                            pic3D.Refresh();
                            this.mouse = e.Location;
                        }
                        break;
                }
            }
        }

        private void pic3D_MouseDown(object sender, MouseEventArgs e)
        {
            this.mouse = new Point(e.X, e.Y);
        }
    }
}
