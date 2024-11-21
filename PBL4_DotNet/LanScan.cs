using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PBL4_DotNet
{
    public partial class LanScan : UserControl
    {
        public LanScan()
        {
            InitializeComponent();
            addInterface();
            splitContainer1.IsSplitterFixed = true;
        }
        public void addInterface()
        {
            var InterfaceList = new List<Interface>();
            try 
            {
                var networkInterface = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var Interface in networkInterface) 
                {
                    if(Interface.OperationalStatus == OperationalStatus.Up && (Interface.NetworkInterfaceType == NetworkInterfaceType.Ethernet || Interface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        var ip = Interface.GetIPProperties().UnicastAddresses.FirstOrDefault(i => i.Address.AddressFamily == AddressFamily.InterNetwork);

                        var inter = new Interface(Interface.Name, ip.Address.ToString());
                        InterfaceList.Add(inter);
                    }    
                }
            }
            catch (Exception ex) { }
            foreach (var Interface in InterfaceList) 
            {
                String temp = Interface.InterfaceName + " : " + Interface.IpAddress;
                comboBox1.Items.Add(temp);
            }
        }
        public async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            splitContainer1.Panel2.Controls.Clear();
            splitContainer1.Panel2.Controls.Add(new LanScan_ViewResult(comboBox1.GetItemText(comboBox1.SelectedItem)));
            button1.Enabled = true;
        }
    }
    public class Interface
    {
        public String InterfaceName;
        public String IpAddress;
        public Interface(String InterfaceName, String IpAddress) 
        {
            this.InterfaceName = InterfaceName;
            this.IpAddress = IpAddress;
        }
    }
}
