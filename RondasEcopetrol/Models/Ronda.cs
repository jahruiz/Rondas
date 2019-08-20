namespace RondasEcopetrol.Models
{
    public class Ronda 
    {
        private string _id_Ronda;
        private string _nombre_Ronda;
        private string _nombre_Puesto;
        private string _nombre_Planta;
        private string _fecha_Gen;
        private string _hora_Gen;
        private string _message_id;

        public string ID_Ronda
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
        public string Nombre_Ronda
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
        public string Nombre_Planta
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
        public string Nombre_Puesto
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
                return _fecha_Gen;
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
    }
}
