using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PBL4_DotNet
{
    public partial class LanScan_ViewResult : UserControl
    {
        private ConcurrentBag<String> connectabledevice;
        private List<DeviceInfo> DeviceInfomation;
        private String HostInfo;
        private const int PING_TIMEOUT = 2000;
        private const string VENDOR_FILE_PATH = "..\\..\\File\\DeviceVendor.txt";

        public LanScan_ViewResult(String HostInfo)
        {
            connectabledevice = new ConcurrentBag<String>();
            DeviceInfomation = new List<DeviceInfo>();
            this.HostInfo = HostInfo;
            InitializeComponent();
            _ = InitializeUIAsync();
        }
        private async Task InitializeUIAsync()
        {
            try
            {
                label2.Text = HostInfo;
                await ScanDevicesAsync();
                MessageBox.Show("Scan completed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during scan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task ScanDevicesAsync()
        {
            var tasks = new List<Task>();
            String HostIpAddress = HostInfo.Split(':')[1].Trim();

            String subnet = HostIpAddress.Split('.')[0].Trim() + "." + HostIpAddress.Split('.')[1].Trim() + "." + HostIpAddress.Split('.')[2].Trim() + ".";
            for (int i = 1; i <= 255; i++)
            {
                String host = subnet + i.ToString();
                tasks.Add(PingHostAsync(host));
            }
            await Task.WhenAll(tasks);
            tasks.Clear();

            String sMacAddress = string.Empty;
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                var ipProperties = adapter.GetIPProperties().UnicastAddresses
                    .FirstOrDefault(i => i.Address.AddressFamily == AddressFamily.InterNetwork);

                if (ipProperties != null && ipProperties.Address.ToString() == HostIpAddress.ToString())
                {
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                    break;
                }
            }
            String MacAddress = String.Join("-", Enumerable.Range(0, 6).Select(i => sMacAddress.Substring(i * 2, 2))).ToLower();
            List<ArpItem> arplist = CollectArpResult(subnet);
            arplist.Add(new ArpItem(HostIpAddress, MacAddress));

            int scannedDevice = 0;
            foreach (var device in connectabledevice)
            {
                Console.WriteLine("Thread: " +  device);
                scannedDevice += 1;
                UpdateProgress(scannedDevice * 100 / connectabledevice.Count);
                foreach (var arp in arplist)
                {

                    if (arp.Ip.Trim().Equals(device.Trim()))
                    {
                        Console.WriteLine("True");
                        scannedDevice += 1;
                        UpdateProgress(scannedDevice * 100 / connectabledevice.Count);
                        tasks.Add(CollectDeviceInfo(device, arp.MacAddress));
                    }
                }
            }
            await Task.WhenAll(tasks);
        }
        private async Task PingHostAsync(String host)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = await ping.SendPingAsync(host, PING_TIMEOUT);
                    if (reply.Status == IPStatus.Success)
                    {
                        connectabledevice.Add(host);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private async Task CollectDeviceInfo(String host, String MacAdress)
        {
            Console.WriteLine("Current Directory: " + Environment.CurrentDirectory);
            var hostEntry = await Dns.GetHostEntryAsync(host); //Thiếu ổn định
            String DeviceName = hostEntry.HostName;

            var oui = new List<OUI>();
            StreamReader sr = new StreamReader(VENDOR_FILE_PATH);

            String line;
            while ((line = sr.ReadLine()) != null)
            {
                OUI uoi = new OUI(line.Split(':')[0].Trim(), line.Split(':')[1].Trim());
                oui.Add(uoi);
            }
            String VendorMac = MacAdress.Split('-')[0] + "-" + MacAdress.Split('-')[1] + "-" + MacAdress.Split('-')[2];
            foreach (var o in oui)
            {
                String Mac = o.MAC.Split('-')[0] + "-" + o.MAC.Split('-')[1] + "-" + o.MAC.Split('-')[2];
                if (Mac.Trim().ToLower().Equals(VendorMac.Trim().ToLower()))
                {
                    DisplayDevices(host, DeviceName, MacAdress, o.Vendor); 
                    return;
                }
            }
            DisplayDevices(host, DeviceName, MacAdress, "N/A");
            return;
        }
        private List<ArpItem> CollectArpResult(String subnet)
        {
            String output;
            using (Process process = Process.Start(new ProcessStartInfo("arp", "-a")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            }))
            {
                output = process.StandardOutput.ReadToEnd();
            }
            var lines = output.Split('\n');

            var result = from line in lines
                         let item = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                         where item.Count() == 4 && !line.Trim().StartsWith("Interface") && !line.Trim().StartsWith(subnet.Trim() + "255") && line.Trim().StartsWith(subnet.Trim())
                         select new ArpItem()
                         {
                             Ip = item[0],
                             MacAddress = item[1],
                         };
            return result.ToList();
        }
        private void UpdateProgress(int value)
        {
            progressBar1.Value = Math.Min(value, 100);
        }
        private void DisplayDevices(String ipAddress, String MacAddress, String DeviceName, String DeviceVendor)
        {
            Console.WriteLine("Chck");
            DeviceInfo device = new DeviceInfo(ipAddress, DeviceName, MacAddress, DeviceVendor);
            Console.WriteLine(device.ipAddress);
            Console.WriteLine(device.MacAddress);
            DeviceInfomation.Add(device);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = DeviceInfomation;
        }
    }
    public class OUI
    {
        public string MAC { get; set; }
        public string Vendor { get; set; }
        public OUI(String MAC, String Vendor)
        {
            this.MAC = MAC;
            this.Vendor = Vendor;
        }
    }
    public class ArpItem
    {
        public String Ip { get; set; }
        public String MacAddress { get; set; }
        public ArpItem() { }
        public ArpItem(string ip, string macAddress)
        {
            Ip = ip;
            MacAddress = macAddress;
        }
    }
    public class DeviceInfo
    {
        public String ipAddress { get; set; }
        public String MacAddress { get; set; }
        public String DeviceName { get; set; }
        public String DeviceVendor { get; set; }

        public DeviceInfo(String ipAddress, String MacAddress, String DeviceName, String DeviceVendor)
        {
            this.ipAddress = ipAddress;
            this.MacAddress = MacAddress;
            this.DeviceName = DeviceName;
            this.DeviceVendor = DeviceVendor;
        }
    }
}