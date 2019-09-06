using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RondasEcopetrolWPF.Models
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class Rondas_Completadas
    {
        [XmlElement("Ronda")]
        public List<RondaCompletada> Rondas = new List<RondaCompletada>();

        public IEnumerator<RondaCompletada> GetEnumerator()
        {
            foreach (RondaCompletada ronda in Rondas)
                yield return ronda;
        }
    }
}
