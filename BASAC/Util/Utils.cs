using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace BASAC
{
    public class Utils
    {
        //app
        public static String AppVer = "1.0.1.1";
        public static String AppDescription = "default despription";
        public static String MAC = "DE:AD:DE:AF";
        public static bool ServiceStackError = true;
        public static bool appUpdateReady = false;
        public static bool appUpdateReadyToInstall = false;


        public static String appName = "BASAC";
        public static String dnsname = "basac";
        public static bool ServiceConsole = false;
        public static int UDPport = 8888;

        //MQTT
        public static bool MQTTserver_auth = false;
        public static String MQTTpass = "";
        public static String MQTTlogin = "";
        public static int MQQTTport = 1883;

        //ACQ
        public static String AcqNumberFormat = "D5";

        //devices
        public static int deviceTimeout = 60;
        public static bool deviceTimeoutActive = true;

        //general purpose

        public static DateTime StartTime;

        public static String getDNSname()
        {
            const Int32 BufferSize = 128;
            string ret = "none";
            try
            {
                using (var fileStream = File.OpenRead("/etc/avahi/avahi-daemon.conf"))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                {
                    String line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (line.StartsWith("host-name="))
                        {
                            line = line.Replace("host-name=", "");
                            ret = line;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return ret;
        }

        public static void PrepareMAC()
        {

            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            //Console.WriteLine("Interface information for {0}.{1}     ",
            //      computerProperties.HostName, computerProperties.DomainName);

            if (nics == null || nics.Length < 1)
            {
                //Console.WriteLine("  No network interfaces found.");
                return;
            }
            PhysicalAddress address = nics[nics.Length - 1].GetPhysicalAddress();
            foreach (var item in nics)
            {
                if (item.NetworkInterfaceType.ToString().ToLower().StartsWith("eth"))
                {
                    address = item.GetPhysicalAddress();
                }
            }
            var MACt = address.ToString();
            var MACt2 = "";
            for (int i = 0; i < MACt.Length; i += 2)
            {
                if (i > 0) MACt2 += ":";
                MACt2 += MACt.Substring(i, 2);

            }
            MAC = MACt2;
            Console.Write("MAC: ");
            Console.WriteLine(MAC);
            /*byte[] bytes = address.GetAddressBytes();
            for (int i = 0; i < bytes.Length; i++)
            {
                // Display the physical address in hexadecimal.
                Console.Write("MAC: ");
                //Console.Write("{0}", bytes[i].ToString("X2"));
                MAC =  String.Format("{0}", bytes[i].ToString("X2"));
                Console.WriteLine(MAC);
                // Insert a hyphen after each byte, unless we are at the end of the
                // address.
                if (i != bytes.Length - 1)
                {
                    Console.Write("-");
                }
                Console.WriteLine("");
            }*/
        }

        public static string GetLocalIPAddress()
        {
            string ret = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ret = ip.ToString();
                }
            }
            return ret;
            //throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        public static String DTString(DateTime dt)
        {
            return (dt.Year + "-" + dt.Month.ToString("D2") + "-" + dt.Day.ToString("D2") + " " + dt.Hour.ToString("D2") + ":" + dt.Minute.ToString("D2") + ":" + dt.Second.ToString("D2"));
        }

        public static String GetUpTime()
        {
            string str = "";
            var uptimeText = File.ReadAllText("/proc/uptime");
            var ut = uptimeText.Split(' ');
            var ut2 = ut[0].Split('.');
            Int64 seconds = Int64.Parse(ut2[0]);
            TimeSpan span = new TimeSpan(TimeSpan.TicksPerSecond * seconds);
            str = span.Hours.ToString("00") + ":" + span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
            return str;
        }
    }
}
