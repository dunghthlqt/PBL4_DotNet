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
        private int DeviceCount = 0;
        private const int PING_TIMEOUT = 5000;
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
                label2.Text = HostInfo.Split('.')[0] + "." + HostInfo.Split('.')[1] + "." + HostInfo.Split('.')[2] + ".0";
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
            DeviceCount = 0;
            var tasks = new List<Task>();
            String HostIpAddress = HostInfo.Split(':')[1].Trim();
            ClearArpCache();

            String subnet = HostIpAddress.Split('.')[0].Trim() + "." + HostIpAddress.Split('.')[1].Trim() + "." + HostIpAddress.Split('.')[2].Trim() + ".";
            for (int i = 0; i <= 255; i++)
            {
                String host = subnet + i.ToString();
                tasks.Add(PingHostAsync(host));
                //tasks.Add(DiscoverHostAsync(host));
                //await PingHostAsync(host);
            }
            await Task.WhenAll(tasks);
            tasks.Clear();
            UpdateProgress(25);

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
            UpdateProgress(50);

            String MacAddress = String.Join("-", Enumerable.Range(0, 6).Select(i => sMacAddress.Substring(i * 2, 2))).ToLower();
            List<ArpItem> arplist = CollectArpResult(subnet);
            arplist.Add(new ArpItem(HostIpAddress, MacAddress));
            UpdateProgress(75);

            int scannedDevice = 0;
            foreach (var device in connectabledevice)
            {
                scannedDevice += 1;
                foreach (var arp in arplist)
                {

                    if (arp.Ip.Trim().Equals(device.Trim()))
                    {
                        scannedDevice += 1;
                        DeviceCount += 1;
                        tasks.Add(CollectDeviceInfo(device, arp.MacAddress));
                    }
                }
            }
            await Task.WhenAll(tasks);
            UpdateProgress(100);
            label2.Text = HostInfo.Split('.')[0] + "." + HostInfo.Split('.')[1] + "." + HostInfo.Split('.')[2] + ".0 (" + DeviceCount + ")";
        }

        private async Task PingHostAsync(String host) //Thử gửi Arp request tới tất cả các Ip trong mạng
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = await ping.SendPingAsync(host, PING_TIMEOUT);
                    //if (reply.Status == IPStatus.Success)
                    //{
                        connectabledevice.Add(host);
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        //private async Task DiscoverHostAsync(String host)
        //{
        //    // Thử nhiều phương thức khác nhau
        //    bool discovered = false;
        //    // Phương thức 1: Ping 
        //    discovered = await TryPingHostAsync(host) ? true : false;

        //    // Phương thức 2: Quét cổng thông dụng
        //    if(!discovered)
        //    {
        //        discovered = await TryScanCommonPortsAsync(host) ? true : false;
        //    }
        //    // Phương thức 3: Tra cứu DNS
        //    if (!discovered)
        //    {
        //        discovered = await TryReverseDNSLookupAsync(host) ? true : false;
        //    }

        //    if (discovered)
        //    {
        //        Console.WriteLine("Alo");
        //        connectabledevice.Add(host);
        //        Console.WriteLine(host);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Blo");
        //    }
        //}
        //private async Task<bool> TryPingHostAsync(String host)
        //{
        //    try
        //    {
        //        using (Ping ping = new Ping())
        //        {
        //            PingReply reply = await ping.SendPingAsync(host, PING_TIMEOUT);
        //            return reply.Status == IPStatus.Success;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        //private async Task<bool> TryScanCommonPortsAsync(String host)
        //{
        //    Console.WriteLine("Scanning" + host);
        //    int[] commonPorts = { 80, 443, 22, 21, 25, 3389 }; // HTTP, HTTPS, SSH, FTP, SMTP, RDP
        //    foreach (int port in commonPorts)
        //    {
        //        if (await IsPortOpenAsync(host, port))
        //        {
        //            // Thực hiện ARP request để cập nhật ARP table
        //            await SendArpRequest(host);
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        //private async Task<bool> IsPortOpenAsync(string host, int port)
        //{
        //    using (TcpClient tcpClient = new TcpClient())
        //    {
        //        try
        //        {
        //            var connectTask = tcpClient.ConnectAsync(host, port);
        //            if (await Task.WhenAny(connectTask, Task.Delay(1000)) == connectTask)
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }
        //}
        //private async Task<bool> TryReverseDNSLookupAsync(String host)
        //{
        //    try
        //    {
        //        var hostEntry = await Dns.GetHostEntryAsync(host);

        //        // Thực hiện ARP request khi DNS lookup thành công
        //        await SendArpRequest(host);

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        //private async Task SendArpRequest(string host)
        //{
        //    try
        //    {
        //        // Sử dụng System.Net.NetworkInformation để gửi ARP request
        //        IPAddress ipAddress = IPAddress.Parse(host);

        //        // Gửi ping để kích hoạt ARP resolution (không nhất thiết phải chờ phản hồi)
        //        using (Ping ping = new Ping())
        //        {
        //            await ping.SendPingAsync(ipAddress, 1000);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Ghi log nếu có lỗi
        //        Console.WriteLine($"ARP request error for {host}: {ex.Message}");
        //    }
        //}

        private async Task CollectDeviceInfo(String host, String MacAdress)
        {
            String DeviceName = "";
            try
            {
                var hostEntry = await Dns.GetHostEntryAsync(host); //Thiếu ổn định
                DeviceName = hostEntry.HostName;
            }
            catch
            {
                DeviceName = "N/A";
            }

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
        private void ClearArpCache()
        {
            using (Process process = Process.Start(new ProcessStartInfo("arp", "-d *")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            })) 
            { 
                process.WaitForExit(); 
            }
        }
        private void UpdateProgress(int value)
        {
            progressBar1.Value = Math.Min(value, 100);
        }
        private void DisplayDevices(String ipAddress, String MacAddress, String DeviceName, String DeviceVendor)
        {
            DeviceInfo device = new DeviceInfo(ipAddress, DeviceName, MacAddress, DeviceVendor);
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