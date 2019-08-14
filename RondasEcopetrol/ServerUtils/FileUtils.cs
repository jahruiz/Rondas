namespace RondasEcopetrol.ServerUtils
{
    using System.Data;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using Windows.Storage;

    using System.Threading.Tasks;
    using System.Runtime;
    using System;

    using System.Collections.Generic;

    public class FileUtils
    {
        private static string path;
        private static string configPath;

        private static string user;
        private static string pwd;

        static FileUtils()
        {
            FileUtils.user = null;
            FileUtils.pwd = null;
            FileUtils.path = null;
            FileUtils.configPath = null;
        }
        public static void configure_user(string user, string pwd)
        {
            FileUtils.user = user;
            FileUtils.pwd = pwd;
        }
        public static async void createUser(string name)
        {
            if (FileUtils.path == null)
            {
                FileUtils.initPath();
            }
            FileUtils.user = name;
            /*if (!Directory.Exists(FileUtils.path + "/" + name))
            {
                Directory.CreateDirectory(FileUtils.path + "/" + name);
            }*/
            StorageFolder rondasFolder = await StorageFolder.GetFolderFromPathAsync(path);
            StorageFolder userFolder = await rondasFolder.CreateFolderAsync(name, CreationCollisionOption.OpenIfExists);
            //StorageFolder rondasFolder = ApplicationData.Current.LocalFolder;
            //StorageFolder userFolder = await rondasFolder.CreateFolderAsync(name, CreationCollisionOption.OpenIfExists);
        }
        public static void deleteUser()
        {
            string[] files = Directory.GetFiles(FileUtils.path + "/" + FileUtils.user);
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
            Directory.Delete(FileUtils.path + "/" + FileUtils.user);
        }
        public static string getActualUser()
        {
            return FileUtils.user;
        }
        public static string getActualUserpwd()
        {
            return FileUtils.pwd;
        }

        public static string getConfigPath()
        {
            if (FileUtils.configPath == null)
            {
                FileUtils.configPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            }
            return FileUtils.configPath;
        }
        public static byte[] getUTF8BytesFromXml(string filename)
        {
            string[] array = new string[]
            {
                FileUtils.path,
                "/",
                FileUtils.user,
                "/",
                filename
            };
            string[] array2 = array;
            string text = string.Concat(array2);
            StreamReader streamReader = new StreamReader(text);
            string text2 = streamReader.ReadToEnd();
            streamReader.Close();
            UTF8Encoding uTF8Encoding = new UTF8Encoding();
            return uTF8Encoding.GetBytes(text2);
        }
        public static void initPath()
        {
            //FileUtils.path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            FileUtils.path = ApplicationData.Current.LocalFolder.Path;
        }
        public static DataSet loadData(string Filename)
        {
            DataSet dataSet = new DataSet();
            string[] array = new string[]
            {
                FileUtils.path,
                "/",
                FileUtils.user,
                "/",
                Filename
            };
            string[] array2 = array;
            dataSet.ReadXml(string.Concat(array2));
            return dataSet;
        }
        public static DataSet loadDataFromStream(Stream stream)
        {
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(new XmlTextReader(stream));
            return dataSet;
        }
        public static string loadDataFromStreamS(Stream stream)
        {
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(new XmlTextReader(stream));
            string datasetString = dataSet.GetXml();
            return datasetString;
        }
        public static DataSet loadSchema(string file)
        {
            DataSet dataSet = new DataSet();
            string[] array = new string[]
            {
                FileUtils.path,
                "/",
                FileUtils.user,
                "/",
                file
            };
            string[] array2 = array;
            dataSet.ReadXmlSchema(string.Concat(array2));
            return dataSet;
        }
        public static DataSet loadSchemaFromRoot(string file)
        {
            DataSet dataSet = new DataSet();
            dataSet.ReadXmlSchema(FileUtils.path + "/" + file);
            return dataSet;
        }

        //public static XmlReader loadXMLFromUser(string file) recomendado XmlReader en vez de XmlTextReader
        public static XmlTextReader loadXMLFromUser(string file)
        {
            string[] array = new string[]
            {
                FileUtils.path,
                "/",
                FileUtils.user,
                "/",
                file
            };
            string[] array2 = array;
            string text = string.Concat(array2);
            return new XmlTextReader(new FileStream(text, FileMode.Open));
        }
        public static void saveData(DataSet dset, string fileName)
        {
            string[] array = new string[]
            {
                FileUtils.path,
                "/",
                FileUtils.user,
                "/",
                fileName
            };
            string[] array2 = array;
            dset.WriteXml(string.Concat(array2));
        }
        public static void writeXmlData(string filename, Stream xmlsource)
        {
            string[] array = new string[]
            {
                FileUtils.path,
                "/",
                FileUtils.user,
                "/",
                filename
            };
            string[] array2 = array;
            string text = string.Concat(array2);
            StreamReader streamReader = new StreamReader(xmlsource);
            FileStream fileStream = new FileStream(text, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            streamWriter.Write(streamReader.ReadToEnd());
            streamWriter.Close();
            fileStream.Close();
        }
        public static void writeXmlData(string filename, string xmlsource)
        {
            string[] array = new string[]
            {
                FileUtils.path,
                "/",
                FileUtils.user,
                "/",
                filename
            };
            string[] array2 = array;
            string text = string.Concat(array2);
            FileStream fileStream = new FileStream(text, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
            streamWriter.Write(xmlsource);
            streamWriter.Close();
            fileStream.Close();
        }
        /// <summary>
        /// populate a class with xml data 
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="input">xml data</param>
        /// <returns>Object Type</returns>
        public static T Deserialize<T>(string input) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        /// <summary>
        /// convert object to xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ObjectToSerialize"></param>
        /// <returns></returns>
        public static string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }

        public static async Task<List<string>> GetUsuariosRondasDescargadasAsync()
        {
            if (FileUtils.path == null)
            {
                FileUtils.initPath();
            }

            StorageFolder rondasFolder = await StorageFolder.GetFolderFromPathAsync(path);

            List<String> users = new List<String>();
            foreach (var folder in await rondasFolder.GetFoldersAsync())
            {
                users.Add(folder.Name);
            }
            return users;
        }

        public static async Task<List<StorageFile>> GetArchivosRondasDescargadasAsync(string usuario)
        {
            if (FileUtils.path == null)
            {
                FileUtils.initPath();
            }

            StorageFolder rondasFolder = await StorageFolder.GetFolderFromPathAsync(path + "\\" + usuario);

            //return (await rondasFolder.GetFilesAsync());
            List<StorageFile> files = new List<StorageFile>();
            foreach (var file in await rondasFolder.GetFilesAsync())
            {
                files.Add(file);
            }
            return files;
        }

        public static async Task<string> GetXmlRondaAsync(StorageFile fileRonda)
        {
            string xmlRonda = await Windows.Storage.FileIO.ReadTextAsync(fileRonda);
            xmlRonda = xmlRonda.Replace("RondasHHT", "Rondas_Descargadas");
            return xmlRonda;
        }
    }
}
