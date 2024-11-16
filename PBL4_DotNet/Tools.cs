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
    public partial class Tools : UserControl
    {
        public SplitContainer SplitContainer1 => splitContainer1;
        public Tools()
        {
            InitializeComponent();
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Panel1.Controls.Add(new Tools_Menu(this));
        }
    }
}
