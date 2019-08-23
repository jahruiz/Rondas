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
    public class RondasCancelarPopUp : WarningPopUp
    {
        private Frame _curFrame;

        public RondasCancelarPopUp(Frame curFrame, bool navFromSheet)
        {
            _curFrame = curFrame;
            this.navFromSheet = navFromSheet;
        }

        public override string getDescription()
        {
            return "Esta seguro que desea INICIAR esta ronda en otro momento? (Si) Perdera los datos que ud haya tomado.";
        }

        public override string getTitle()
        {
            return "Cancelar Ronda";
        }

        public override void noClick()
        {
            if (navFromSheet)
            {
                _curFrame.Navigate(typeof(CapturaDatos2));
            }
            else
            {
                _curFrame.Navigate(typeof(CapturaDatos1));
            }
        }

        public override void yesClick()
        {
            if (RondasLector.CurrentRonda.Suspend)
            {
                //TODO Pendiente
                /*RondasApp.deleteSuspend(Sheet.CurrentRonda.MessageID);*/
            }
            else
            {
                RondasLector.CurrentRonda.Lector.Close();
            }
            _curFrame.Navigate(typeof(MainPage));
        }
    }
}
