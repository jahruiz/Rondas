namespace RondasEcopetrol.Models
{
    //using jump.controls.tree;
    using System;
    using System.Collections;

    public class Steps
    {
        // Methods
        public Steps(Rondas round, string[] row)
        {
            this.id = null;
            this.type = null;
            this.referencia = null;
            this.orden = 0;
            this.states = null;
            this.selectedValue = -1;
            this.works = new ArrayList();
            this.fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.round = round;
            this.id = row[0];
            this.type = row[1];
            this.referencia = row[2];
            this.orden = Convert.ToInt32(row[3]);
            char[] chArray1 = new char[1] { ',' };
            this.states = row[4].Trim().Split(chArray1);
            this.alias = row[5].Trim();
            this.valid_states = row[6].Trim().Split(chArray1);
            this.selectedValue = (row[7].ToString().Length == 0) ? -1 : Convert.ToInt32(row[5].ToString());
            this.fecha = row[8].ToString();
            this.direccion = row[9] == null ? "" : row[9].ToString();
            if (this.fecha != null && this.fecha.Length == 0)
            {
                Rondas ronda = round;
                this.fecha = DateTime.ParseExact(ronda.Fecha + " " + ronda.Hora, "dd/MM/yyyy HH:mm:ss", null).ToString("yyyy-MM-dd HH:mm:ss");
            }
            commentary = row[10];

            if (commentary != null && commentary.Equals("nothing"))
            {
                commentary = "";
            }

            string oldValue = row[11];

            if (oldValue != null)
            {
                for (int i = 0; i < states.Length; i++)
                {
                    if (oldValue.Equals(states[i]))
                    {
                        this.selectedValue = i;
                        break;
                    }
                }
            }
            isfather = row[12] == null || row[12].Equals("0");
            change = false;
        }

        public void addWork(Work work)
        {
            if (this.works.Count == 0)
            {
                work.PrevObj = this;
                this.NextObj = work;
            }
            else
            {
                Work work1 = (Work)this.works[this.works.Count - 1];
                work.PrevObj = work1;
                work1.NextObj = work;
            }
            this.works.Add(work);
        }

        public void fechar()
        {
            DateTime time1 = round.getRealDate();
            round.MaxDate = time1;
            round.MinDate = time1;
            this.fecha = time1.ToString("yyyy-MM-dd HH:mm:ss");
            this.change = true;
            clearOldValues();
        }

        public void clearOldValues()
        {
            for (int i = 0; i < works.Count; i++)
            {
                Work w = (Work)works[i];
                if (w != null && !w.isValidForThisState())
                {
                    w.clearData();
                }
            }
        }

        //TODO Pendiente resolver esto
        /*public void getRoot(Node root)
        {
            Node node1 = new Node(new RondaTreeObject(this.type, "Step", this));
            root.addNode(node1);
        }*/

        public string getStates()
        {
            string text1 = ",";
            for (int num1 = 0; num1 < this.states.Length; num1++)
            {
                text1 = text1 + this.states[num1] + ",";
            }
            return text1;
        }

        public string getStringSave()
        {
            if (selectedValue != -1)
            {
                string[] textArray1 = new string[9] { "<tipo><id>", this.id, "</id><selectedValue><![CDATA[", (selectedValue != -1 ? this.states[this.selectedValue] : "nothing"), "]]></selectedValue><fecha><![CDATA[", this.fecha, "]]></fecha><descripcion><![CDATA[", this.commentary.Length == 0 ? "nothing" : this.commentary, "]]></descripcion></tipo>" };
                string text1 = string.Concat(textArray1);
                for (int num1 = 0; num1 < this.works.Count; num1++)
                {
                    text1 = text1 + ((Work)this.works[num1]).getStringSave();
                }
                return text1;
            }
            return "";
        }

        public override string ToString()
        {
            return "Steps";
        }

        public string toXML()
        {
            string text1 = "<Step>";
            text1 = text1 + "<Step_ID>" + this.id + "</Step_ID>";
            text1 = text1 + "<Step_Type>" + this.type + "</Step_Type>";
            text1 = text1 + "<Ronda_ID>" + this.round.Id + "</Ronda_ID>";
            text1 = text1 + "<Step_Ref>" + this.referencia + "</Step_Ref>";
            object obj1 = text1;
            object[] objArray1 = new object[4] { obj1, "<Step_Orden>", this.orden, "</Step_Orden>" };
            text1 = string.Concat(objArray1);
            text1 = text1 + "<Step_States>" + this.getStates() + "</Step_States>";
            text1 = text1 + "<Selected_State>" + (selectedValue != -1 ? this.states[this.selectedValue] : "nothing") + "</Selected_State>";
            text1 = text1 + "<Fecha>" + this.fecha + "</Fecha>";
            text1 = text1 + "</Step>";
            for (int num1 = 0; num1 < this.works.Count; num1++)
            {
                text1 = text1 + ((Work)this.works[num1]).toXML();
            }
            return text1;
        }

        public string[] Valid_states
        {
            get
            {
                return valid_states;
            }
        }


        // Properties
        public string Id
        {
            get
            {
                return this.id;
            }
        }

        public object NextObj
        {
            get
            {
                return this.nextObj;
            }
            set
            {
                this.nextObj = value;
            }
        }

        public int Orden
        {
            get
            {
                return this.orden;
            }
        }

        public object PrevObj
        {
            get
            {
                return this.prevObj;
            }
            set
            {
                this.prevObj = value;
            }
        }

        public string Referencia
        {
            get
            {
                return this.referencia;
            }
        }

        public Rondas Round
        {
            get
            {
                return this.round;
            }
        }

        public int SelectedValue
        {
            get
            {
                return this.selectedValue;
            }
            set
            {
                this.selectedValue = value;
            }
        }

        public string[] States
        {
            get
            {
                return this.states;
            }
        }

        public string Type
        {
            get
            {
                return this.type;
            }
        }

        public string Alias
        {
            get
            {
                return this.alias;
            }
        }

        public ArrayList Works
        {
            get
            {
                return this.works;
            }
        }

        public bool isValid()
        {
            if (selectedValue == -1) return false;
            string str = states[SelectedValue];
            for (int num1 = 0; num1 < this.valid_states.Length; num1++)
            {
                if (this.valid_states[num1].Equals(str))
                {
                    return true;
                }
            }
            return false;
        }

        public bool isValidState(string state)
        {
            for (int num1 = 0; num1 < this.valid_states.Length; num1++)
            {
                if (this.valid_states[num1].Equals(state))
                {
                    return true;
                }
            }
            return false;
        }

        public string Direccion
        {
            get
            {
                return direccion;
            }
        }

        public string Commentary
        {
            set
            {
                commentary = value;
            }
            get
            {
                return commentary;
            }
        }

        public bool isFather
        {
            get
            {
                return isfather && !change;
            }
        }

        // Fields
        private bool isfather;
        private bool change;
        private string fecha;
        private string id;
        private object nextObj;
        private int orden;
        private object prevObj;
        private string referencia;
        private Rondas round;
        private int selectedValue;
        private string[] states;
        private string[] valid_states;
        private string type;
        private ArrayList works;
        private string alias;
        private string direccion;
        private string commentary;
    }

}

