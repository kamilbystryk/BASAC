using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BASAC.ActionAutomation;
using BASAC.Database;
using BASAC.Devices;
using BASAC.MQTT;
using BASAC.Util;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using static BASAC.Controller;
using static BASAC.Database.EventEnum;

namespace BASAC
{
    class MainClass
    {
        static bool sendmail = true;
        //public event EventHandler<IotDeviceNotify> mIotDeviceNotify;

        static MqttClient client;
        static int UDPport = 8888;

        static UdpClient ClientUDP;


        static string ExecuteBashCommand(string command)
        {
            // according to: https://stackoverflow.com/a/15262019/637142
            // thans to this we will pass everything as one command
            command = command.Replace("\"", "\"\"");

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"" + command + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            proc.WaitForExit();

            return proc.StandardOutput.ReadToEnd();
        }


        public static void Main(string[] args)
        {
            var _FilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/Acq";
            if (!Directory.Exists(_FilePath))
            {
                Directory.CreateDirectory(_FilePath);
            }

            Utils.StartTime = DateTime.Now;

            if (sendmail)
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            }

            Console.WriteLine("try command...");
            /*if (ExecuteBashCommand("whoami").StartsWith("root"))
            {
                ExecuteBashCommand("iptables -t nat -A PREROUTING -p tcp --dport 80 -j REDIRECT --to-port 12345");
                ExecuteBashCommand("ufw allow 8888");
            }
            else
            {
                ExecuteBashCommand("sudo iptables -t nat -A PREROUTING -p tcp --dport 80 -j REDIRECT --to-port 12345");
                ExecuteBashCommand("sudo ufw allow 8888");
            }*/

            //ExecuteBashCommand("mosquitto &");
            Console.WriteLine("done");

            //String[] x = { "2", "3" };
            //Console.Write(x[4]);
            try
            {
                //ServiceStackHost.RunSelfHost();
            }
            catch (Exception e1)
            {
                Log.WriteLine(e1.ToString());
            }
            EventDataBase.Instance.AddEvent((int)EventType.StartApplication, Utils.DTString(DateTime.Now), 0, "");
            ClientUDP = new UdpClient(UDPport);

            try
            {
                ClientUDP.BeginReceive(new AsyncCallback(recv), null);
            }
            catch (Exception e1)
            {
                Log.WriteLine(e1.ToString());
            }


            Utils.PrepareMAC();


            //Expression e = new Expression("(20.5 > 17)");
            //Expression e = new Expression("((24 <29) & (29< 27) &1) | (29< 24)");
            //e.calculate();
            //Console.WriteLine("mat test wynik =" + e.calculate().ToString() + "\n");

            Utils.AppVer = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("################################################################################");
            Console.WriteLine(Utils.appName + ", " + Utils.AppDescription + ", ver: " + Utils.AppVer);
            Console.Write("Connect at: ");
            Console.Write(Utils.GetLocalIPAddress());
            Console.WriteLine(":12345");
            Console.WriteLine("################################################################################\n");
            Console.ForegroundColor = ConsoleColor.White;
            IotDevicesDataBase.Instance.CreateIotDevicesBase();
            IotDevicesDataBase.Instance.DBGetAllIoTDevices();
            //EnergyDataBase.Instance.CreateEnergyBase();


            ActionAutomationService.Instance.InitAutomanionService();


            client = new MqttClient("127.0.0.1");
            string clientId = Utils.MAC + "L";
            try
            {
                client.Connect(clientId);
                client.Subscribe(new string[] { MQTTenum.IoTserviceTopic + "/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                client.Subscribe(new string[] { MQTTenum.IoTtopic + "/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                client.Subscribe(new string[] { "$SYS/clients/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                client.Subscribe(new string[] { "sys/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
            }
            catch { }
            Log.WriteLine("local MQTT connected?");
            if (client.IsConnected)
            {
                Log.WriteLine("yes");
                EventDataBase.Instance.AddEvent((int)EventType.MQTTConnect, Utils.DTString(DateTime.Now), 0, "");
            }
            else
            {
                Log.WriteLine("no", ConsoleColor.DarkYellow);
            }
            Controller.Instance.mReadyToSend += mqttPublish;

            // konsola serwisowa
            if (Utils.ServiceConsole)
            {
                //var mSC = new ServiceConsole();
            }
            new Thread(delegate ()
            {
                try
                {
                    CloudMQTTClient.Instance.Connect();
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee.Message);
                }
            }).Start();
            Thread.Sleep(2000);
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        }

        static void mqttPublish(object sender, ReadyToSend e)
        {
            if (client != null)
            {
                try
                {
                    client.Publish(e.Topic, e.Message, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
                    Log.WriteLine("publish:" + e.Topic + ";" + Encoding.ASCII.GetString(e.Message));
                }
                catch
                {
                    Log.WriteLine("publish exception!!!:" + e.Topic + ";" + Encoding.ASCII.GetString(e.Message), ConsoleColor.Red);
                }
            }
        }

        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            Log.WriteLine("MQTT receive: " + e.Topic + "; " + Encoding.ASCII.GetString(e.Message));
            String _topic = e.Topic;
            String _mac = "";
            //String _nodesmessage;
            String _nodesobj;
            if (_topic.StartsWith(MQTTenum.IoTtopic))
            {
                //powiadomienie z nody
                _mac = _topic.Replace(MQTTenum.IoTtopic+"/", "");
                _nodesobj = _mac.Substring(18);
                _mac = _mac.Substring(0, 17);

                Controller.Instance.MqttIotDeviceNotify(_mac, _nodesobj, e.Message, false);
            }
            else if (_topic.StartsWith(MQTTenum.IoTserviceTopic+ "/up/"))
            {
                _mac = _topic.Replace(MQTTenum.IoTserviceTopic +"/up/", "");
                _mac = _mac.Substring(0, 17);
                Controller.Instance.IotDeviceServiceEvent(_mac, e.Message, false);
            }
            else
            {
                Log.WriteLine("Received = " + Encoding.UTF8.GetString(e.Message) + " on topic " + e.Topic);
            }
        }

        //CallBack
        private static void recv(IAsyncResult res)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 5000);
            byte[] received = ClientUDP.EndReceive(res, ref RemoteIpEndPoint);

            //Process codes
            Log.Write("UDP data rec: ");
            Log.WriteLine(Encoding.ASCII.GetString(received));
            Log.WriteLine("From: " + RemoteIpEndPoint.Address.ToString());

            String rec = Encoding.ASCII.GetString(received);
            if (rec.StartsWith("BASACCentralSearch", StringComparison.Ordinal))
            {
                var ips = RemoteIpEndPoint.ToString();


                byte[] send_buffer = System.Text.Encoding.ASCII.GetBytes("BasacCentalInfo;" + Utils.AppDescription + ";" +
                                                                         Utils.MQQTTport + ";" + 
                                                                         ";mdns=" + Utils.getDNSname());
                ClientUDP.SendAsync(send_buffer, send_buffer.Length, ips.Split(':')[0], Utils.UDPport);
            }
            else if (rec.StartsWith("BAS;", StringComparison.Ordinal))
            {
                var parts = rec.Split('#');
                if (parts[0].Length > 1)
                {
                    var inf = parts[0].Split(';');
                    string _mac = "";
                    string _devmodel = "";
                    foreach (var item in inf)
                    {
                        if (item.StartsWith("mac="))
                        {
                            _mac = item.Substring(4);
                        }
                        if (item.StartsWith("name"))
                        {
                            if (item.StartsWith("name"))
                            {
                                var xx = item.Replace("name=", "");
                                if (xx.StartsWith("BWX"))
                                    _devmodel = xx;
                            }
                        }
                    }
                    if (!Controller.Instance.NewIotDeviceMACexists(_mac) && !Controller.Instance.IotDeviceMACexists(_mac))
                    {
                        var scanneddevice = new IoTDevice();
                        scanneddevice.ID = 0;
                        scanneddevice.MAC = _mac;
                        scanneddevice.DeviceModel = _devmodel;
                        List<IoTregister> _registers = new List<IoTregister>();
                        IoTregister x = new IoTregister();
                        for (int i = 1; i < parts.Length; i++)
                        {
                            //dodawanie poszczególnych obiektów

                            var oi = parts[i].Replace("\0", "").Split(';');
                            int number;
                            bool success = Int32.TryParse(oi[0], out number);
                            if (!(number > 0)) success = false;
                            if (oi.Length > 6 && success)
                            {
                                x.Id = Int32.Parse(oi[0]);
                                x.Name = oi[1];
                                x.Description = oi[2];
                                x.MAC = _mac;
                            }
                            _registers.Add(x);
                            x = new IoTregister();
                        }
                        _registers[0].Name = "DEV";
                        scanneddevice.Registers = _registers;
                        Controller.Instance.AddDeviceToNewDeviceList(scanneddevice);
                    }
                }
                Log.WriteLine("#");
            }
            ClientUDP.BeginReceive(new AsyncCallback(recv), null);
        }

        static void SendUdp(int srcPort, string dstIp, int dstPort, byte[] data)
        {
            using (UdpClient c = new UdpClient(srcPort))
                c.Send(data, data.Length, dstIp, dstPort);
        }

        public static void UDPsend(String message)
        {
            //byte[] x = {0x31, 0x32};
            byte[] send_buffer = System.Text.Encoding.ASCII.GetBytes(message);
            string ips = Utils.GetLocalIPAddress();
            var ip = ips.Split('.');
            ips = ip[0] + "." + ip[1] + "." + ip[2] + ".255";
            //if (ClientUDP.)
            ClientUDP.Send(send_buffer, send_buffer.Length, "255.255.255.255", Utils.UDPport);
            ClientUDP.Send(send_buffer, send_buffer.Length, "192.168.100.100", Utils.UDPport);
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            newExc.ToLogUnhandledException();
        }
        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            newExc.ToLogUnhandledException();
        }
    }
}
