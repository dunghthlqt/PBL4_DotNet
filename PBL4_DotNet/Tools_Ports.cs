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
        private List<Port> PortDescription;
        private List<Port> FoundPort;
        private const string PORT_DESCRIPTION_PATH = "..\\..\\File\\PortDescription.txt";

        public Tools_Ports()
        {
            InitializeComponent();
            PortDescription = new List<Port>();
            FoundPort = new List<Port>();
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
                progressBar1.Value = 0;
                dataGridView1.DataSource = null;
                FoundPort.Clear();

                PortDecriptionCollect();
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
            string path = "..\\..\\File\\CommonPort.txt";
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
                if (tasks.Count >= 50)
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
                    IPAddress ip = IPAddress.Parse(textBox1.Text);
                    var connectTask = tcp.ConnectAsync(ip, port);
                    if (await Task.WhenAny(connectTask, Task.Delay(1000)) == connectTask)
                    {
                        UpdateResult(port);
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

        private void UpdateResult(int port)
        {
            Port foundPort = new Port(port, DecriptionFind(port));
            Console.WriteLine(foundPort.PortNumber);
            FoundPort.Add(foundPort);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = FoundPort;
        }
        private String DecriptionFind(int port)
        {
            String description = "";
            for(int i = 0; i < PortDescription.Count(); i++)
            {
                if (PortDescription[i].PortNumber == port)
                {
                    description = PortDescription[i].Description;
                    break;
                }
            }
            return description;
        }
        private void PortDecriptionCollect()
        {
            StreamReader reader = new StreamReader(PORT_DESCRIPTION_PATH);
            String line;
            while((line = reader.ReadLine()) != null)
            {
                Port port = new Port(Int32.Parse(line.Split('-')[0].Trim()), line.Split('-')[1].Trim());
                PortDescription.Add(port);
            }
        }
    }
    public class Port
    {
        public int PortNumber { get; set; }
        public String Description { get; set; }
        public Port(int PortNumber, String Description) 
        {
            this.PortNumber = PortNumber;
            this.Description = Description;
        }
    }
}