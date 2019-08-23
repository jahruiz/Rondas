using Windows.UI.Xaml.Controls;
using RondasEcopetrol.Base;
using RondasEcopetrol.Models;
using RondasEcopetrol.Views;
using RondasEcopetrol.ViewModels;

namespace RondasEcopetrol.PopUps
{
    public class RondasSuspenderPopUp : WarningPopUp
    {
        private Frame _curFrame;

        public RondasSuspenderPopUp(Frame curFrame, bool navFromSheet)
        {
            _curFrame = curFrame;
            this.navFromSheet = navFromSheet;
        }
        public override string getDescription()
        {
            return "Desea Suspender la Ronda? \n(Si) La ronda será almacenada en disco y podrá continuarla en otro momento";
        }

        public override string getTitle()
        {
            return "Suspender Ronda";
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
            suspendRound();
            //Ir al menú principal
            _curFrame.Navigate(typeof(MainPage));
        }

        private void suspendRound()
        {
            SuspendRound.addSuspendRound(RondasLector.CurrentRonda);
            RondasLector.CurrentRonda.Suspend = true;
            RondasLector.CurrentWork = null;
            RondasLector.CurrentRonda = null;
            RondasLector.Step = null;
            RondasLector.StartStep = null;
        }
    }
}
