namespace RondasEcopetrolWPF.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    public class Rondas_Disponibles
    {
        [XmlElement("Ronda")]
        public List<Ronda> Rondas = new List<Ronda>();

        public IEnumerator<Ronda> GetEnumerator()
        {
            foreach (var ronda in Rondas)
                yield return ronda;
        }
    }
}
