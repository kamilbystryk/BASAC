using System;
using System.IO;
using System.Reflection;

namespace BASAC.Util
{
    public class Log
    {
        private static string _FilePath = System.IO.Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString()).ToString() + "/Log";//Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        private static string _FileName;
        private EnhancedSerialPort port;
        String logUartPort = "/dev/ttyACM11";
        private static Log _instance;
        private static readonly object Lock = new object();
        public static bool DetailLog = true;
        public static Log Instance
        {
            get
            {
                lock (Lock)
                {
                    if (_instance == null)
                        _instance = new Log();
                }
                return _instance;
            }
        }
        public Log()
        {
            //_FilePath = System.IO.Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString()).ToString();
            _FilePath = System.IO.Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).ToString()).ToString() + "/Log";
            if (!Directory.Exists(_FilePath))
            {
                Directory.CreateDirectory(_FilePath);
            }
            //_FilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        public void OpenPort()
        {

            if (port != null)
                if (port.IsOpen)
                    port.Close();


            try
            {
                port = new EnhancedSerialPort(logUartPort, 115200);
                //port.DataReceived += HandlePortDataReceived;
                port.ReadTimeout = 400;
                port.ReadBufferSize = 4096;
                port.Open();
            }
            catch
            {
                Log.WriteLine("LOG serial port open ERROR (" + logUartPort + ")");
            }
        }

        public static void WriteLine(String line)
        {
            var now = DateTime.Now;
            String timestamp = now.ToString("yyyy.MM.dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            Console.WriteLine(timestamp + " # " + line);
            if (DetailLog)
            {
                _FileName = "log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                var path = Path.Combine(_FilePath, _FileName);
                try
                {
                    File.AppendAllText(path, timestamp + " # " + line + "\n");
                }
                catch { }
            }
        }

        public static void Write(String line)
        {
            var now = DateTime.Now;
            String timestamp = now.ToString("yyyy.MM.dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            Console.Write(timestamp + " # " + line);
            if (DetailLog)
            {
                _FileName = "log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                var path = Path.Combine(_FilePath, _FileName);
                try
                {
                    File.AppendAllText(path, timestamp + " # " + line + "\n");
                }
                catch { }
            }
        }

        public static void WriteLine(String line, ConsoleColor Color)
        {
            var now = DateTime.Now;
            String timestamp = now.ToString("yyyy.MM.dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            Console.ForegroundColor = Color;
            Console.WriteLine(timestamp + " # " + line);
            Console.ForegroundColor = ConsoleColor.White;
            if (DetailLog)
            {
                _FileName = "log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                var path = Path.Combine(_FilePath, _FileName);
                try
                {
                    File.AppendAllText(path, timestamp + " # " + line + "\n");
                }
                catch { }
            }
        }

        public void SerialWrite(String line)
        {
            if (port != null && port.IsOpen)
                port.Write(line);
        }
        public void SerialWriteLn(String line)
        {
            line += '\n';
            if (port != null && port.IsOpen)
                port.Write(line);
        }

        /*public void WriteLine(String line) {
            String timestamp = DateTime.Now;
            Console.WriteLine (timestamp + " ## " + line); 
        }*/
    }
}
