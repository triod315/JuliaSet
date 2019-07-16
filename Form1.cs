
using Microsoft.Build.Utilities;
using NGenerics.DataStructures.Mathematical;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JuliaSet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            timer1.Interval = 2;
        }
      
        private void Button1_Click(object sender, EventArgs e)
        {
            //pictureBox1.Image = DrawJuliaSet(new ComplexNumber(0.1, 0.1), 400, 400, 15,0.1,0.1,100,100);

            int zoom = trackBar1.Value / 10;
            int maxiter = trackBar2.Value;
            pictureBox1.Image = drawJuliaSet(zoom, maxiter);
            //bitmap.Save("julia-set.png");
        }

        Color[] gentrateGradient(Color beginColor,Color endColor,int size)
        {
            int rMax = endColor.R;
            int rMin = beginColor.R;
            // ... and for B, G

            int gMax = endColor.G;
            int gMin = beginColor.G;

            int bMax = endColor.B;
            int bMin = beginColor.B;

            var colorList = new List<Color>();
            for (int i = 0; i < size; i++)
            {
                var rAverage = rMin + (int)((rMax - rMin) * i / size);
                var gAverage = gMin + (int)((gMax - gMin) * i / size);
                var bAverage = bMin + (int)((bMax - bMin) * i / size);
                colorList.Add(Color.FromArgb(rAverage, gAverage, bAverage));
            }
            return colorList.ToArray();
        }

        private Bitmap drawJuliaSet(int zoom, int maxiter, int w = 929, int h = 572)
        {
            toolStripStatusLabel1.Text = $"Scale: {zoom}";
            toolStripStatusLabel2.Text = $"Max iteration: {maxiter}";            
            
            int moveX = 0;
            int moveY = 0;
            double cX = double.Parse(textBox6.Text);
            double cY = double.Parse(textBox7.Text);
            double zx, zy, tmp;
            int i;

             Color[] colors = (from c in Enumerable.Range(0, maxiter+1)
                          select Color.FromArgb((c >> 5) * 36, (c >> 3 & 7) * 36, (c & 3) * 85)).ToArray();

            //Color[] colors = gentrateGradient(Color.White, Color.Blue, maxiter +1);

            var bitmap = new Bitmap(w, h);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    zx = 1.5 * (x - w / 2) / (0.5 * zoom * w) + moveX;
                    zy = 1.0 * (y - h / 2) / (0.5 * zoom * h) + moveY;
                    //i = maxiter;
                    i = 0;
                    while (zx * zx + zy * zy < 4 && /*i > 1*/i<maxiter)
                    {
                        tmp = zx * zx - zy * zy + cX;
                        zy = 2.0 * zx * zy + cY;
                        zx = tmp;
                        i += 1;
                    }
                    //TODO: change coloe index
                    bitmap.SetPixel(x, y, colors[i] /*Color.FromArgb(255, (i * 9) % 255, 0, (i * 9) % 255)*/);
                    //if (i == maxiter) bitmap.SetPixel(x, y, Color.Black);
                }
            }
            //pictureBox1.Image = bitmap;

            RectangleF rectf = new RectangleF(10, 10, 200, 100);

            Graphics g = Graphics.FromImage(bitmap);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString($"c={cX}+i{cY};\nwidth={w} height={h}; \nmax iter={maxiter};\nscale={zoom}", new Font("Tahoma", 10), Brushes.Red, rectf);

            g.Flush();

            return bitmap;
        }

        int curscale;
        int curmaxiter;

        void scale_timer_Tick(object sender, EventArgs args)
        {
            curscale += 1;
            pictureBox1.Image = drawJuliaSet(curscale, curmaxiter);
            int maxScale = int.Parse(textBox2.Text);
            if (curscale >= maxScale) timer1.Stop();

            toolStripProgressBar1.Value = curscale;
        }

        void iter_timet_Tick(object sender, EventArgs args)
        {
            curmaxiter += 1;
            pictureBox1.Image = drawJuliaSet(curscale, curmaxiter);
            int maxIter = int.Parse(textBox3.Text);
            if (curmaxiter >= maxIter) timer1.Stop();

            toolStripProgressBar1.Value = curmaxiter;
        }

        private void Button2_Click(object sender, EventArgs e)
        {

            timer1.Interval = int.Parse(textBox5.Text);

            int defZoom = trackBar1.Value/10;
            int defMaxIteration = trackBar2.Value;
            if (radioButton1.Checked)
            {
                
                int minScale = int.Parse(textBox1.Text);
                int maxScale = int.Parse(textBox2.Text);
                toolStripProgressBar1.Minimum = minScale;
                toolStripProgressBar1.Maximum = maxScale;

                curscale = minScale;
                curmaxiter = defMaxIteration;
                timer1.Tick -= iter_timet_Tick;
                timer1.Tick += scale_timer_Tick;
                timer1.Start();
            }
            else
            {
                int minIter = int.Parse(textBox4.Text);
                int maxIter = int.Parse(textBox3.Text);
                toolStripProgressBar1.Minimum = minIter;
                toolStripProgressBar1.Maximum = maxIter;
                curmaxiter = minIter;
                curscale = defZoom;
                timer1.Tick -= scale_timer_Tick;
                timer1.Tick += iter_timet_Tick;
                timer1.Start();

            }
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "*.png|*.png";
            if (saveFileDialog1.ShowDialog()==DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }
        }

        private void StatusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "*.png|*.png";
            int zoom = trackBar1.Value / 10;
            int maxiter = trackBar2.Value;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap res = drawJuliaSet(zoom, maxiter,int.Parse(textBox8.Text),int.Parse(textBox9.Text));
                res.Save(saveFileDialog1.FileName);
            }
        }
    }
}
