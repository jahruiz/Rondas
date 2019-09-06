using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RondasEcopetrolWPF.Models
{
    public class RondaCompletada
    {
        private string _id_Ronda;
        private string _nombre_Ronda;
        private string _message_date;
        private string _message_id;
        private string _comentary;

        private string _usuario;

        public string id
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
        public string name
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
        public string message_date
        {
            get
            {
                return _message_date;
            }
            set
            {
                if (_message_date != value)
                {
                    _message_date = value;
                }
            }
        }
        public string message_id
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

        public string User_ID
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

        public string Comentary
        {
            get
            {
                return _comentary;
            }
            set
            {
                if (_comentary != value)
                {
                    _comentary = value;
                }
            }
        }

        public int Pasos
        {
            get
            {
                return 0;
            }
        }

        //Propiedades ReadOnly creadas para compatibilidad con la View de Enviar Ronda
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

        public string Fecha_Gen
        {
            get
            {
                if (_message_date == null) return null;
                else return _message_date.Substring(0, 10);
            }
        }
        public string Hora_Gen
        {
            get
            {
                if (_message_date == null) return null;
                else return _message_date.Substring(11);
            }
        }

    }
}
