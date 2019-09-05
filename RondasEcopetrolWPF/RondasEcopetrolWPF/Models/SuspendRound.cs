using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RondasEcopetrolWPF.Models
{
    class SuspendRound
    {
        private static Hashtable suspendRounds;
        private const string CACHE_FILENAME = @"suspendRounds.xml";

        public static IEnumerable<Rondas> getSuspendRoundsList()
        {
            if (suspendRounds == null) return new List<Rondas>();
            return suspendRounds.Values.Cast<Rondas>();
        }
        public static void addSuspendRound(Rondas ronda)
        {
            if (suspendRounds == null) suspendRounds = new Hashtable();
            if (!suspendRounds.ContainsKey(ronda.MessageID))
                suspendRounds.Add(ronda.MessageID, ronda);
        }

        public static bool isRoundSuspend(object MessageID)
        {
            if (suspendRounds == null) return false;
            return suspendRounds.Contains(MessageID);
        }

        public static Rondas getSuspendRound(object messageId)
        {
            if (suspendRounds == null) return null;
            return (Rondas)suspendRounds[messageId];
        }

        public static void deleteSuspend(object MessageID)
        {
            Rondas rondas = getSuspendRound(MessageID);
            if (rondas != null)
            {
                rondas.Lector.Close();
                suspendRounds.Remove(MessageID);
            }
        }

        public static void closeSuspends()
        {
            try
            {
                if (suspendRounds != null && suspendRounds.Count > 0)
                {
                    foreach (Rondas r in suspendRounds.Values)
                    {
                        r.Lector.Close();
                    }
                    //suspendRounds.Clear();
                }
            }
            catch (Exception)
            {
            }
        }

        public static void SaveSuspends()
        {
            try
            {
                if (suspendRounds != null && suspendRounds.Count > 0)
                {
                    SerializeData();
                }
            }
            catch (Exception e)
            {
            }
        }

        public static void LoadSuspends()
        {
            try
            {
                if (File.Exists(CACHE_FILENAME))
                {
                    DeserializeData();
                }
            }
            catch (Exception e)
            {
            }
        }

        private static void SerializeData()
        {
            //Colocar las rondas en un objeto lista temporal
            List<Rondas> tempdataitems = new List<Rondas>(suspendRounds.Count);
            foreach (Rondas ronda in suspendRounds.Values)
            {
                ronda.Lector.Close();
                ronda.Lector = null;
                //Agregar a la lista temporal
                tempdataitems.Add(ronda);
            }

            //Serializar la lista en un archivo XML
            XmlSerializer serializer = new XmlSerializer(typeof(List<Rondas>));
            TextWriter textWriter = new StreamWriter(CACHE_FILENAME);
            serializer.Serialize(textWriter, tempdataitems);
            textWriter.Close();
        }

        private static void DeserializeData()
        {
            //Inicializar la cache
            suspendRounds = new Hashtable();
            
            //Leer la lista de Rondas del archivo XML
            XmlSerializer serializer = new XmlSerializer(typeof(List<Rondas>));
            TextReader textReader = new StreamReader(CACHE_FILENAME);
            List<Rondas> tempdataitems = (List<Rondas>)serializer.Deserialize(textReader);
            textReader.Close();

            //Agregar las rondas a la cache
            foreach (Rondas ronda in tempdataitems)
            {
                addSuspendRound(ronda);
            }
        }
    }
}
