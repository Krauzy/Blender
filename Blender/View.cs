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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blender
{
    public partial class View : Form
    {
        public const int XY = 0;
        public const int XZ = 1;
        public const int YZ = 2;
        public const int CAVALEIRA = 3;
        public const int CABINET = 4;
        public const int OUTLOOK = 5;
        public const int FLAT = 101;
        public const int GOURAUD = 102;
        public const int PHONG = 103;
        //
        private string tabflag;
        private bool move;
        private Point p;
        private Object3D object3D;
        private DMA bitmap;
        private Point mouse;
        private double zoom;
        private double axisX;
        private double axisY;
        private FileInfo info;
        private int opt;
        private bool hidden;
        private Color color;
        private Point plight;
        private bool lightmove;
        private Point checkpoint;
        //
        private bool ambient;
        private bool dif;
        private bool esp;
        private Color ambcolor;
        private Color difcolor;
        private Color espcolor;

        public View()
        {
            InitializeComponent();
            this.Icon = Resources.favico;
            this.tabflag = "Info";
            this.move = false;
            zoom = 1;
            axisX = 0;
            axisY = 0;
            hidden = false;
            btTab_Click(btInfo, new EventArgs());
            opt = XY;
            this.pic3D.MouseWheel += pic3D_scroll;
            color = Color.FromArgb(0, 122, 204);
            ambient = false;
            dif = false;
            esp = false;
            //
            ambcolor = Color.FromArgb(100, 100, 100);
            difcolor = Color.FromArgb(100, 100, 100);
            espcolor = Color.FromArgb(100, 100, 100);
        }

        private void pic3D_scroll (object sender, MouseEventArgs e)
        {
            if (object3D != null)
            {
                if (e.Delta > 0)
                {
                    object3D.Scala(1.1, 1.1, 1.1);
                    zoom += 0.1;
                }
                else
                {
                    object3D.Scala(0.9, 0.9, 0.9);

                    //Console.WriteLine(zoom);
                    zoom += -0.1;
                }
                zoom = Math.Round(zoom, 1, MidpointRounding.AwayFromZero);
                this.LoadImageBox(opt);
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
                //btFlat_Click(btFlat, new EventArgs());
                picAram.Image = null;
            }
            else
            {
                btProjSolid.Font = new Font(btProjAram.Font, FontStyle.Regular);
                picAram.Image = Resources.OK;
                //btXY_Click(btXY, new EventArgs());
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
                //Console.WriteLine(this.Location);
            }
        }

        private void pnMove_MouseUp(object sender, MouseEventArgs e)
        {
            this.move = false;
        }

        private void LoadImageBox(int option, bool hidden = false)
        {
            if(object3D != null)
            {
                if (bitmap != null)
                    bitmap.Dispose();
                txtZoom.Text = zoom.ToString() + "x";
                txtRotationX.Text = axisX + "°";
                txtRotationY.Text = axisY + "°";
                bitmap = new DMA(pic3D.Width, pic3D.Height);
                switch (option)
                {
                    case View.XY:
                        object3D.Draw(bitmap, color, hidden, Object3D.XY);
                        break;

                    case View.XZ:
                        object3D.Draw(bitmap, color, hidden, Object3D.XZ);
                        break;

                    case View.YZ:
                        object3D.Draw(bitmap, color, hidden, Object3D.YZ);
                        break;

                    case View.CAVALEIRA:
                        object3D.Cavaleira(bitmap, color, hidden);
                        break;

                    case View.CABINET:
                        object3D.Cabinet(bitmap, color, hidden);
                        break;

                    case View.OUTLOOK:
                        object3D.Outlook(bitmap, color, -Convert.ToInt32(txtDeep.Text), hidden);
                        break;

                    case View.FLAT:
                        object3D.Flat(bitmap, color, difcolor, ambcolor);
                        break;

                    case View.GOURAUD:
                        object3D.Gouraud(bitmap, color);
                        break;

                    case View.PHONG:
                        object3D.Phong(bitmap, color);
                        break;
                }
                pic3D.Image = bitmap.Bitmap;
            }            
        }

        private string GetSize(long size)
        {
            string[] unit = { "B", "KB", "MB", "GB", "TB" };
            double d = size;
            int i = 0;
            while(d / 1024 > 1)
            {
                i++;
                d = (double)d / 1024;
            }
            d = Math.Round(d, 2);
            return d.ToString() + " " + unit[i];
        }

        private void btOpen_Click(object sender, EventArgs e)
        {
            btProj_Click(btProjAram, new EventArgs());
            btXY_Click(btXY, new EventArgs());
            zoom = 1.0;
            axisX = 0;
            axisY = 0;
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
                    //
                    this.info =  new FileInfo(dialog.FileName);
                    txtFilename.Text = char.ToUpper(info.Name[0]) + info.Name.Substring(1);

                    txtFilesize.Text = this.GetSize(info.Length);
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
                    txtEdge.Text = edges.Count.ToString();
                    txtFaces.Text = faces.Count.ToString();
                    object3D = new Object3D(edges, faces);
                    object3D.Light(xLight.Location.X, xLight.Location.Y, 10);
                    opt = XY;
                    this.LoadImageBox(opt, hidden);
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
                            this.LoadImageBox(opt, hidden);
                            pic3D.Refresh();
                            this.mouse = e.Location;
                        }
                        break;

                    case MouseButtons.Right:
                        if (e.X != mouse.X || e.Y != mouse.Y)
                        {
                            object3D.Rotation(e.Y - mouse.Y, Object3D.X_AXIS);
                            object3D.Rotation(e.X - mouse.X, Object3D.Y_AXIS);
                            axisX += e.Y - mouse.Y;
                            axisY += e.X - mouse.X;
                            this.LoadImageBox(opt, hidden);
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

        private void txtFilename_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.btOpen_Click(btOpen, new EventArgs());
        }

        private void btXY_Click(object sender, EventArgs e)
        {
            btXY.Font = new Font(btXY.Font, FontStyle.Bold);
            picXY.Image = Resources.OK;
            opt = XY;
            this.LoadImageBox(opt, hidden);
            btXZ.Font = new Font(btXY.Font, FontStyle.Regular);
            picXZ.Image = null;
            btYZ.Font = new Font(btXY.Font, FontStyle.Regular);
            picYZ.Image = null;
            btCavaleira.Font = new Font(btXY.Font, FontStyle.Regular);
            picCav.Image = null;
            btCab.Font = new Font(btXY.Font, FontStyle.Regular);
            picCab.Image = null;
            btPersp.Font = new Font(btXY.Font, FontStyle.Regular);
            picPersp.Image = null;
            this.ActiveControl = null;
            btProj_Click(btProjAram, new EventArgs());
        }

        private void btYZ_Click(object sender, EventArgs e)
        {
            btYZ.Font = new Font(btXY.Font, FontStyle.Bold);
            picYZ.Image = Resources.OK;            
            opt = YZ;
            this.LoadImageBox(opt, hidden);
            btXZ.Font = new Font(btXY.Font, FontStyle.Regular);
            picXZ.Image = null;
            btXY.Font = new Font(btXY.Font, FontStyle.Regular);
            picXY.Image = null;
            btCavaleira.Font = new Font(btXY.Font, FontStyle.Regular);
            picCav.Image = null;
            btCab.Font = new Font(btXY.Font, FontStyle.Regular);
            picCab.Image = null;
            btPersp.Font = new Font(btXY.Font, FontStyle.Regular);
            picPersp.Image = null;
            this.ActiveControl = null;
            btProj_Click(btProjAram, new EventArgs());
        }

        private void btXZ_Click(object sender, EventArgs e)
        {
            btXZ.Font = new Font(btXY.Font, FontStyle.Bold);
            picXZ.Image = Resources.OK;
            opt = XZ;
            this.LoadImageBox(opt, hidden);
            btYZ.Font = new Font(btXY.Font, FontStyle.Regular);
            picYZ.Image = null;
            btXY.Font = new Font(btXY.Font, FontStyle.Regular);
            picXY.Image = null;
            btCavaleira.Font = new Font(btXY.Font, FontStyle.Regular);
            picCav.Image = null;
            btCab.Font = new Font(btXY.Font, FontStyle.Regular);
            picCab.Image = null;
            btPersp.Font = new Font(btXY.Font, FontStyle.Regular);
            picPersp.Image = null;
            this.ActiveControl = null;
            btProj_Click(btProjAram, new EventArgs());
        }

        private void btCavaleira_Click(object sender, EventArgs e)
        {
            btCavaleira.Font = new Font(btXY.Font, FontStyle.Bold);
            picCav.Image = Resources.OK;
            opt = CAVALEIRA;
            this.LoadImageBox(opt, hidden);
            btYZ.Font = new Font(btXY.Font, FontStyle.Regular);
            picYZ.Image = null;
            btXY.Font = new Font(btXY.Font, FontStyle.Regular);
            picXY.Image = null;
            btXZ.Font = new Font(btXY.Font, FontStyle.Regular);
            picXZ.Image = null;
            btCab.Font = new Font(btXY.Font, FontStyle.Regular);
            picCab.Image = null;
            btPersp.Font = new Font(btXY.Font, FontStyle.Regular);
            picPersp.Image = null;
            this.ActiveControl = null;
            btProj_Click(btProjAram, new EventArgs());
        }

        private void btCab_Click(object sender, EventArgs e)
        {
            btCab.Font = new Font(btXY.Font, FontStyle.Bold);
            picCab.Image = Resources.OK;
            opt = CABINET;
            this.LoadImageBox(opt, hidden);
            btYZ.Font = new Font(btXY.Font, FontStyle.Regular);
            picYZ.Image = null;
            btXY.Font = new Font(btXY.Font, FontStyle.Regular);
            picXY.Image = null;
            btXZ.Font = new Font(btXY.Font, FontStyle.Regular);
            picXZ.Image = null;
            btCavaleira.Font = new Font(btXY.Font, FontStyle.Regular);
            picCav.Image = null;
            btPersp.Font = new Font(btXY.Font, FontStyle.Regular);
            picPersp.Image = null;
            this.ActiveControl = null;
            btProj_Click(btProjAram, new EventArgs());
        }

        private void btPersp_Click(object sender, EventArgs e)
        {
            btPersp.Font = new Font(btXY.Font, FontStyle.Bold);
            picPersp.Image = Resources.OK;
            opt = OUTLOOK;
            this.LoadImageBox(opt, hidden);
            btYZ.Font = new Font(btXY.Font, FontStyle.Regular);
            picYZ.Image = null;
            btXY.Font = new Font(btXY.Font, FontStyle.Regular);
            picXY.Image = null;
            btXZ.Font = new Font(btXY.Font, FontStyle.Regular);
            picXZ.Image = null;
            btCavaleira.Font = new Font(btXY.Font, FontStyle.Regular);
            picCav.Image = null;
            btCab.Font = new Font(btXY.Font, FontStyle.Regular);
            picCab.Image = null;
            this.ActiveControl = null;
            btProj_Click(btProjAram,new EventArgs());
        }

        private void btPlus_Click(object sender, EventArgs e)
        {
            int deep = Convert.ToInt32(txtDeep.Text);
            deep += 100;
            txtDeep.Text = deep.ToString();
            this.LoadImageBox(opt, hidden);
            this.ActiveControl = null;
        }

        private void btMinus_Click(object sender, EventArgs e)
        {
            int deep = Convert.ToInt32(txtDeep.Text);
            if(deep > 0)
                deep -= 100;
            txtDeep.Text = deep.ToString();
            this.LoadImageBox(opt, hidden);
            this.ActiveControl = null;
        }

        private void btFOcultas_Click(object sender, EventArgs e)
        {
            this.hidden = !hidden;
            if(hidden)
            {
                btFOcultas.Font = new Font(btFOcultas.Font, FontStyle.Bold);
                picFOcultas.Image = Resources.OK;
            }
            else
            {
                btFOcultas.Font = new Font(btFOcultas.Font, FontStyle.Regular);
                picFOcultas.Image = null;
            }
            this.ActiveControl = null;
            this.LoadImageBox(opt, hidden);
        }

        private void btHelp_Click(object sender, EventArgs e)
        {
            Help ex = new Help();
            ex.ShowDialog();
        }

        private void redBar_Scroll(object sender, EventArgs e)
        {
            int value = redBar.Value;
            lbRed.Text = "[" + value + "]";
            Color tempc = Color.FromArgb(redBar.Value, greenBar.Value, blueBar.Value);
            if (ambient)
                ambcolor = tempc;
            if (dif)
                difcolor = tempc;
            if (esp)
                color = tempc;
            if(object3D != null)
                this.LoadImageBox(opt, hidden);
        }

        private void greenBar_Scroll(object sender, EventArgs e)
        {
            int value = greenBar.Value;
            lbGreen.Text = "[" + value + "]";
            Color tempc = Color.FromArgb(redBar.Value, greenBar.Value, blueBar.Value);
            if (ambient)
                ambcolor = tempc;
            if (dif)
                difcolor = tempc;
            if (esp)
                color = tempc;

            if (object3D != null)
                this.LoadImageBox(opt, hidden);
        }

        private void blueBar_Scroll(object sender, EventArgs e)
        {
            int value = blueBar.Value;
            lbBlue.Text = "[" + value + "]";
            Color tempc = Color.FromArgb(redBar.Value, greenBar.Value, blueBar.Value);
            if (ambient)
                ambcolor = tempc;
            if (dif)
                difcolor = tempc;
            if (esp)
                color = tempc;

            if (object3D != null)
                this.LoadImageBox(opt, hidden);
        }

        private void btFlat_Click(object sender, EventArgs e)
        {
            picFlat.Image = Resources.OK;
            picGouraud.Image = null;
            picPhong.Image = null;
            //picDif.Image = Resources.OK;
            //picEspec.Image = Resources.OK;
            btProj_Click(btProjSolid, new EventArgs());
            //
            opt = FLAT;
            LoadImageBox(opt);
        }

        private void btGouraud_Click(object sender, EventArgs e)
        {
            picFlat.Image = null;
            picGouraud.Image = Resources.OK;
            picPhong.Image = null;
            //picDif.Image = Resources.OK;
            //picEspec.Image = Resources.OK;
            btProj_Click(btProjSolid, new EventArgs());
            //
            opt = GOURAUD;
            LoadImageBox(opt);
        }

        private void btPhong_Click(object sender, EventArgs e)
        {
            picFlat.Image = null;
            picGouraud.Image = null;
            picPhong.Image = Resources.OK;
            //picDif.Image = Resources.OK;
            //picEspec.Image = Resources.OK;
            btProj_Click(btProjSolid, new EventArgs());
            //
            opt = PHONG;
            LoadImageBox(opt);
        }

        private void xLight_MouseDown(object sender, MouseEventArgs e)
        {
            lightmove = true;
            plight = new Point(e.X, e.Y);
            checkpoint = new Point(e.X, e.Y);
        }

        private void xLight_MouseUp(object sender, MouseEventArgs e)
        {
            lightmove = false;           
        }

        private void xLight_MouseMove(object sender, MouseEventArgs e)
        {
            int delay = 10;
            if (lightmove)
            {
                int x = e.X + plight.X - 20;
                int y = e.Y + plight.Y - 20;
                if (x > pic3D.Location.X && x < pic3D.Location.X + pic3D.Width - xLight.Width && y > pic3D.Location.Y && x < pic3D.Location.Y + pic3D.Height - xLight.Height)
                {
                    plight = new Point(x, y);
                    xLight.Location = plight;
                    if (checkpoint.X >= plight.X + delay || checkpoint.X <= plight.X - delay || checkpoint.Y >= plight.Y + delay || checkpoint.Y <= plight.Y - delay)
                    {
                        if (object3D != null)
                        {
                            object3D.Light(x, y, 10);
                            LoadImageBox(opt);
                        }
                    }
                }
            } 
        }

        private void btAmb_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            picAmb.Image = Resources.OK;
            picEspec.Image = null;
            picDif.Image = null;
            ambient = true;
            dif = false;
            esp = false;
            redBar.Value = ambcolor.R;
            lbRed.Text = ambcolor.R.ToString();            
            greenBar.Value = ambcolor.G;
            lbGreen.Text = ambcolor.G.ToString();
            blueBar.Value = ambcolor.B;
            lbBlue.Text = ambcolor.B.ToString();
        }

        private void btDif_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            picAmb.Image = null;
            picEspec.Image = null;
            picDif.Image = Resources.OK;
            ambient = false;
            dif = true;
            esp = false;
            redBar.Value = difcolor.R;
            lbRed.Text = difcolor.R.ToString();
            greenBar.Value = difcolor.G;
            lbGreen.Text = difcolor.G.ToString();
            blueBar.Value = difcolor.B;
            lbBlue.Text = difcolor.B.ToString();
        }

        private void btForce_Click(object sender, EventArgs e) // Disabled
        {
            //this.ActiveControl = null;
        }

        private void btEspec_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            picAmb.Image = null;
            picEspec.Image = Resources.OK;
            picDif.Image = null;
            ambient = false;
            dif = true;
            esp = false;
            redBar.Value = color.R;
            lbRed.Text = color.R.ToString();
            greenBar.Value = color.G;
            lbGreen.Text = color.G.ToString();
            blueBar.Value = color.B;
            lbBlue.Text = color.B.ToString();
        }
    }
}
