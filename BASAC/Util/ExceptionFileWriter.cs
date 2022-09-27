using System;
using System.IO;
using System.Reflection;

namespace BASAC.Util
{
    public static class ExceptionFileWriter
    {
        static string FilePath
        {
            get
            {
                string _FilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var _FileName = "Fatal.txt";
                var path = Path.Combine(_FilePath, _FileName);
                return path;
            }
        }

        public static void ToLogUnhandledException(this Exception exception)
        {
            try
            {
                string ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                String errorMessage = "BASAC ver." + ver;
                Console.WriteLine("###################################");
                Console.WriteLine(errorMessage);
                errorMessage += String.Format("Time: {0}\r\nError: Unhandled Exception\r\n{1}\n\n", DateTime.Now, string.IsNullOrEmpty(exception.StackTrace) ? exception.ToString() : exception.StackTrace);
                File.WriteAllText(FilePath, errorMessage);
                /*MailMessage mail = new MailMessage("example@gmail.com", "example@gmail.com, "BASAC v" + ver + " unhandled exception", errorMessage);
                SmtpClient client = new SmtpClient();
                client.Host = ("gmail.com");
                client.Port = 587; //smtp port for SSL
                client.Credentials = new System.Net.NetworkCredential("example@gmail.com", "pass");
                client.EnableSsl = false; //for gmail SSL must be true
                client.Send(mail);*/
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}