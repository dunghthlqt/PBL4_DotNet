using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing;

namespace PBL4_DotNet
{
    public partial class Information : UserControl
    {
        public Information()
        {
            InitializeComponent();
            SetupDataGrids();
            LoadNetworkAdapters();
        }

        private void SetupDataGrids()
        {
            // Cấu hình DataGridView cho thông tin adapter mạng
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.RowHeadersVisible = false;

            dataGridView1.Columns.Add("Property", "Property");
            dataGridView1.Columns.Add("Value", "Value");

            // Cấu hình DataGridView cho thông tin mạng Wi-Fi
            dataGridView2.Columns.Clear();
            dataGridView2.Rows.Clear();
            dataGridView2.RowHeadersVisible = false;

            dataGridView2.Columns.Add("Property", "Property");
            dataGridView2.Columns.Add("Value", "Value");
        }

        private void LoadNetworkAdapters()
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();

            var adapter = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(a => a.OperationalStatus == OperationalStatus.Up &&
                                     (a.Name.Contains("Wi-Fi") || a.Name.Contains("Ethernet")));

            if (adapter != null)
            {
                var properties = adapter.GetIPProperties();
                var ipAddresses = properties.UnicastAddresses
                    .Where(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(a => a.Address.ToString())
                    .ToList();

                // Hiển thị thông tin về adapter vào dataGridView1
                dataGridView1.Rows.Add("Name", adapter.Name);
                dataGridView1.Rows.Add("Description", adapter.Description);
                dataGridView1.Rows.Add("Status", adapter.OperationalStatus.ToString());
                dataGridView1.Rows.Add("Speed (Mbps)", adapter.Speed / 1_000_000);
                dataGridView1.Rows.Add("IP Address", ipAddresses.Any() ? string.Join(", ", ipAddresses) : "N/A");

                // Nếu adapter là Wi-Fi, lấy thông tin về mạng Wi-Fi và hiển thị vào dataGridView2
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    AddCurrentWifiNetworkInfo();
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy adapter mạng phù hợp.");
            }
        }

        private void AddCurrentWifiNetworkInfo()
        {
            try
            {
                string output;
                Process proc = new Process();
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.FileName = "netsh";
                proc.StartInfo.Arguments = "wlan show interfaces";
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();

                output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();

                StringReader sr = new StringReader(output);
                string ssid = "", bssid = "", signal = "", radioType = "", channel = "", auth = "", encrypt = "";

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("SSID") && !line.Contains("BSSID"))
                    {
                        ssid = GetValue(line);
                    }
                    else if (line.Contains("BSSID"))
                    {
                        bssid = GetValue(line);
                    }
                    else if (line.Contains("Signal"))
                    {
                        signal = GetValue(line);
                    }
                    else if (line.Contains("Radio type"))
                    {
                        radioType = GetValue(line);
                    }
                    else if (line.Contains("Channel"))
                    {
                        channel = GetValue(line);
                    }
                    else if (line.Contains("Authentication"))
                    {
                        auth = GetValue(line);
                    }
                    else if (line.Contains("Encryption"))
                    {
                        encrypt = GetValue(line);
                    }
                }

                // Hiển thị thông tin mạng Wi-Fi vào dataGridView2
                dataGridView2.Rows.Add("SSID", ssid);
                dataGridView2.Rows.Add("BSSID", bssid);
                dataGridView2.Rows.Add("Signal", signal);
                dataGridView2.Rows.Add("Radio Type", radioType);
                dataGridView2.Rows.Add("Channel", channel);
                dataGridView2.Rows.Add("Authentication", auth);
                dataGridView2.Rows.Add("Encryption", encrypt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy thông tin mạng Wi-Fi: " + ex.Message, "Lỗi");
            }
        }

        private string GetValue(string line)
        {
            var parts = line.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 2 ? parts[1].Trim() : "";
        }
    }
}
