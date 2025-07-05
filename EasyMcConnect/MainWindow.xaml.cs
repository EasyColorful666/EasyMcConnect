using System;
using System.Diagnostics;
using Tomlyn;
using Tomlyn.Model;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Diagnostics;
using iNKORE.UI.WPF.Controls;
using System.IO;

namespace EasyMcConnect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int srcport = new Random().Next(10000, 65535);
        public static string token = Guid.NewGuid().ToString().Replace("-", "");

        public MainWindow()
        {
            InitializeComponent();
            textBox4.Text += $"当前电脑主机名:{Dns.GetHostName()}\r\n";
            textBox5.Text = srcport.ToString();
            textBox3_frp.Text = srcport.ToString();
            textBox4_frp.Text += $"当前电脑IP:{Dns.GetHostAddresses(Dns.GetHostName())[0]}\r\n";
        }

        private  void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process p= new();
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = "openp2p.exe";
                string protocol="";
                switch (comboBox1.SelectedIndex)
                {
                    case 0: { protocol = "tcp"; break; }
                    case 1: { protocol = "udp"; break; }
                }
                string arg = $"-node {textBox3.Text} -token 18049001166862278348 -appname {textBox1.Text} -peernode {Dns.GetHostName()} -dstip localhost -dstport {textBox2.Text}  -protocol {protocol} -srcport {srcport}";
                processStartInfo.Arguments = arg;
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.CreateNoWindow = true;
                Task.Run(() => {
                    p= Process.Start(processStartInfo);
                });
                textBox4.AppendText("Openp2p Client 已启动\r\n");
                p.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        textBox4.AppendText(e.Data + "\r\n");
                    }
                });
                p.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        textBox4.AppendText(e.Data + "\r\n");
                    }
                });
                

                //await Task.Run(() => {

                //    while (!p.StandardOutput.EndOfStream || !p.StandardError.EndOfStream)
                //    {
                //        this.Dispatcher.Invoke(() =>
                //        {
                //            textBox4.AppendText(p.StandardOutput.ReadLine() + "\r\n");
                //        });
                //        this.Dispatcher.Invoke(() =>
                //        {
                //            textBox4.AppendText(p.StandardError.ReadLine() + "\r\n");
                //        });

                //    }
                //});
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process p in processes)
                {
                   
                    if (p.ProcessName == "openp2p")
                    {
                        p.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button复制__C__Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo processStartInfo = new();
            processStartInfo.FileName = ".\\openp2p.exe";
            processStartInfo.CreateNoWindow = true;
            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            textBox4.Clear();
        }

        private void button1_frp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process p = new();
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = "frpc.exe";
                string arg = $"-c frpc.toml";
                processStartInfo.Arguments = arg;
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.CreateNoWindow = true;
                FileStream fs = new FileStream("frpc.toml", FileMode.Create);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                var toml = $@"serverAddr = ""154.7.177.37""
auth.token = ""wy780104""
serverPort = 7000
[[proxies]]
name = ""game{token}""
type = ""{comboBox1_frp.Text.ToLower()}""
localIP = ""127.0.0.1""
localPort = {textBox1_frp.Text}
remotePort = {textBox3_frp.Text}";

                fs.Write(Encoding.UTF8.GetBytes(toml), 0, Encoding.UTF8.GetByteCount(toml));
                fs.Flush();
                fs.Close();

                   p = Process.Start(processStartInfo);
               
                textBox4.AppendText("frpc 已启动\r\n");
                p.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        textBox4_frp.AppendText(e.Data + "\r\n");
                    }
                });
                p.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        textBox4_frp.AppendText(e.Data + "\r\n");
                    }
                });
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public static Proxies Parse(string toml)
        {
            var model = Toml.ToModel<Proxies>(toml);
            return model;
        }

        private void button2_frp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process p in processes)
                {

                    if (p.ProcessName == "frpc")
                    {
                        p.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_frp_Click(object sender, RoutedEventArgs e)
        {
            textBox4_frp.Clear();

        }
    }
    public class Proxies
    {
        public string name { get; set; }
        public string type { get; set; }
        public string localIP { get; set; }
        public int localPort { get; set; }
        public int remotePort { get; set; }

    }
}