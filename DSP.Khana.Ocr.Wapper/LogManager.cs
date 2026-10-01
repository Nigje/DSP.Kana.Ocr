using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bina.Ocr.Wapper
{
    public class LogManager
    {
        public static void Log(string message)
        {
            if (false)
            {
                string _path = AppDomain.CurrentDomain.BaseDirectory;
                int conter = 1;
                while (true && conter < 1000)
                {
                    try
                    {
                        if (!Directory.Exists(_path))
                            Directory.CreateDirectory(_path);

                        File.AppendAllText(_path + "Log.txt", DateTime.Now + " ====> " + message + "\r\n");
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("it is being used by another process"))
                        {
                            conter++;
                            Task.Delay(50);
                        }
                        else throw ex;
                    }
                }
            }


        }
        public static void Log(Exception exception,string lable="")
        {
            Log(lable+exception.Message + "\r\n" + exception.StackTrace);
        }
    }
}
