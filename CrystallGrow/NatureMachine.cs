using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;
using SVGLib;

namespace CrystallGrow
{
    public class Crystall
    {
        public Crystall()
        {
            
        }

        public double X;
        public double Y;
        public double Power;
        public int Life = 0;
    }

    public class CrystallPowerComparer : IComparer<Crystall>
    {
        public int Compare(Crystall x, Crystall y)
        {
            return x.Power.CompareTo(y.Power);
        }
    }

    public class NatureMachine
    {
        public NatureMachine(int width, int height, double[,] pmap)
        {
            Width = width;
            Height = height;
            Pmap = pmap;
            Crystalls = new List<Crystall>();
            each_src_cnt = Config.ExpectEnergySourceNum(Width*Height);
            expect_crys_cnt = Config.ExpectCrystallCount(Width*Height);
            purpose_energy = Config.PurposeEnergy(Width*Height);
            FormatPmap();
        }
        public NatureMachine(double[,] pmap) :this(pmap.GetLength(0), pmap.GetLength(1), pmap)
        {
            
        }

        /// <summary>
        /// 整个地图的长宽。。(但是晶体的位置是连续的)
        /// </summary>
        public readonly int Width, Height;
        /// <summary>
        /// 概率地图
        /// </summary>
        public readonly double[,] Pmap;
        public static Random Rand = new Random();

        public readonly List<Crystall> Crystalls;

        private readonly int purpose_energy;
        private double PlanEnery()
        {
            double ne = 0;
            foreach (var c in Crystalls)
                ne += c.Power - Config.DieEnergy(c.Power);
            return Config.ExpectEnergyRate(purpose_energy - ne);
        }

        private void LootEnergy(double x, double y, double energy)
        {
            Crystall mi = null;
            double maxl = 0;
            foreach (var c in Crystalls)
            {
                double r = Config.PowerToR(c.Power);
                double now = Config.GetLocalPower(c.Power, x - c.X, y - c.Y, r);
                if (now < 0)
                    Debugger.Break();
                if (maxl < now)
                {
                    maxl = now;
                    mi = c;
                }
            }
            if (maxl == 0) Debugger.Break();
            mi.Power += energy;
        }

        private List<Pair<int, int>> Enes = new List<Pair<int, int>>();
        private readonly int each_src_cnt;
        private void EnergyFall()
        {
            Enes.Clear();
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    if (Rand.NextDouble() <= Pmap[i, j] * each_src_cnt)
                        Enes.Add(new Pair<int, int>(i, j));

            double sum = 0;
            for (int i = 0; i < Enes.Count; i++)
                sum += Pmap[Enes[i].Item1, Enes[i].Item2];

            double expect = PlanEnery();
            for (int i = 0; i < Enes.Count; i++)
                LootEnergy(Enes[i].Item1, Enes[i].Item2, expect * Pmap[Enes[i].Item1, Enes[i].Item2] / sum);
        }

        void DieCrys()
        {
            foreach (var crystall in Crystalls)
            {
                crystall.Power -= Config.DieEnergy(crystall.Power);
                crystall.Life ++;
            }
            int cnt = 0, maxlen = 0;
            for (int i = 0; i < Crystalls.Count; i++)
            {
                if (Crystalls[i].Power <= 0)
                {
                    if (Crystalls[i].Life > maxlen)
                        maxlen = Crystalls[i].Life;
                    Crystalls.RemoveAt(i);
                    cnt ++;
                    i--;
                }
            }
            Console.Write("Died : {0}, MaxLen : {1}, ", cnt, maxlen);
        }

        void SeedOne()
        {
            double x = Rand.NextDouble() * Width, y = Rand.NextDouble() * Height;
            Crystalls.Add(new Crystall() { Power = Config.InitPower, X = x, Y = y});
        }

        private int expect_crys_cnt;
        void SeedFall()
        {
            if (Crystalls.Count >= expect_crys_cnt) return;
            int expect = Config.SeedRate(expect_crys_cnt - Crystalls.Count);
            for (int i = 0; i < expect; i++)
                SeedOne();
        }

        private void Crowd()
        {
            int csnum = 0;
            for (int i = 0; i < Crystalls.Count; i++)
            {
                var a = Crystalls[i];
                double dec = 0;
                for (int j = 0; j < Crystalls.Count; j++)
                {
                    if (i == j) continue;
                    var b = Crystalls[j];
                    var t = Config.GetDistance(b.X - a.X, b.Y - a.Y) - Config.PowerToR(a.Power) -
                            Config.PowerToR(b.Power);
                    dec += Config.CrowDec(Config.GetDistance(b.X - a.X, b.Y - a.Y) - Config.PowerToR(a.Power) - Config.PowerToR(b.Power),
                        a.Power);
                }
                a.Power -= dec;
                if (a.Power < 0) csnum += 1;
            }
            Console.Write("CSNUM : {0}, ", csnum);
        }

       /* private static int last_anneal_time = 0;
        public void Annealing()
        {
            if (time - last_anneal_time < Config.MinAnnealInterval) return;
            if (Rand.NextDouble() > Config.AnnealInvokeP(time - last_anneal_time))
                return;
            var cl = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Annealing!!");
            Console.ForegroundColor = cl;
            last_anneal_time = time;
            foreach (var c in Crystalls)
                c.Power -= Config.AnnealDecPower(c.Power);
        }*/

        private static int time = 0;
        public void GameOne(bool seed, bool enable_anneal = true)
        {
            Console.Write("{0} : ", time++);
            if (seed)
                SeedFall();
            EnergyFall();
            //Crowd();
            //if (enable_anneal) Annealing();
            DieCrys();

            double avglife = 0, maxlen = 0, mpow = 0;
            foreach (var c in Crystalls)
            {
                avglife += c.Life;
                if (maxlen < c.Life)
                {
                    maxlen = c.Life;
                    mpow = c.Power;
                }
            }
            avglife /= Crystalls.Count;
            Console.WriteLine("AvgLife : {0}, MaxLen : {1}, {2}, CrysCnt : {3}", avglife, maxlen, mpow, Crystalls.Count);
        }

        private void FormatPmap()
        {
            double sum = 0;
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    sum += Pmap[i, j];
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    Pmap[i, j] /= sum;
        }

        #region Draw

        private static float Root3 = (float)Math.Sqrt(3);
        public void Draw(string adress)
        {
            var writer = new StreamWriter(new FileStream(adress, FileMode.Create, FileAccess.Write), Encoding.UTF8);
            writer.Write("<?xml version=\"1.0\" standalone=\"no\"?>\n<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \n\"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">\n");
            writer.Write("<svg width=\"{0}px\" height=\"{1}px\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\">\n", Width, Height);

            var crys = Crystalls.ToArray();
            Array.Sort(crys, new CrystallPowerComparer());
            var pol_src = new PointF[6]
            {
                new PointF(Root3/2.0f, 0.5f), new PointF(0.0f, 1.0f), new PointF(-Root3/2.0f, 0.5f), 
                new PointF(-Root3/2.0f, -0.5f), new PointF(0.0f, -1.0f), new PointF(Root3/2.0f, -0.5f)
            };
            var cpol = new PointF[6];

            var brush = new SolidBrush(Color.Black);
            var pen = new Pen(Color.White, 2);
            for (int i = crys.Length - 1; i >= 0; --i)
            {
                var c = crys[i];
                var r = Config.PowerToR(c.Power);
                writer.Write("<polygon points=\"");
                for (int j = 0; j < 6; j++)
                {
                    cpol[j].X = (float)c.X + pol_src[j].X*(float)r;
                    cpol[j].Y = (float)c.Y + pol_src[j].Y * (float)r;
                    writer.Write("{0},{1} ", cpol[j].X, cpol[j].Y);
                }
                writer.Write("\" style=\"fill:black;stroke:white;stroke-width:1\"/>\n");
            }
            writer.Write("</svg>\n");
            writer.Dispose();

            /*int enfMetafileHandle = meta.GetHenhmetafile().ToInt32();
            int bufferSize = GetEnhMetaFileBits(enfMetafileHandle, 0, null); // Get required buffer size.  
            byte[] buffer = new byte[bufferSize]; // Allocate sufficient buffer  
            if (GetEnhMetaFileBits(enfMetafileHandle, bufferSize, buffer) <= 0) // Get raw metafile data.  
                throw new SystemException("Fail");  

            var writer = new FileStream(adress, FileMode.Create, FileAccess.Write);
            writer.Write(buffer, 0, bufferSize);
            writer.Close();
            writer.Dispose();
            if (!DeleteEnhMetaFile(enfMetafileHandle)) //free handle  
                throw new SystemException("Fail Free");  */

        }

        [DllImport("gdi32")]
        public static extern int GetEnhMetaFileBits(int hemf, int cbBuffer, byte[] IpbBuffer);
        [DllImport("gdi32")]  
        public static extern bool DeleteEnhMetaFile(int hemfbitHandle);  
        #endregion
    }
}
