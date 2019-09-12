namespace RondasEcopetrolWPF.Models
{
    using System;
    using System.Collections;
    using System.Windows.Controls;

    [Serializable()]
    public class Rondas
    {
        // Methods
        public Rondas(string[] row)
        {
            this.id = null;
            this.nombre = null;
            this.states = null;
            this.show_tree = false;
            this.complete = false;
            this.puesto = null;
            this.planta = null;
            this.fecha = null;
            this.hora = null;
            this.steps = new ArrayList();
            this.currentObj = null;
            this.id = row[0];
            this.nombre = row[1];
            char[] chArray1 = new char[1] { ',' };
            this.states = row[7].Trim().Split(chArray1);
            this.message_id = row[8].Trim();
            this.suspend = false;
            if (row[6].Equals("NO"))
            {
                this.show_tree = false;
            }
            else
            {
                this.show_tree = true;
            }
            this.puesto = row[2];
            this.planta = row[3];
            this.fecha = row[4];
            this.hora = row[5];

            this.pocketdate = DateTime.ParseExact(row[10], "yyyy-MM-dd HH:mm:ss", null);
            this.serverdate = DateTime.ParseExact(row[9], "yyyy-MM-dd HH:mm:ss", null);
            /*if (pocketdate > serverdate)
                this.diff = pocketdate - serverdate;
            else*/
            this.diff = serverdate - pocketdate;
            this.steps_table = new Hashtable();
        }

        public DateTime getRealDate()
        {
            DateTime time1 = DateTime.Now; //fechaPocket            
            return time1 + diff;
        }

        public void addStep(Steps step)
        {
            if (this.steps.Count > 0)
            {
                Steps steps1 = (Steps)this.steps[this.steps.Count - 1];
                if (steps1.Works.Count > 0)
                {
                    Work work1 = (Work)steps1.Works[steps1.Works.Count - 1];
                    work1.NextObj = step;
                    step.PrevObj = work1;
                }
                else
                {
                    steps1.NextObj = step;
                    step.PrevObj = steps1;
                }
            }
            this.steps.Add(step);
            if (step.isFather)
            {
                if (!this.steps_table.Contains(step.Alias)) this.steps_table.Add(step.Alias, step);
            }
        }

        public void ensureProccessAll()
        {
            if (!this.lector.isClose)
            {
                Steps steps1 = null;
                while ((steps1 = this.lector.nextStep()) != null)
                {
                    this.addStep(steps1);
                }
                this.Lector.Close();
            }
        }

        public void getLastWork()
        {
            Steps steps1 = (Steps)this.Steps[this.Steps.Count - 1];
            this.currentObj = steps1.Works[steps1.Works.Count - 1];
        }

        public void getLastStep()
        {
            Steps steps1 = (Steps)this.Steps[this.Steps.Count - 1];
            this.currentObj = steps1;
        }

        public TreeViewItem getRoot()
        {
            if (this.node == null)
            {
                this.node = new TreeViewItem();
                this.node.Header = this.nombre;
                this.node.Tag = this;
                for (int num1 = 0; num1 < this.steps.Count; num1++)
                {
                    Steps steps1 = (Steps)this.steps[num1];
                    TreeViewItem node1 = new TreeViewItem();
                    node1.Header = steps1.Alias;
                    node1.Tag = steps1;
                    //node1.ImageIndex = 2;
                    //node1.SelectedImageIndex = 2;
                    this.node.Items.Add(node1);
                }
                if (!this.Lector.isClose)
                {
                    Steps steps2 = null;
                    while ((steps2 = this.lector.nextStep()) != null)
                    {
                        this.addStep(steps2);
                        TreeViewItem node2 = new TreeViewItem();
                        node2.Header = steps2.Alias;
                        node2.Tag = steps2;
                        //node2.ImageIndex = 2;
                        //node2.SelectedImageIndex = 2;
                        this.node.Items.Add(node2);
                    }
                    this.Lector.Close();
                }
            }
            return this.node;
        }

        public string getStates()
        {
            string text1 = "";
            for (int num1 = 0; num1 < this.states.Length; num1++)
            {
                text1 = text1 + this.states[num1] + ",";
            }
            return text1;
        }

        public string getStringSave()
        {
            string text1 = "<RondasValues><Ronda><id>" + this.id + "</id>" + "<name><![CDATA[" + this.nombre + "]]></name>";
            text1 = text1 + "<User_ID><![CDATA[" + this.usuario + "]]></User_ID><Comentary><![CDATA[";
            text1 = text1 + Comentary + "]]></Comentary>";
            text1 = text1 + "<message_id><![CDATA[" + message_id + "]]></message_id>";
            text1 = text1 + "<message_date><![CDATA[" + this.fecha + " " + this.hora + "]]></message_date>";
            text1 = text1 + "<mindate><![CDATA[" + this.minDate.ToString("yyyy-MM-dd HH:mm:ss") + "]]></mindate>";
            text1 = text1 + "<maxdate><![CDATA[" + this.maxDate.ToString("yyyy-MM-dd HH:mm:ss") + "]]></maxdate>";
            //INICIO 2019/09/09 mamejiar: Se agregan los campos Planta y Puesto
            text1 = text1 + "<Planta><![CDATA[" + this.Planta + "]]></Planta>";
            text1 = text1 + "<Puesto><![CDATA[" + this.Puesto + "]]></Puesto>";
            //FIN 2019/09/09 mamejiar: Se agregan los campos Planta y Puesto
            text1 = text1 + "</Ronda>";
            for (int num1 = 0; num1 < this.steps.Count; num1++)
            {
                text1 = text1 + ((Steps)this.steps[num1]).getStringSave();
            }
            return (text1 + "</RondasValues>");
        }

        public bool isValidState(string state)
        {
            for (int num1 = 0; num1 < this.States.Length; num1++)
            {
                if (this.States[num1].Equals(state))
                {
                    return true;
                }
            }
            return false;
        }

        public object next()
        {
            if (this.currentObj == null)
            {
                if (!this.lector.isClose)
                {
                    this.currentObj = this.lector.nextStep();
                    if (this.currentObj == null)
                    {
                        this.lector.Close();
                    }
                    else if (this.currentObj is Steps)
                    {
                        this.addStep((Steps)this.currentObj);
                    }
                }
                else
                {
                    if (RondasLector.EndObj == RondasLector.Step) this.currentObj = null;
                    else this.currentObj = this.Steps[0];
                }
            }
            else if (this.currentObj is Work)
            {
                Work work1 = (Work)this.currentObj;
                if ((work1.NextObj == null) & !this.lector.isClose)
                {
                    Steps steps1 = this.lector.nextStep();
                    if (steps1 != null)
                    {
                        this.addStep(steps1);
                    }
                    this.currentObj = steps1;
                }
                else
                {
                    this.currentObj = work1.NextObj;
                }
            }
            else
            {
                this.currentObj = ((Steps)this.currentObj).NextObj;
                if (this.currentObj == null)
                {
                    next();
                }
            }
            return this.currentObj;
        }

        public object prev()
        {
            if (this.currentObj == null)
            {
                return null;
            }
            if (this.currentObj is Work)
            {
                this.currentObj = ((Work)this.currentObj).PrevObj;
            }
            else
            {
                this.currentObj = ((Steps)this.currentObj).PrevObj;
            }
            return this.currentObj;
        }

        public override string ToString()
        {
            return "Ronda";
        }

        public string toXML()
        {
            string text1 = "<RondasHHT>";
            text1 = text1 + "<Ronda>";
            text1 = text1 + "<Ronda_ID>" + this.id + "</Ronda_ID>";
            text1 = text1 + "<Nombre>" + this.nombre + "</Nombre>";
            text1 = text1 + "<Puesto>" + this.puesto + "</Puesto>";
            text1 = text1 + "<Planta>" + this.planta + "</Planta>";
            text1 = text1 + "<Fecha_Gen>" + this.fecha + "</Fecha_Gen>";
            text1 = text1 + "<Hora_Gen>" + this.hora + "</Hora_Gen>";
            text1 = text1 + "<Show_Tree>" + (this.show_tree ? "SI" : "NO") + "</Show_Tree>";
            text1 = text1 + "<States>" + this.getStates() + "</States>";
            object obj1 = text1;
            object[] objArray1 = new object[4] { obj1, "<Complete>", this.complete, "</Complete>" };
            text1 = string.Concat(objArray1);
            text1 = text1 + "</Ronda>";
            for (int num1 = 0; num1 < this.steps.Count; num1++)
            {
                text1 = text1 + ((Steps)this.steps[num1]).toXML();
            }
            return (text1 + "</RondasHHT>");
        }


        // Properties
        public bool Complete
        {
            get
            {
                return this.complete;
            }
            set
            {
                this.complete = value;
            }
        }

        public object Current
        {
            get
            {
                return this.currentObj;
            }
            set
            {
                this.currentObj = value;
            }
        }

        public string Fecha
        {
            get
            {
                return this.fecha;
            }
        }

        public string Hora
        {
            get
            {
                return this.hora;
            }
        }

        public string Id
        {
            get
            {
                return this.id;
            }
        }

        public RondasLector Lector
        {
            get
            {
                return this.lector;
            }
            set
            {
                this.lector = value;
            }
        }

        public string Comentary
        {
            get
            {
                return this.comentary;
            }
            set
            {
                this.comentary = value;
            }
        }

        public string Nombre
        {
            get
            {
                return this.nombre;
            }
        }

        public string Planta
        {
            get
            {
                return this.planta;
            }
        }

        public string Puesto
        {
            get
            {
                return this.puesto;
            }
        }

        public bool Show_tree
        {
            get
            {
                return this.show_tree;
            }
        }

        public string[] States
        {
            get
            {
                return this.states;
            }
        }

        public ArrayList Steps
        {
            get
            {
                return this.steps;
            }
        }

        public string MessageID
        {
            get
            {
                return this.message_id;
            }
        }

        public bool Suspend
        {
            get
            {
                return suspend;
            }
            set
            {
                suspend = value;
            }
        }

        public string Usuario
        {
            get
            {
                return this.usuario;
            }
            set
            {
                this.usuario = value;
            }
        }

        public Steps getStepByAlias(string alias)
        {
            return (Steps)steps_table[alias];
        }

        // Fields
        private bool suspend;
        private bool complete;
        private object currentObj;
        private string fecha;
        private string hora;
        private string id;
        [field: NonSerialized()]
        private RondasLector lector;
        //TODO Resolver asunto del atributo node
        [field: NonSerialized()]
        private TreeViewItem node;
        private string nombre;
        private string planta;
        private string puesto;
        private bool show_tree;
        private string[] states;
        private ArrayList steps;
        private Hashtable steps_table;
        private string comentary;
        private string message_id;

        private string usuario;

        private Boolean maxInit = true;
        private Boolean minInit = true;
        private DateTime maxDate = DateTime.Now;
        private DateTime minDate = DateTime.Now;

        private DateTime serverdate;
        private DateTime pocketdate;
        private TimeSpan diff;

        private int _pasos; //Guarda el total de pasos de la ronda

        public DateTime MaxDate
        {
            set
            {
                if (maxInit)
                {
                    maxDate = value;
                    maxInit = false;
                }
                else if (maxDate < value)
                {
                    maxDate = value;
                }
            }
        }

        public DateTime MinDate
        {
            set
            {
                if (minInit)
                {
                    minDate = value;
                    minInit = false;
                }
                else if (minDate > value)
                {
                    minDate = value;
                }
            }
        }
        public int TotalPasos
        {
            get
            {
                return _pasos;
            }
            set
            {
                if (_pasos != value)
                {
                    _pasos = value;
                }
            }
        }

    }
}

