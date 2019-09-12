using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RondasEcopetrolWPF.Models
{
    class SuspendRound
    {
        private static Hashtable suspendRounds;

        private static bool loaded = false;
        private const string CACHE_FILENAME = @"suspendRounds.dat";

        public static IEnumerable<Rondas> getSuspendRoundsList()
        {
            if (suspendRounds == null) return new List<Rondas>();
            return suspendRounds.Values.Cast<Rondas>();
        }

        public static int getSuspendRoundCount()
        {
            if (suspendRounds == null) return 0;
            return suspendRounds.Count;
        }

        public static void addSuspendRound(Rondas ronda)
        {
            if (suspendRounds == null) suspendRounds = new Hashtable();
            if (!suspendRounds.ContainsKey(ronda.MessageID))
            {
                suspendRounds.Add(ronda.MessageID, ronda);
                //Actualizar el archivo de la cache
                SaveSuspends();
            }
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
                //Actualizar el archivo de la cache
                SaveSuspends();
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
                else
                {
                    //Borrar el archivo de la cache
                    File.Delete(CACHE_FILENAME);
                }
            }
            catch (Exception)
            {
            }
        }

        public static void LoadSuspends()
        {
            if (!loaded)
            {
                try
                {
                    if (File.Exists(CACHE_FILENAME))
                    {
                        DeserializeData();
                    }
                }
                catch (Exception)
                {
                }
                //Actualizar la bandera
                loaded = true;
            }
        }

        private static void SerializeData()
        {
            //Guardar el HashTasble en un archivo binario (.dat)
            Stream SaveFileStream = File.Create(CACHE_FILENAME);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(SaveFileStream, suspendRounds);
            SaveFileStream.Close();
        }

        private static void DeserializeData()
        {
            //Cargar el HashTasble desde el archivo binario (.dat)
            Stream openFileStream = File.OpenRead(CACHE_FILENAME);
            BinaryFormatter deserializer = new BinaryFormatter();
            suspendRounds = (Hashtable)deserializer.Deserialize(openFileStream);
            openFileStream.Close();
        }
    }
}
