using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrystallGrow
{
    public static class Factory
    {
        public static double[,] PmaxFromImage(Bitmap image, int width, int height)
        {
            var re = new double[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var c = image.GetPixel(i, j);
                    re[i, j] = 255.0 - (Math.Max(c.R, Math.Max(c.B, c.G)) + Math.Min(c.R, Math.Min(c.B, c.G))) / 2.0;
                }
            }
            return re;
        }
        public static double[,] PmaxFromImage(Bitmap image)
        {
            return PmaxFromImage(image, image.Width, image.Height);
        }

        public static double[,] ScatterPmax(double[,] pmax)
        {
            var re = pmax;
            return re;
            //var re = new double[pmax.GetLength(0), pmax.GetLength(1)];
            int width = pmax.GetLength(0), height = pmax.GetLength(1);
            /*for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    for (int p = Math.Max(0, i - Config.PmaxScatterRange); p < Math.Min(width, i + Config.PmaxScatterRange); p++)
                        for (int q = Math.Max(0, j - Config.PmaxScatterRange); q < Math.Min(height, j + Config.PmaxScatterRange); q++)
                            re[i, j] += Config.PmaxScatter((p - i) * (p - i) + (q - j) * (q - j)) * pmax[i, j];*/

            double sum = 0, avg, max = 0, min = int.MaxValue;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    sum += re[i, j];
                }
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    re[i, j] /= sum;
                    if (re[i, j] > max) max = re[i, j];
                    if (re[i, j] < min) min = re[i, j];
                }
            avg = sum / width * height;


            var pen = new Pen(Color.White, 1);
            var bitmap = new Bitmap(width, height);
            var gra = Graphics.FromImage(bitmap);
            var rec = new Rectangle(0, 0, 1, 1);
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    int val = (int)((re[i, j] - min)/(max - min) * 255.0);
                    pen.Color = Color.FromArgb(255, val, val, val);
                    rec.X = i;
                    rec.Y = j;
                    gra.DrawRectangle(pen, rec);
                }
            gra.Save();
            gra.Dispose();
            bitmap.Save("dump.bmp");

            return re;
        }



        public static double TideRMax = 2000, TideMinPeer = 0.35, TidePeerCnt = 2;
        private static double tideA, TidePeerLen;
        public static double TideFunc(double r)
        {
            double mr = (r/TideRMax)*tideA;
            var re = Math.Exp(-mr) * Math.Pow(Math.Sin(Math.Pow((r / TideRMax) * TidePeerLen, 0.3)), 2);
            //re += 0.4;
            return Math.Max(0, re);
        }
        public static void InitFide()
        {
            tideA = -Math.Log(TideMinPeer);
            TidePeerLen = Math.Pow(TidePeerCnt*2*Math.PI, 1/0.25);
        }
        public static double[,] GenerateTide(int width, int height)
        {
            InitFide();
            TideRMax = Math.Min(width, height) / 2 - 100;
            var re = new double[width, height];
            double midx = width * 0.6, midy = height * 0.8;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var r = Math.Sqrt((i - midx)*(i - midx) + (j - midy)*(j - midy));
                    //if (r >= TideRMax) continue;
                    re[i, j] = TideFunc(r);
                }
            }


            double sum = 0, avg, max = 0, min = int.MaxValue;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    sum += re[i, j];
                }
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    re[i, j] /= sum;
                    if (re[i, j] > max) max = re[i, j];
                    if (re[i, j] < min) min = re[i, j];
                }
            avg = sum / width * height;


            var pen = new Pen(Color.White, 1);
            var bitmap = new Bitmap(width, height);
            var gra = Graphics.FromImage(bitmap);
            var rec = new Rectangle(0, 0, 1, 1);
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    int val = (int)((re[i, j] - min) / (max - min) * 255.0);
                    pen.Color = Color.FromArgb(255, val, val, val);
                    rec.X = i;
                    rec.Y = j;
                    gra.DrawRectangle(pen, rec);
                }
            gra.Save();
            gra.Dispose();
            bitmap.Save("dump.bmp");


            return re;
        }

    }
}
