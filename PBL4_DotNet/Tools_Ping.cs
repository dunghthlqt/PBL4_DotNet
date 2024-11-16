using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PBL4_DotNet
{
    public partial class Tools_Ping : UserControl
    {
        private bool isPinging = false; // Biến kiểm soát trạng thái ping

        public Tools_Ping()
        {
            InitializeComponent();
        }

        // Phương thức ping một lần
        public async Task Pinghost(string host)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(host, 1000);
                    if (reply.Status == IPStatus.Success)
                    {
                        AppendTextToRichTextBox($"[SUCCESS] Ping đến {host}\n");
                        AppendTextToRichTextBox($"  - Địa chỉ: {reply.Address}\n");
                        AppendTextToRichTextBox($"  - Thời gian phản hồi: {reply.RoundtripTime}ms\n");
                        AppendTextToRichTextBox($"  - Thời gian sống (TTL): {reply.Options.Ttl}\n");
                        AppendTextToRichTextBox($"  - Kích thước gói tin: {reply.Buffer.Length} bytes\n");
                        AppendTextToRichTextBox("-------------------------------------------------\n");
                    }
                    else
                    {
                        AppendTextToRichTextBox($"[FAILED] Ping đến {host} thất bại. Trạng thái: {reply.Status}\n");
                    }
                }
            }
            catch (Exception ex)
            {
                AppendTextToRichTextBox($"[ERROR] Lỗi khi ping: {ex.Message}\n");
            }
        }

        // Phương thức ping liên tục
        private async Task PingHostContinuously(string host)
        {
            while (isPinging)
            {
                await Pinghost(host);
                await Task.Delay(1000); // Dừng 1 giây trước khi ping tiếp
            }
        }

        // Nút Ping/Stop
        private async void button1_Click(object sender, EventArgs e)
        {
            string host = textBoxPing.Text.Trim();

            if (string.IsNullOrEmpty(host))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ IP hoặc tên miền hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isPinging)
            {
                // Dừng ping
                isPinging = false;
                button1.Text = "Ping";
            }
            else
            {
                // Bắt đầu ping
                isPinging = true;
                button1.Text = "Stop";

                if (checkBoxContinuousPing.Checked)
                {
                    await PingHostContinuously(host);
                }
                else
                {
                    await Pinghost(host);
                    isPinging = false;
                    button1.Text = "Ping";
                }
            }
        }

        // Nút Clear
        private void button2_Click(object sender, EventArgs e)
        {
            richTextBoxPing.Clear();
            textBoxPing.Clear();
            checkBoxContinuousPing.Checked = false;
        }

        // Cập nhật giao diện an toàn từ luồng khác
        private void AppendTextToRichTextBox(string text)
        {
            if (richTextBoxPing.InvokeRequired)
            {
                richTextBoxPing.Invoke(new Action(() => richTextBoxPing.AppendText(text)));
            }
            else
            {
                richTextBoxPing.AppendText(text);
            }
        } 
    }
}
