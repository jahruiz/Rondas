namespace RondasEcopetrolWPF.Models
{
    using System;
    using System.Text.RegularExpressions;

    public class Work
    {
        // Methods
        public Work(Steps step, string[] row)
        {
            this.workname = null;
            this.actividad = null;
            this.orden = 0;
            this.nombre = null;
            this.id = null;
            this.tipo = null;
            this.step = null;
            this.values = null;
            this.valor = "";
            this.descripcion = "";
            this.obligatorio = false;
            this.fecha = "";
            this.oldValue1 = "";
            this.oldValue2 = "";
            this.workname = row[0];
            this.actividad = row[1];
            this.orden = Convert.ToInt32(row[2]);
            this.nombre = row[3];
            this.id = row[4];
            this.tipo = row[5];
            this.step = step;
            this.causa = "";
            char[] chArray1 = new char[1] { ',' };
            if (row[6].Equals("@"))
            {
                this.values = new string[0] { };
            }
            else
            {
                this.values = row[6].Trim().Split(chArray1);
            }
            this.obligatorio = row[9].Equals("SI");
            this.descripcion = row[8];
            this.valor = row[7];
            this.fecha = row[10];
            this.oldValue1 = row[11];
            this.oldValue2 = row[12];
            this.um = row[14];
            string _states = row[15];
            if (_states != null)
            {
                _states = _states.Trim();
                if (_states.IndexOf(',') == -1)
                {
                    this.states = new string[] { _states };
                }
                else
                {
                    this.states = _states.Trim().Split(chArray1);
                }
            }
            if (this.fecha.Length == 0)
            {
                Rondas ronda = step.Round;
                this.fecha = DateTime.ParseExact(ronda.Fecha + " " + ronda.Hora, "dd/MM/yyyy HH:mm:ss", null).ToString("yyyy-MM-dd HH:mm:ss");
            }
            chArray1 = new char[1] { '@' };
            string[] actions = row[13].Split(chArray1);
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i] = actions[i].Replace(',', '\n');
            }
            this.actions = actions;

            chArray1[0] = ',';
            this.ncrChars = row[16] != null ? row[16].Split(chArray1) : null;
            this.incChars = row[17] != null ? row[17].Split(chArray1) : null;
            this.Description = row[18];
        }

        public void fechar()
        {
            DateTime time1 = step.Round.getRealDate();
            step.Round.MaxDate = time1;
            step.Round.MinDate = time1;
            this.fecha = time1.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public string getStringSave()
        {
            if (this.fecha != null && !"".Equals(this.fecha))
            {
                if (!(this.Valor.Length == 0 && this.descripcion.Length == 0))
                {
                    string[] textArray1 = new string[13] { "<work><id>", this.id, "</id><valor><![CDATA[", (this.Valor.Length == 0) ? "nothing" : this.valor, "]]></valor><descripcion><![CDATA[", (this.descripcion.Length == 0) ? "nothing" : this.descripcion, "]]></descripcion><fecha><![CDATA[", this.fecha, "]]></fecha><causa><![CDATA[", this.causa, "]]></causa><ncr><![CDATA[", this.ncr, "]]></ncr></work>" };
                    return string.Concat(textArray1);
                }
            }
            return "";
        }

        public string getValues()
        {
            string text1 = "";
            for (int num1 = 0; num1 < this.values.Length; num1++)
            {
                text1 = text1 + this.values[num1] + ",";
            }
            if (this.values.Length == 0) return "@";
            return text1;
        }

        public void save()
        {
        }

        public override string ToString()
        {
            return "Works";
        }

        public string toXML()
        {
            string text1 = "<Work>";
            text1 = text1 + "<Work_Name>" + this.workname + "</Work_Name>";
            text1 = text1 + "<Actividad>" + this.actividad + "</Actividad>";
            object obj1 = text1;
            object[] objArray1 = new object[4] { obj1, "<Orden>", this.orden, "</Orden>" };
            text1 = string.Concat(objArray1);
            text1 = text1 + "<Nombre>" + this.nombre + "</Nombre>";
            text1 = text1 + "<ID>" + this.id + "</ID>";
            text1 = text1 + "<Tipo>" + this.tipo + "</Tipo>";
            text1 = text1 + "<Step_ID>" + this.step.Id + "</Step_ID>";
            text1 = text1 + "<Values>" + this.getValues() + "</Values>";
            text1 = text1 + "<Valor>" + this.valor + "</Valor>";
            text1 = text1 + "<description>" + this.descripcion + "</description>";
            text1 = text1 + "<Necessary>" + (this.obligatorio ? "SI" : "NO") + "</Necessary>";
            text1 = text1 + "<Fecha>" + this.fecha + "</Fecha>";
            text1 = text1 + "<oldValue1>" + this.oldValue1 + "</oldValue1>";
            text1 = text1 + "<oldValue2>" + this.oldValue2 + "</oldValue2>";
            return (text1 + "</Work>");
        }

        public int validEntryText(int index, string str, bool showMsg, out string resultMsgTitle, out string resultMsgDetail)
        {
            ncr = "";
            int value = 2;
            string msg = "Acciones";
            string suffix = "";
            resultMsgTitle = null;
            resultMsgDetail = null;
            if (ncrChars != null)
                for (int i = 0; i < ncrChars.Length; i++)
                {
                    if (ncrChars[i].Equals(str))
                    {
                        ncr = "GC:Baja";
                        msg = "No conformidad";
                        suffix = "\nSe sugiere documentar.";
                        value = 1;
                        break;
                    }
                }
            if (incChars != null)
                for (int i = 0; i < incChars.Length; i++)
                {
                    if (incChars[i].Equals(str))
                    {
                        ncr = "VO:Baja";
                        msg = "Incidente";
                        suffix = "\nSe sugiere documentar.";
                        value = 1;
                        break;
                    }
                }
            string action = getAction(index);
            if (!action.Equals("NA"))
            {
                if (showMsg)
                {
                    //MessageBox.Show(action + suffix, msg);
                    resultMsgDetail = action + suffix;
                    resultMsgTitle = msg;
                }
            }
            else if (value == 1)
            {
                if (showMsg)
                {
                    //MessageBox.Show("Se sugiere documentar", msg);
                    resultMsgDetail = "Se sugiere documentar";
                    resultMsgTitle = msg;
                }
            }

            return value;
        }

        public int validEntry(string str, bool showMsg, out string resultMsgTitle, out string resultMsgDetail)
        {
            ncr = "";
            int flag1;
            resultMsgTitle = null;
            resultMsgDetail = null;
            if ((this.Values.Length == 1) || this.Values[0].Equals("null"))
            {
                return 2;
            }
            try
            {
                System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo();
                info.CurrencyDecimalSeparator = ".";
                info.CurrencyGroupSeparator = ".";
                info.NumberDecimalSeparator = ".";
                info.NumberGroupSeparator = "";
                Regex rx = new Regex(@"(\-|\+)?\d+(\.\d+)?");
                if (!rx.IsMatch(str))
                {
                    throw new Exception();
                }
                double num1 = Double.Parse(str, info);
                double num2 = Double.Parse(this.Values[0], info);
                double num3 = Double.Parse(this.Values[1], info);
                double num4 = Double.Parse(this.Values[2], info);
                double num5 = Double.Parse(this.Values[3], info);
                double num6 = Double.Parse(this.Values[5], info);
                double num7 = Double.Parse(this.Values[4], info);

                bool exp1 = num6 == -9999 ? false : num1 < num6;
                bool exp2 = num7 == -9999 ? false : num1 > num7;
                if (exp1 || exp2)
                {
                    object[] objArray1 = null;
                    if (num6 == -9999)
                        objArray1 = new object[2] { "El limite superior es ", num7 };
                    else if (num7 == -9999)
                        objArray1 = new object[2] { "El limite inferior es ", num6 };
                    else
                        objArray1 = new object[4] { "Los Limites son ", num6, " y ", num7 };
                    //MessageBox.Show(string.Concat(objArray1), "Fuera de Limites");
                    resultMsgDetail = string.Concat(objArray1);
                    resultMsgTitle = "Fuera de Limites";
                    return 0;
                }

                string msg;
                exp1 = num4 == -9999 ? false : num1 > num4;
                exp2 = num5 == -9999 ? false : num1 < num5;
                if (exp1 || exp2)
                {

                    object[] objArray3 = null;
                    if (num4 == -9999)
                        objArray3 = new object[2] { "La ventana inferior es ", num5 };
                    else if (num5 == -9999)
                        objArray3 = new object[2] { "La ventana superior es ", num4 };
                    else
                        objArray3 = new object[4] { "Las ventanas son ", num5, " y ", num4 };

                    string finalMsg = string.Concat(objArray3);
                    if (num1 > num4) msg = getAction(2);
                    else msg = getAction(3);
                    if (!msg.Equals("NA")) finalMsg += "\nAcciones:\n" + msg;
                    if (exp1) ncr = "VO:Alta";
                    if (exp2) ncr = "VO:Baja";
                    finalMsg += "\nSe sugiere documentar.";
                    if (showMsg)
                    {
                        //MessageBox.Show(finalMsg, "Fuera de Ventana");
                        resultMsgDetail = finalMsg;
                        resultMsgTitle = "Fuera de Ventana";
                    }
                    return 1;
                }

                exp1 = num2 == -9999 ? false : num1 > num2;
                exp2 = num3 == -9999 ? false : num1 < num3;
                if (exp2 || exp1)
                {
                    object[] objArray2 = null;
                    if (num3 == -9999)
                        objArray2 = new object[2] { "La guia superior es ", num2 };
                    else if (num2 == -9999)
                        objArray2 = new object[2] { "La guia inferior es ", num3 };
                    else
                        objArray2 = new object[4] { "Las guias son ", num3, " y ", num2 };

                    string finalMsg = string.Concat(objArray2);
                    if (num1 > num2) msg = getAction(0);
                    else msg = getAction(1);
                    if (!msg.Equals("NA")) finalMsg += "\nAcciones:\n" + msg;
                    if (exp1) ncr = "GC:Alta";
                    if (exp2) ncr = "GC:Baja";
                    finalMsg += "\nSe sugiere documentar.";
                    if (showMsg)
                    {
                        //MessageBox.Show(finalMsg, "Fuera de Guia");
                        resultMsgDetail = finalMsg;
                        resultMsgTitle = "Fuera de Guia";
                    }
                    return 1;
                }
                flag1 = 2;
            }
            catch (Exception)
            {
                //MessageBox.Show("El Tipo de dato es invalido, asegurese de que el valor digitado es un número.", "Error de Formato");
                resultMsgDetail = "El Tipo de dato es invalido, asegurese de que el valor digitado es un número.";
                resultMsgTitle = "Error de Formato";
                flag1 = 0;
            }
            return flag1;
        }


        // Properties
        public string Actividad
        {
            get
            {
                return this.actividad;
            }
        }

        public string Descripcion
        {
            get
            {
                return this.descripcion;
            }
            set
            {
                this.descripcion = value;
            }
        }

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

        public string Nombre
        {
            get
            {
                return this.nombre;
            }
        }

        public bool Obligatorio
        {
            get
            {
                return this.obligatorio;
            }
        }

        public string OldValue1
        {
            get
            {
                return this.oldValue1;
            }
        }

        public string OldValue2
        {
            get
            {
                return this.oldValue2;
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

        public Steps Step
        {
            get
            {
                return this.step;
            }
        }

        public string Tipo
        {
            get
            {
                return this.tipo;
            }
        }

        public string Valor
        {
            get
            {
                return this.valor;
            }
            set
            {
                this.valor = value;
            }
        }

        public string[] Values
        {
            get
            {
                return this.values;
            }
        }

        public string Workname
        {
            get
            {
                return this.workname;
            }
        }

        public string getAction(int i)
        {
            return actions[i];
        }

        public string UM
        {
            get
            {
                return um;
            }
        }

        public void clearData()
        {
            this.valor = "";
            this.descripcion = "";
            this.fecha = "";
            this.ncr = "";
            this.noComment = false;
        }

        public bool isValidForThisState()
        {
            string state = step.States[step.SelectedValue];
            if (states == null) states = step.Valid_states;
            if (states == null)
            {
                clearData();
                return false;
            }
            if (states.Length == 0) return true;
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].Equals(state)) return true;
            }
            clearData();
            return false;
        }

        public bool NoComment
        {
            get
            {
                return noComment;
            }
            set
            {
                noComment = value;
            }
        }

        public String Causa
        {
            get
            {
                return causa;
            }
            set
            {
                causa = value;
            }
        }

        public String Description
        {
            get
            {
                return this.desc;
            }
            set
            {
                this.desc = value;
            }
        }

        // Fields
        private string actividad;
        private string descripcion;
        private string fecha;
        private string id;
        private object nextObj;
        private string nombre;
        private bool obligatorio;
        private string oldValue1;
        private string oldValue2;
        private int orden;
        private object prevObj;
        private Steps step;
        private string tipo;
        private string valor;
        private string[] values;
        private string[] actions;
        private string workname;
        private string um; //Unidad de Medida
        private string ncr; //Guarda los incidentes.
        private string[] states;
        //Utilez solo para tareas tipo estado
        private string[] ncrChars;
        private string[] incChars;
        private String causa; //Almacena la informacion de la causa
        private bool noComment = false;
        private string desc;
    }
}
