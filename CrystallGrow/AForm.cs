using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrystallGrow
{
    public partial class AForm : Form
    {
        public AForm()
        {
            InitializeComponent();
        }

        public Panel MainPanel { get { return panel1; } }
    }
}
