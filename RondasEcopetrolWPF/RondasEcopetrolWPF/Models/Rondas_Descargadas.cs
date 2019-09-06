namespace RondasEcopetrolWPF.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    public class Rondas_Descargadas
    {
        [XmlElement("Ronda")]
        public List<RondaDescargada> Rondas = new List<RondaDescargada>();

        [XmlElement("Step")]
        public List<Step> Steps = new List<Step>();

        public IEnumerator<RondaDescargada> GetEnumerator()
        {
            foreach (RondaDescargada ronda in Rondas)
                yield return ronda;
        }
    }
}
