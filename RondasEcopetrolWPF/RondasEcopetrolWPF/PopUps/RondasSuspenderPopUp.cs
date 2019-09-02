using RondasEcopetrolWPF.Base;
using RondasEcopetrolWPF.Models;
using RondasEcopetrolWPF.Views;
using RondasEcopetrolWPF.ViewModels;
using System.Windows.Controls;

namespace RondasEcopetrolWPF.PopUps
{
    public class RondasSuspenderPopUp : WarningPopUp
    {
        private Page _page;

        public RondasSuspenderPopUp(Page page, bool navFromSheet)
        {
            _page = page;
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
                _page.NavigationService.Navigate(typeof(CapturaDatos2));
                CapturaDatos2ViewModel.currentInstance.initPanel();
            }
            else
            {
                _page.NavigationService.Navigate(typeof(CapturaDatos1));
            }
        }

        public override void yesClick()
        {
            suspendRound();
            //Ir al menú principal
            _page.NavigationService.Navigate(typeof(MainPage));
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
