namespace RondasEcopetrolWPF.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    public class Rondas_Descargadas
    {
        [XmlElement("Ronda")]
        public List<RondaDescargada> Rondas = new List<RondaDescargada>();

        public IEnumerator<RondaDescargada> GetEnumerator()
        {
            foreach (RondaDescargada ronda in Rondas)
                yield return ronda;
        }
    }
}
