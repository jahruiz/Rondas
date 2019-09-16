using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RondasEcopetrolWPF.ServerUtils
{
    public class LogError
    {
        private static StreamWriter sw = null;
        public LogError() { }

        public static void CustomErrorLog(Exception objException)
        {
            string strLogFilePath = string.Empty;
            string FileNameLog = string.Empty;
            try
            {
                strLogFilePath = initPath();

                if (!string.IsNullOrEmpty(strLogFilePath))
                {
                    if(LogError.CheckDirectory(strLogFilePath))
                    {
                        FileNameLog = GetLogFilePath(strLogFilePath);
                        if(!string.IsNullOrEmpty(FileNameLog))
                        {
                            WriteErrorLog(FileNameLog, objException);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        public static string initPath()
        {
            string FilePath = string.Empty;
            FilePath = ConfigurationManager.AppSettings["LogPath"].ToString();

            if (FilePath.Trim().Length == 0)
                FilePath = Directory.GetCurrentDirectory();

            return FilePath;
        } 
        private static bool CheckDirectory(string strLogPath)
        {
            try
            {
                if (!Directory.Exists(strLogPath))
                    Directory.CreateDirectory(strLogPath);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private static void WriteErrorLog(string strPathName, Exception objException)
        {
            string strException = string.Empty;
            try
            {
                sw = new StreamWriter(strPathName, true);
                sw.WriteLine("Source		: " + objException.Source.ToString().Trim());
                sw.WriteLine("Method		: " + objException.TargetSite.Name.ToString());
                sw.WriteLine("Date		: " + DateTime.Now.ToLongTimeString());
                sw.WriteLine("Time		: " + DateTime.Now.ToShortDateString());
                sw.WriteLine("Error		: " + objException.Message.ToString().Trim());
                sw.WriteLine("Stack Trace	: " + objException.StackTrace.ToString().Trim());
                sw.WriteLine("^^-------------------------------------------------------------------^^");
                sw.Flush();
                sw.Close();
            }
            catch (Exception)
            {
            }
        }
        private static string GetLogFilePath(string path)
        {
            try
            {
                string FileNameLog = "\\Log_" + DateTime.Now.ToString("dd_MM_yyyy") + ".txt";
                
                string retFilePath = path + FileNameLog;

                if (File.Exists(retFilePath))
                    return retFilePath;
                else
                {
                    FileStream fs = new FileStream(retFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs.Close();
                }
                return retFilePath;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }        
    }
}
