using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PBL4_DotNet
{
    public partial class Tools_Menu : UserControl
    {
        public Tools _parentControl;
        public Tools_Menu(Tools parentControl)
        {
            InitializeComponent();
            _parentControl = parentControl;
            button1.PerformClick();
        }
        public void button1_Click(object sender, EventArgs e) 
        {
            _parentControl.SplitContainer1.Panel2.Controls.Clear();
            _parentControl.SplitContainer1.Panel2.Controls.Add(new Tools_Ping());
        }
        public void button2_Click(object sender, EventArgs e)
        {
            _parentControl.SplitContainer1.Panel2.Controls.Clear();
            _parentControl.SplitContainer1.Panel2.Controls.Add(new Tools_Route());
        }
        public void button3_Click(object sender, EventArgs e)
        {
            _parentControl.SplitContainer1.Panel2.Controls.Clear();
            _parentControl.SplitContainer1.Panel2.Controls.Add(new Tools_Ports());
        }
        public void button5_Click(object sender, EventArgs e)
        {
            _parentControl.SplitContainer1.Panel2.Controls.Clear();
            _parentControl.SplitContainer1.Panel2.Controls.Add(new Tools_DNS());
        }
    }
}
