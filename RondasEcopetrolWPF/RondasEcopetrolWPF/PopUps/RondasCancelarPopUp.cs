using Windows.UI.Xaml.Controls;
using RondasEcopetrol.Base;
using RondasEcopetrol.Models;
using RondasEcopetrol.Views;
using RondasEcopetrol.ViewModels;

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
            return "Esta seguro que desea INICIAR esta ronda en otro momento? \n(Si) Perdera los datos que ud haya tomado.";
        }

        public override string getTitle()
        {
            return "Cancelar Ronda";
        }

        public override void noClick()
        {
            if (navFromSheet)
            {
                CapturaDatos2ViewModel.NEXT_TRIGGER = false;
                _curFrame.Navigate(typeof(CapturaDatos2));
                CapturaDatos2ViewModel.currentInstance.initPanel();
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
                SuspendRound.deleteSuspend(RondasLector.CurrentRonda.MessageID);
            }
            else
            {
                RondasLector.CurrentRonda.Lector.Close();
            }
            //Ir al menú principal
            _curFrame.Navigate(typeof(MainPage));
        }
    }
}
