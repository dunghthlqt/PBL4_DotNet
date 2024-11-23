using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PBL4_DotNet
{
    public partial class Tools_DNS : UserControl
    {
        public Tools_DNS()
        {
            InitializeComponent();
            AddDNServer();
        }

        public void AddDNServer()
        {
            List<String> server = new List<String>();
            server.Add("Google Public DNS - 8.8.8.8");
            server.Add("Cloudfare DNS - 1.1.1.1");
            foreach (String s in server)
            {
                comboBox1.Items.Add(s);
            }
        }

        public async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("Please select a DNS server");
                return;
            }

            String ServerIP = comboBox1.Text.Split('-')[1].Trim();

            Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ServerIP), 53);
            socket.Connect(ep);

            try
            {
                await SendQueryAsync(socket);
                await ReceiveResponseAsync(socket);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                if (socket != null && socket.Connected)
                {
                    socket.Close();
                }
            }
        }

        public async Task SendQueryAsync(Socket socket)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                throw new ArgumentException("Domain name cannot be empty");
            }

            String host1 = textBox1.Text.Trim();
            string[] hostParts = host1.Split('.');
            if (hostParts.Length != 2)
            {
                throw new ArgumentException("Invalid domain format");
            }

            byte[] hostnameLength = new byte[1];
            byte[] hostdomainLength = new byte[1];

            byte[] tranactionID1 = { 0x46, 0x62 };
            byte[] queryType1 = { 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] hostname = Encoding.ASCII.GetBytes(hostParts[0]);
            hostnameLength[0] = (byte)hostname.Length;
            byte[] hostdomain = Encoding.ASCII.GetBytes(hostParts[1]);
            hostdomainLength[0] = (byte)hostdomain.Length;
            byte[] queryEnd = { 0x00, 0x00, 0x01, 0x00, 0x01 };
            byte[] dnsQueryString = tranactionID1.Concat(queryType1).Concat(hostnameLength).Concat(hostname).Concat(hostdomainLength).Concat(hostdomain).Concat(queryEnd).ToArray();

            await Task.Run(() => socket.Send(dnsQueryString));
        }

        public async Task ReceiveResponseAsync(Socket socket)
        {
            byte[] rBuffer = new byte[1000];

            int receivedLength = await Task.Run(() => socket.Receive(rBuffer));

            var transId = (ushort)BitConverter.ToInt16(new byte[] { rBuffer[1], rBuffer[0] }, 0);
            var queCount = (ushort)BitConverter.ToInt16(new byte[] { rBuffer[5], rBuffer[4] }, 0);
            var ansCount = (ushort)BitConverter.ToInt16(new[] { rBuffer[7], rBuffer[6] }, 0);
            var authCount = (ushort)BitConverter.ToInt16(new[] { rBuffer[9], rBuffer[8] }, 0);
            var addCount = (ushort)BitConverter.ToInt16(new[] { rBuffer[11], rBuffer[10] }, 0);

            int byteCount = 12;
            Question[] questions = new Question[queCount];
            for (int i = 0; i < queCount; i++)
            {
                questions[i] = new Question();  // Initialize Question object

                // Read Name
                while (true)
                {
                    int stringLength = rBuffer[byteCount];
                    byteCount++;
                    if (stringLength == 0)
                    {
                        if (!string.IsNullOrEmpty(questions[i].qName) && questions[i].qName[questions[i].qName.Length - 1] == '.')
                        {
                            questions[i].qName = questions[i].qName.Substring(0, questions[i].qName.Length - 1);
                        }
                        break;
                    }
                    byte[] tempName = new byte[stringLength];
                    for (int k = 0; k < stringLength; k++)
                    {
                        tempName[k] = rBuffer[byteCount];
                        byteCount++;
                    }
                    questions[i].qName += Encoding.ASCII.GetString(tempName) + '.';
                }
                // Name read now read Type
                questions[i].qType = rBuffer[byteCount] + rBuffer[byteCount + 1];
                byteCount += 2;
                questions[i].qClass = rBuffer[byteCount] + rBuffer[byteCount + 1];
                byteCount += 2;
            }

            Answer[] answers = new Answer[ansCount];
            for (int i = 0; i < ansCount; i++)
            {
                answers[i] = new Answer();  // Initialize Answer object
                answers[i].aName = new List<byte>();  // Initialize aName list

                // Skip reading Name, since it points to the Name given in question
                byteCount += 2;
                answers[i].aType = rBuffer[byteCount] + rBuffer[byteCount + 1];
                byteCount += 2;
                answers[i].aClass = rBuffer[byteCount] + rBuffer[byteCount + 1];
                byteCount += 2;
                answers[i].aTtl = BitConverter.ToInt32(rBuffer.Skip(byteCount).Take(4).Reverse().ToArray(), 0);
                byteCount += 4;
                answers[i].rdLength = BitConverter.ToInt16(rBuffer.Skip(byteCount).Take(2).Reverse().ToArray(), 0);
                byteCount += 2;
                answers[i].rData = rBuffer.Skip(byteCount).Take(answers[i].rdLength).ToArray();
                byteCount += answers[i].rdLength;
            }

            StringBuilder result = new StringBuilder();
            foreach (var a in answers)
            {
                if (a.aType == 5)  // CNAME record
                {
                    string namePortion = "";
                    for (int bytePosition = 0; bytePosition < a.rData.Length;)
                    {
                        int length = a.rData[bytePosition];
                        bytePosition++;
                        if (length == 0) continue;
                        namePortion += Encoding.ASCII.GetString(a.rData.Skip(bytePosition).Take(length).ToArray()) + ".";
                        bytePosition += length;
                    }
                    result.AppendLine(namePortion.TrimEnd('.'));
                }
                if (a.aType == 1)  // A record
                {
                    string ipString = "";
                    foreach (var b in a.rData)
                    {
                        ipString += b + ".";
                    }
                    result.AppendLine(ipString.TrimEnd('.'));
                }
            }

            ResultUpdate(result);
        }
        public void ResultUpdate(StringBuilder result)
        {
            String Server = "Server: " + comboBox1.Text.Split('-')[0].Trim() + "\n";
            String Address = "Address: " + comboBox1.Text.Split('-')[1].Trim() + "\n\n";
            String Name = "Name:  " + textBox1.Text + "\n";
            String Addresses = "Addresses: " + result.ToString() + "\n";
            richTextBox1.Text = Server + Address + Name + Addresses;
        }
    }

    class Question
    {
        public Question()
        {
            qName = string.Empty;
        }

        public string qName { get; set; }
        public int qType { get; set; }
        public int qClass { get; set; }
    }

    class Answer
    {
        public Answer()
        {
            aName = new List<byte>();
        }

        public List<byte> aName { get; set; }
        public int aType { get; set; }
        public int aClass { get; set; }
        public int aTtl { get; set; }
        public int rdLength { get; set; }
        public byte[] rData { get; set; }
    }
}