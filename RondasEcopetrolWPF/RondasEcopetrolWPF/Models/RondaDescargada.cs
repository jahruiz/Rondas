using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RondasEcopetrolWPF.Models
{
    public class RondaDescargada
    {
        private string _id_Ronda;
        private string _nombre_Ronda;
        private string _nombre_Puesto;
        private string _nombre_Planta;
        private string _fecha_Gen;
        private string _hora_Gen;
        private string _message_id;

        private string _usuario;

        public string Ronda_ID
        {
            get
            {
                return _id_Ronda;
            }
            set
            {
                if (_id_Ronda != value)
                {
                    _id_Ronda = value;
                }
            }
        }
        public string Nombre
        {
            get
            {
                return _nombre_Ronda;
            }
            set
            {
                if (_nombre_Ronda != value)
                {
                    _nombre_Ronda = value;
                }
            }
        }
        public string Planta
        {
            get
            {
                return _nombre_Planta;
            }
            set
            {
                if (_nombre_Planta != value)
                {
                    _nombre_Planta = value;
                }
            }
        }
        public string Puesto
        {
            get
            {
                return _nombre_Puesto;
            }
            set
            {
                if (_nombre_Puesto != value)
                {
                    _nombre_Puesto = value;
                }
            }
        }
        public string Fecha_Gen
        {
            get
            {
                return _fecha_Gen;
            }
            set
            {
                if (_fecha_Gen != value)
                {
                    _fecha_Gen = value;
                }
            }
        }
        public string Hora_Gen
        {
            get
            {
                return _hora_Gen;
            }
            set
            {
                if (_hora_Gen != value)
                {
                    _hora_Gen = value;
                }
            }
        }
        public string Message_ID
        {
            get
            {
                return _message_id;
            }
            set
            {
                if (_message_id != value)
                {
                    _message_id = value;
                }
            }
        }

        public string Usuario
        {
            get
            {
                return _usuario;
            }
            set
            {
                if (_usuario != value)
                {
                    _usuario = value;
                }
            }
        }
    }
}
