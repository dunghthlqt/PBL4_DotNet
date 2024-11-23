using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PBL4_DotNet
{
    public partial class Tools_Route : UserControl
    {
        private const string Data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"; // Dữ liệu gói tin ping (có thể thay đổi)

        public Tools_Route()
        {
            InitializeComponent();
        }

        private async void buttonStart_Click(object sender, EventArgs e)
        {
            string domain = textBoxRoute.Text.Trim(); // Lấy tên miền/IP từ TextBox
            if (string.IsNullOrEmpty(domain))
            {
                MessageBox.Show("Vui lòng nhập tên miền hoặc địa chỉ IP.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            richTextBoxRoute.Text = "Đang kiểm tra route, vui lòng chờ...\n";

            try
            {
                // Chạy traceroute trong luồng nền để tránh làm giao diện "đơ"
                await Task.Run(() => RunTraceroute(domain));
            }
            catch (Exception ex)
            {
                richTextBoxRoute.Text = $"Lỗi: {ex.Message}";
            }
        }

        private async void RunTraceroute(string hostNameOrAddress)
        {
            Ping pinger = new Ping();
            int timeout = 30000; // Thời gian chờ trong ms
            List<IPAddress> hops = new List<IPAddress>();
            int delayBetweenHops = 3000; // Thời gian chờ giữa các hops (1000 ms = 1 giây)

            // Thực hiện Traceroute với TTL tăng dần
            for (int ttl = 1; ttl <= 30; ttl++) // Thử 30 hops
            {
                PingOptions pingOptions = new PingOptions(ttl, true); // Thiết lập TTL
                byte[] buffer = Encoding.ASCII.GetBytes(Data);
                Stopwatch stopwatch = new Stopwatch(); // Khởi tạo Stopwatch để đo thời gian

                try
                {
                    stopwatch.Start(); // Bắt đầu đo thời gian
                    PingReply reply = await pinger.SendPingAsync(hostNameOrAddress, timeout, buffer, pingOptions);
                    stopwatch.Stop(); // Dừng Stopwatch sau khi nhận được phản hồi

                    if (reply.Status == IPStatus.Success)
                    {
                        hops.Add(reply.Address);
                        AppendToRichTextBox($"{ttl}\t{reply.Address}\t{stopwatch.ElapsedMilliseconds} ms");
                        AppendToRichTextBox("Traceroute hoàn tất.");
                        break;
                    }
                    else if (reply.Status == IPStatus.TtlExpired || reply.Status == IPStatus.TimedOut)
                    {
                        if (reply.Status == IPStatus.TtlExpired)
                        {
                            hops.Add(reply.Address);
                            AppendToRichTextBox($"{ttl}\t{reply.Address}\t{stopwatch.ElapsedMilliseconds} ms");
                        }
                        else
                        {
                            AppendToRichTextBox($"{ttl}\t*\tTimeout");
                        }
                    }
                    else
                    {
                        AppendToRichTextBox($"{ttl}\t*\tKhông thể tiếp cận");
                    }

                    // Thêm thời gian chờ giữa các hops
                    await Task.Delay(delayBetweenHops); // Thêm thời gian chờ giữa các hops
                }
                catch (Exception ex)
                {
                    AppendToRichTextBox($"{ttl}\t*\tLỗi: {ex.Message}");
                }
            }
        }


        // Hàm dùng để cập nhật giao diện người dùng (richTextBoxRoute)
        private void AppendToRichTextBox(string text)
        {
            if (richTextBoxRoute.InvokeRequired)
            {
                richTextBoxRoute.Invoke(new Action(() =>
                {
                    richTextBoxRoute.AppendText(text + "\n");
                }));
            }
            else
            {
                richTextBoxRoute.AppendText(text + "\n");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBoxRoute.Text = "";
            richTextBoxRoute.Text = "";
        }
    }
}
