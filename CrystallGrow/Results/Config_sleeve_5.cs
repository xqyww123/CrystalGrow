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
        /// double GetLocalPower(double power, double x, double y, double r)
        /// </summary>
        public static Func<double, double, double, double, double> GetLocalPower = (p, x, y, r) =>
        {
            var d = GetDistance(x, y);
            if (d < r) return p;
            return DiffuseG * p / (Math.Pow(d, 3.0));
        };

        /// <summary>
        /// 每一个Crystall 每一回合掉下的 Power值，d是此Crystall 的Power
        /// </summary>
        public static Func<double, double> DieEnergy = d =>
        {
            return d > 10 ? Math.Pow(d, 1.29)*0.1 : 2;
        };

        public static Func<int, int> PurposeEnergy = i =>
        {
            return (int) Math.Ceiling(Math.Pow(i, 0.5)*10.1);
        };

        /// <summary>
        /// double EnergyReleaseRate(double orgin_plan_add_energy);
        /// <para> return: modified plan_add_energy </para>
        /// </summary>
        public static Func<double, double> ExpectEnergyRate = d =>
        {
            return Math.Pow(d, 0.5);
        };

        /// <summary>
        /// Size -> ExpectCrystallCount
        /// </summary>
        public static Func<int, int> ExpectCrystallCount = i => (int) Math.Ceiling(Math.Pow(i, 0.5)*1);

        /// <summary>
        /// Size -> Expect Energy Source Count
        /// </summary>
        public static Func<int, int> ExpectEnergySourceNum = d => (int)(Math.Pow(d, 0.5));
        public const int InitPower = 6;
        public static Func<int, int> SeedRate = i => 90;

        /// <summary>
        /// (return Dec num) CrowDec(double distance, double origin_power)
        /// </summary>
        public static Func<double, double, double> CrowDec = (d, p) =>
        {
            d = Math.Max(d, 0);
            return (d <= 0) ? p*(0.1*Math.Pow(d + 1, -1)) : 0;
        };

        /// <summary>
        /// Current Power -> Decline power
        /// </summary>
        public static Func<double, double> AnnealDecPower = d => Math.Pow(d, 0.1);
        public const int MinAnnealInterval = 20;

        /// <summary>
        /// (Current Interval - MinAnnealInterval) -> Invoke P
        /// </summary>
        public static Func<int, double> AnnealInvokeP = i => Math.Min(0.2, Math.Pow(i, 1.08)/100.0);

        public static Func<double, double> PowerToR = d => d;

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
