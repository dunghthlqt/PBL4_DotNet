﻿using System;
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
    public partial class WifiSignal : UserControl
    {
        public WifiSignal()
        {
            InitializeComponent();
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Panel2.Controls.Add(new WifiSignal_ViewResult());
        }
    }
}
