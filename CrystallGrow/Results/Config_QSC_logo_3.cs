using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrystallGrow
{
    public class Config
    {
        private static double Root3 = Math.Sqrt(3);
        public const double DiffuseG = 10;

        public static Func<double, double, double> GetDistance = (x, y) =>
        {
            double r;
            if (Math.Abs(y) < Math.Abs(x) * Root3)
                r = x;
            else if (x * y > 0)
                r = y + x * Root3 / 2.0;
            else r = y - x * Root3 / 2.0;
            r = Math.Abs(r);
            return r;
        };
        /// <summary>
        /// double GetLocalPower(double power, double x, double y)
        /// </summary>
        public static Func<double, double, double, double> GetLocalPower = (p, x, y) =>
        {
            var r = GetDistance(x, y);
            return DiffuseG * p / (Math.Pow(r, 2.0));
        };

        /// <summary>
        /// 每一个Crystall 每一回合掉下的 Power值，d是此Crystall 的Power
        /// </summary>
        public static Func<double, double> DieEnergy = d =>
        {
            return d > 10 ? Math.Pow(d, 1.25)*0.1 : 2;
        };

        public static Func<int, int> PurposeEnergy = i => (int)Math.Ceiling(Math.Pow(i, 0.6) * 12.1);

        /// <summary>
        /// double EnergyReleaseRate(double orgin_plan_add_energy);
        /// <para> return: modified plan_add_energy </para>
        /// </summary>
        public static Func<double, double> ExpectEnergyRate = d =>
        {
            return Math.Pow(d, 0.6);
        };

        /// <summary>
        /// Size -> ExpectCrystallCount
        /// </summary>
        public static Func<int, int> ExpectCrystallCount = i => (int) Math.Ceiling(Math.Pow(i, 0.4));

        /// <summary>
        /// (return Dec num) CrowDec(double distance, double origin_power)
        /// </summary>
        public static Func<double, double, double> CrowDec = (d, p) =>
        {
            d = Math.Max(d, 0);
            return (d <= 5) ? p*(0.05*Math.Pow(d + 1, -1)) : 0;
        };

        public static Func<double, double> PowerToR = d => d;

        /// <summary>
        /// Size -> Expect Energy Source Count
        /// </summary>
        public static Func<int, int> ExpectEnergySourceNum = d => (int)Math.Pow(d, 0.5);

        public const int InitPower = 6;
        public static Func<int, int> SeedRate = i => (int)Math.Ceiling(Math.Pow(i, 0.5));

        public const int PmaxScatterRange = 4;
        /// <summary>
        /// R2 -> rate
        /// </summary>
        public static Func<double, double> PmaxScatter = d =>
        {
            return (d == 0 ? 1 : 1.0 / (d + 1));
        };

    }
}
