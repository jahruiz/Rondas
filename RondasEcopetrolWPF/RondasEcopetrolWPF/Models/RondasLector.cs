namespace RondasEcopetrolWPF.Models
{
    using RondasEcopetrolWPF.ServerUtils;
    using System;
    using System.Xml;

    public class RondasLector
    {
        public RondasLector()
        {
        }

        // Methods
        public RondasLector(XmlTextReader reader, string usuario)
        {
            this.usuario = usuario;
            this.close = false;
            this.reader = reader;
            reader.MoveToContent();
            reader.WhitespaceHandling = WhitespaceHandling.None;
            this.createRonda();
        }

        public void Close()
        {
            this.reader.Close();
            this.close = true;
        }

        public void createRonda()
        {
            if (XmlUtils.readNextElement(this.reader) && this.reader.Name.Equals("Ronda"))
            {
                string[] textArray1 = new string[11] {
                        XmlUtils.readNextTextContent(this.reader, "Ronda_ID"),
                        XmlUtils.readNextTextContent(this.reader, "Nombre"),
                        XmlUtils.readNextTextContent(this.reader, "Puesto"),
                        XmlUtils.readNextTextContent(this.reader, "Planta"),
                        XmlUtils.readNextTextContent(this.reader, "Fecha_Gen"),
                        XmlUtils.readNextTextContent(this.reader, "Hora_Gen"),
                        XmlUtils.readNextTextContent(this.reader, "Show_Tree"),
                        XmlUtils.readNextTextContent(this.reader, "States"),
                        XmlUtils.readNextTextContent(this.reader, "Message_ID"),
                        XmlUtils.readNextTextContent(this.reader, "serverdate"),
                        XmlUtils.readNextTextContent(this.reader, "pocketdate"),
                };
                XmlUtils.readEndElement(this.reader, "Ronda");
                this.current = new Rondas(textArray1);
                this.current.Lector = this;
                this.current.Usuario = this.usuario;
            }
        }

        public void createWorks(Steps step)
        {
            while (XmlUtils.readNextElement(this.reader))
            {
                if (this.reader.Name.Equals("Work"))
                {

                    string[] textArray1 = new string[19];
                    textArray1[0] = XmlUtils.readNextTextContent(this.reader, "Work_Name");
                    textArray1[1] = XmlUtils.readNextTextContent(this.reader, "Actividad");
                    textArray1[2] = XmlUtils.readNextTextContent(this.reader, "Orden");
                    textArray1[3] = XmlUtils.readNextTextContent(this.reader, "Nombre");
                    textArray1[4] = XmlUtils.readNextTextContent(this.reader, "ID");
                    textArray1[5] = XmlUtils.readNextTextContent(this.reader, "Tipo");
                    XmlUtils.readNextTextContent(this.reader, "Step_ID");
                    textArray1[6] = XmlUtils.readNextTextContent(this.reader, "Values");
                    textArray1[13] = XmlUtils.readNextTextContent(this.reader, "actions");
                    textArray1[16] = XmlUtils.readNextTextContent(this.reader, "ncr");
                    textArray1[17] = XmlUtils.readNextTextContent(this.reader, "inc");
                    textArray1[7] = XmlUtils.readNextTextContent(this.reader, "Valor");
                    textArray1[8] = XmlUtils.readNextTextContent(this.reader, "description");
                    textArray1[9] = XmlUtils.readNextTextContent(this.reader, "Necessary");
                    textArray1[10] = XmlUtils.readNextTextContent(this.reader, "Fecha");
                    textArray1[11] = XmlUtils.readNextTextContent(this.reader, "oldValue1");
                    textArray1[12] = XmlUtils.readNextTextContent(this.reader, "oldValue2");
                    textArray1[14] = XmlUtils.readNextTextContent(this.reader, "um");
                    textArray1[15] = XmlUtils.readNextTextContent(this.reader, "states");
                    textArray1[18] = XmlUtils.readNextTextContent(this.reader, "desc");
                    Work work1 = new Work(step, textArray1);
                    step.addWork(work1);
                    continue;
                }
                if (this.reader.Name.Equals("Step"))
                {
                    return;
                }
            }
        }

        /*public static void debug(string[] items)
        {
            for (int num1 = 0; num1 < items.Length; num1++)
            {
                MessageBox.Show(items[num1]);
            }
        }*/

        public Steps nextStep()
        {
            do
            {
                if (this.reader.Name.Equals("Step"))
                {

                    string[] textArray1 = new string[13];
                    textArray1[0] = XmlUtils.readNextTextContent(this.reader, "Step_ID");
                    textArray1[1] = XmlUtils.readNextTextContent(this.reader, "Step_Type");
                    XmlUtils.readNextTextContent(this.reader, "Ronda_ID");
                    textArray1[2] = XmlUtils.readNextTextContent(this.reader, "Step_Ref");
                    textArray1[3] = XmlUtils.readNextTextContent(this.reader, "Step_Orden");
                    textArray1[4] = XmlUtils.readNextTextContent(this.reader, "Step_States");
                    textArray1[5] = XmlUtils.readNextTextContent(this.reader, "alias");
                    textArray1[6] = XmlUtils.readNextTextContent(this.reader, "valid_states");
                    textArray1[7] = XmlUtils.readNextTextContent(this.reader, "Selected_State");
                    textArray1[8] = XmlUtils.readNextTextContent(this.reader, "Fecha");
                    textArray1[9] = XmlUtils.readNextTextContent(this.reader, "dir");
                    textArray1[10] = XmlUtils.readNextTextContent(this.reader, "commentary");
                    textArray1[11] = XmlUtils.readNextTextContent(this.reader, "oldvalue");
                    textArray1[12] = XmlUtils.readNextTextContent(this.reader, "father");
                    XmlUtils.readEndElement(this.reader, "Step");
                    Steps steps1 = new Steps(this.current, textArray1);
                    this.createWorks(steps1);


                    return steps1;
                }
            }
            while (XmlUtils.readNextElement(this.reader));
            return null;
        }


        // Properties
        public Rondas Current
        {
            get
            {
                return this.current;
            }
        }

        public bool isClose
        {
            get
            {
                return this.close;
            }
        }


        // Fields
        private bool close;
        private Rondas current;
        private XmlTextReader reader;
        private string usuario;

        // Campos estaticos migrados de la clase StateMachine
        public static Steps Step; //StateMachine.step
        public static Steps StartStep; //StateMachine.StartStep
        public static Object EndObj; //StateMachine.EndObj

        // Campos estaticos migrados de la clase Sheet
        public static Work CurrentWork; //Sheet.current
        public static Rondas CurrentRonda; //Sheet.CurrentRonda

    }
}
