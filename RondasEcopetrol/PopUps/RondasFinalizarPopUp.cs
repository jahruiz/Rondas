using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using RondasEcopetrol.Base;
using RondasEcopetrol.Models;
using RondasEcopetrol.Views;
using RondasEcopetrol.ServerUtils;

namespace RondasEcopetrol.PopUps
{
    public class RondasFinalizarPopUp : WarningPopUp
    {
        private Frame _curFrame;

        public RondasFinalizarPopUp(Frame curFrame, bool navFromSheet)
        {
            _curFrame = curFrame;
            this.navFromSheet = navFromSheet;
        }

        public override string getDescription()
        {
            return "No existen más datos por capturar.\nDesea terminar la ronda en este momento?";
        }

        public override string getTitle()
        {
            return "Finalizar Ronda";
        }

        public override void noClick()
        {
            if (navFromSheet)
            {
                //TODO
                /*Sheet.NEXT_TRIGGER = false;*/
                RondasLector.CurrentRonda.getLastWork();
                _curFrame.Navigate(typeof(CapturaDatos2));
            }
            else
            {
                _curFrame.Navigate(typeof(CapturaDatos1));
            }
        }

        public override void yesClick()
        {
            Rondas rondas1 = RondasLector.CurrentRonda;
            rondas1.ensureProccessAll();
            String rondaXML = rondas1.getStringSave();
            FileUtils.writeXmlData("rnd" + rondas1.MessageID + "Save.xml", rondaXML);
            //System.IO.FileStream stream = new System.IO.FileStream("/My Documents/ronda" + rondas1.Id + DateTime.Now.ToString("yyyyMMddHHmmss"), System.IO.FileMode.Create);
            //byte[] b = System.Text.UTF8Encoding.UTF8.GetBytes(rondaXML);
            //stream.Write(b, 0, b.Length);
            //stream.Close();

            //TODO Pendiente traducir esto
            /*DataRow[] rowArray1 = RondasApp.app.DsetRondasDown.Tables["Ronda"].Select("Message_ID=" + rondas1.MessageID);
            rowArray1[0]["Complete"] = "true";
            rowArray1[0].AcceptChanges();
            rowArray1[0].Table.AcceptChanges();
            if (rondas1.Suspend)
            {
                RondasApp.deleteSuspend(rondas1);
            }*/
        }
    }
}
