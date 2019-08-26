using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RondasEcopetrol.Models
{
    class SuspendRound
    {
        private static Hashtable suspendRounds;

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
                if (suspendRounds.Count > 0)
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

        public static void saveSuspends()
        {
            //TODO
        }

        public static void loadSuspends()
        {
            //TODO
        }
    }
}
