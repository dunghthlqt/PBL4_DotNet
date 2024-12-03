using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PBL4_DotNet
{
    public partial class Form1 : Form
    {
        public SplitContainer SplitContainer1 => splitContainer1;
        public Form1()
        {
            bool isAdmin;

            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);

                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            if (isAdmin)
            {
                InitializeComponent();
                splitContainer1.IsSplitterFixed = true;

                splitContainer1.Panel1.Controls.Add(new MainMenu(this));
            }
            else
            {
                MessageBox.Show("Administrator permission is required to run this application :0 !");
                Environment.Exit(0);
            }
        }
    }
}
