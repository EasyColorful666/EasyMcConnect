using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Diagnostics;
using iNKORE.UI.WPF.Controls;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace EasyMcConnect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int srcport = new Random().Next(10000, 65535);
        public string token = "";
        public FileStream fs = new FileStream("emcc.json", FileMode.Create);
        

        public MainWindow()
        {
            InitializeComponent();
            fs.Close();
            
            if (!(File.Exists("emcc.json") || (new FileInfo("emcc.json").Length == 0)))
            {
                FileStream fs = new FileStream("emcc.json", FileMode.OpenOrCreate);
                byte[] buffer = new byte[fs.Length];
                string token = Guid.NewGuid().ToString();
                buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new Config { token = token }));
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
                fs.Close();
            }
            else
            {
                FileStream fs2 = new FileStream("emcc.json", FileMode.Open);
                byte[] buffer2 = new byte[fs2.Length];
                fs2.Read(buffer2, 0, buffer2.Length);
                Config config = JsonConvert.DeserializeObject<Config>(Encoding.UTF8.GetString(buffer2));
                this.token = config.token;
                fs2.Close();
            }




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
                FileStream fs2 = new FileStream("emcc.json", FileMode.Open);
                byte[] buffer2 = new byte[fs2.Length];
                fs2.Read(buffer2, 0, buffer2.Length);
                var toml = $@"serverAddr = ""154.7.177.37""
auth.token = ""wy780104""
serverPort = 7000
[[proxies]]
name = ""game.{token}""
type = ""{comboBox1_frp.Text.ToLower()}""
localIP = ""127.0.0.1""
localPort = {textBox1_frp.Text}
remotePort = {textBox3_frp.Text}";

                fs.Write(Encoding.UTF8.GetBytes(toml), 0, Encoding.UTF8.GetByteCount(toml));
                fs.Flush();
                fs.Close();

                p = Process.Start(processStartInfo);
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                textBox4_frp.AppendText("frpc 已启动\r\n");
                p.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            textBox4_frp.AppendText(e.Data + "\r\n");
                        });
                    }
                });
                p.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            textBox4_frp.AppendText(e.Data + "\r\n");
                        });
                    }
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

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

        private void button1_et_Click(object sender, RoutedEventArgs e)
        {
            Process p = new();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "easytier-core.exe";
            processStartInfo.Arguments = $"-d --network-name {textBox1_et.Text} --network-secret {textBol2_et.Text} -p tcp://154.7.177.37:11010";
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.CreateNoWindow = true;
            p = Process.Start(processStartInfo);
            textBox4_et.AppendText("EasyTier 已启动\r\n");
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        textBox4_et.AppendText(e.Data + "\r\n");
                        Regex IPAd = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                        MatchCollection MatchResult = IPAd.Matches(e.Data);
                        textBox3_et.Text = MatchResult[0].Value;
                    });
                }
            });
            p.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        textBox4_et.AppendText(e.Data + "\r\n");
                    });
                }
            });
        }
        

        private void button3_et_Click(object sender, RoutedEventArgs e)
        {
              textBox4_et.Clear();
        }

        private void button2_et_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process p in processes)
                {

                    if (p.ProcessName == "easytier-core")
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
class Config
{
    public string token { get; set; }
}