using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrystallGrow
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();

            var adress = "input.jpg";//args[0];
            //Factory.ScatterPmax(Factory.PmaxFromImage(new Bitmap(Image.FromFile(adress))))
            var natrue =
                new NatureMachine(Factory.ScatterPmax(Factory.PmaxFromImage(new Bitmap(Image.FromFile(adress)))));//Factory.ScatterPmax(Factory.GenerateTide(4000,2000)));
            for (int i = 0; i < 70; i++)
                natrue.GameOne(true);
            for (int i = 0; i < 5; i++)
                natrue.GameOne(true, false);
            for (int i = 0; i < 2; i++)
                natrue.GameOne(false, false);
            natrue.Draw("re.svg");
            Console.ReadKey();
        }
    }
}
