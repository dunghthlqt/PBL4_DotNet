using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PBL4_DotNet
{
    public partial class Tools_Ports : UserControl
    {
        private CancellationTokenSource _cancellationTokenSource;

        public Tools_Ports()
        {
            InitializeComponent();
            comboBox1.Items.Add("Common");
            comboBox1.Items.Add("All");
            comboBox1.SelectedIndex = 0;
            button2.Enabled = false;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                button1.Enabled = false;
                button2.Enabled = true;
                richTextBox2.Clear();
                progressBar1.Value = 0;

                IPAddress ipAddress = IPAddress.Parse(textBox1.Text);
                string mode = comboBox1.SelectedItem.ToString();

                _cancellationTokenSource = new CancellationTokenSource();

                if (mode.Equals("Common"))
                {
                    await ScanCommonPorts(_cancellationTokenSource.Token);
                }
                else
                {
                    await ScanAllPorts(_cancellationTokenSource.Token);
                }

                MessageBox.Show("Scan completed!", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Scan was cancelled.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button1.Enabled = true;
                button2.Enabled = false;
                progressBar1.Value = 0;
            }
        }

        public async void button2_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();

        }

        private async Task ScanCommonPorts(CancellationToken cancellationToken)
        {
            string path = "D:\\PBL4\\PBL4_DotNet\\PBL4_DotNet\\File\\common_port.txt";
            List<int> ports = new List<int>();

            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    if (int.TryParse(line, out int port))
                    {
                        ports.Add(port);
                    }
                }
            }

            var tasks = new List<Task>();
            int processedPorts = 0;

            foreach (int port in ports)
            {
                cancellationToken.ThrowIfCancellationRequested();

                tasks.Add(ScanPort(port));
                processedPorts++;

                UpdateProgress(processedPorts * 100 / ports.Count);

                // Limit concurrent connections
                if (tasks.Count >= 10)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(t => t.IsCompleted);
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task ScanAllPorts(CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            const int totalPorts = 20000;
            int processedPorts = 0;

            for (int port = 1; port <= totalPorts; port++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                tasks.Add(ScanPort(port));
                processedPorts++;

                UpdateProgress(processedPorts * 100 / totalPorts);

                // Limit concurrent connections
                if (tasks.Count >= 10)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(t => t.IsCompleted);
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task ScanPort(int port)
        {
            try
            {
                using (var tcp = new TcpClient())
                {
                    var connectTask = tcp.ConnectAsync(textBox1.Text, port);
                    if (await Task.WhenAny(connectTask, Task.Delay(1000)) == connectTask)
                    {
                        UpdateResult($"{port} is open\n");
                    }
                }
            }
            catch
            {
                // Port is closed or connection failed
            }
        }

        private void UpdateProgress(int value)
        {
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    progressBar1.Value = Math.Min(value, 100);
                });
            }
            else
            {
                progressBar1.Value = Math.Min(value, 100);
            }
        }

        private void UpdateResult(string text)
        {
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    richTextBox2.AppendText(text);
                });
            }
            else
            {
                richTextBox2.AppendText(text);
            }
        }

        private void Tools_Ports_Load(object sender, EventArgs e)
        {

        }
    }
}