namespace RondasEcopetrolWPF.ServerUtils
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Net;
    public class ServerUtils
    {
        private static string backupHost = null;

        private static string contentType;

        private static string host;

        private static WebResponse response;

        private static Stream stream;

        static ServerUtils()
        {
            ServerUtils.host = "";
            ServerUtils.contentType = "";
        }
        public static void close()
        {
            if (ServerUtils.stream != null)
            {
                ServerUtils.stream.Close();
                ServerUtils.stream = null;
            }
            if (ServerUtils.response != null)
            {
                ServerUtils.response.Close();
                ServerUtils.response = null;
            }
        }
        public static string getContentType()
        {
            return ServerUtils.contentType;
        }
        public static string getMIME()
        {
            return ServerUtils.contentType;
        }
        public static Stream getStream()
        {
            Stream result;
            if (ServerUtils.response != null)
            {
                result = (ServerUtils.stream = ServerUtils.response.GetResponseStream());
            }
            else
            {
                //MessageBox.Show("No hay flujo de datos de parte del servidor", "Error");
                result = null;
            }
            return result;
        }
        public static string initServer()
        {
            /*XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(FileUtils.getConfigPath() + "/config.xml");
            XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlDocument);
            xmlNodeReader.MoveToContent();
            xmlNodeReader.MoveToAttribute(0);
            ServerUtils.host = xmlNodeReader.Value;
            string result = "";
            int attributeCount = xmlNodeReader.AttributeCount;
            for (int i = 1; i < attributeCount; i++)
            {
                xmlNodeReader.MoveToNextAttribute();
                string name = xmlNodeReader.Name;
                if (name.Equals("tag"))
                {
                    result = xmlNodeReader.Value;
                }
                else
                {
                    ServerUtils.backupHost = xmlNodeReader.Value;
                }
            }
            xmlNodeReader.Close();*/
            ServerUtils.host = ConfigurationManager.AppSettings["host"].ToString();
            string result = ConfigurationManager.AppSettings["tag"].ToString();
            return result;
        }
        public static bool isMIME(string mime)
        {
            return ServerUtils.contentType.IndexOf(mime) != -1;
        }
        public static bool send(string uri, string parameters)
        {
            string[] array = new string[]
            {
                "http://",
                ServerUtils.host,
                uri,
                "?",
                parameters
            };
            string[] array2 = array;
            bool flag;
            bool result;
            try
            {
                WebRequest webRequest = WebRequest.Create(string.Concat(array2));
                webRequest.Timeout = 30000;
                ServerUtils.response = webRequest.GetResponse();
                ServerUtils.contentType = ServerUtils.response.ContentType;
                flag = true;
            }
            catch (Exception e)
            {
                if (ServerUtils.response != null)
                {
                    ServerUtils.response.Close();
                }
                array2[1] = ServerUtils.backupHost;
                if (ServerUtils.backupHost != null)
                {
                    result = ServerUtils.sendBackup(string.Concat(array2), parameters);
                    return result;
                }
                flag = false;
                LogError.CustomErrorLog(e);
            }
            result = flag;
            return result;
        }
        public static bool send(string uri, string mime, byte[] bytes)
        {
            bool flag;
            bool result;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + ServerUtils.host + uri);
                httpWebRequest.ContentType = mime;
                httpWebRequest.SendChunked = true;
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = (long)bytes.Length;
                httpWebRequest.Timeout = 30000;
                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                ServerUtils.response = httpWebRequest.GetResponse();
                flag = true;
            }
            catch (Exception e)
            {
                if (ServerUtils.response != null)
                {
                    ServerUtils.response.Close();
                }
                if (ServerUtils.backupHost != null)
                {
                    result = ServerUtils.sendBackup(uri, mime, bytes);
                    return result;
                }
                flag = false;
                LogError.CustomErrorLog(e);
            }
            result = flag;
            return result;
        }
        public static bool sendBackup(string uri, string parameters)
        {
            bool result;
            try
            {
                WebRequest webRequest = WebRequest.Create(uri);
                webRequest.Timeout = 30000;
                ServerUtils.response = webRequest.GetResponse();
                ServerUtils.contentType = ServerUtils.response.ContentType;
                result = true;
            }
            catch (Exception e)
            {
                if (ServerUtils.response != null)
                {
                    ServerUtils.response.Close();
                }
                result = false;
                LogError.CustomErrorLog(e);
            }
            return result;
        }

        public static bool sendBackup(string uri, string mime, byte[] bytes)
        {
            bool result;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + ServerUtils.backupHost + uri);
                httpWebRequest.ContentType = mime;
                httpWebRequest.SendChunked = true;
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = (long)bytes.Length;
                httpWebRequest.Timeout = 30000;
                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                ServerUtils.response = httpWebRequest.GetResponse();
                result = true;
            }
            catch (Exception e)
            {
                if (ServerUtils.response != null)
                {
                    ServerUtils.response.Close();
                }
                result = false;
                LogError.CustomErrorLog(e);
            }
            return result;
        }
    }
}
