using RondasEcopetrolWPF.Base;
using RondasEcopetrolWPF.Models;
using RondasEcopetrolWPF.Views;
using RondasEcopetrolWPF.ViewModels;
using System.Windows.Controls;

namespace RondasEcopetrolWPF.PopUps
{
    public class RondasCancelarPopUp : WarningPopUp
    {
        private ViewModelBase _viewModel;

        public RondasCancelarPopUp(ViewModelBase viewModel, bool navFromSheet)
        {
            _viewModel = viewModel;
            this.navFromSheet = navFromSheet;
        }

        public override string getDescription()
        {
            return "Esta seguro que desea INICIAR esta ronda en otro momento? \n\n(Si) Perdera los datos que ud haya tomado.";
        }

        public override string getTitle()
        {
            return "Cancelar Ronda";
        }

        public override void noClick()
        {
            /*if (navFromSheet)
            {
                CapturaDatos2ViewModel.NEXT_TRIGGER = false;
                _viewModel.Navigated(typeof(CapturaDatos2));
                //CapturaDatos2ViewModel.currentInstance.initPanel();
            }
            else
            {
                _viewModel.Navigated(typeof(CapturaDatos1));
            }*/
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
            _viewModel.Navigated(typeof(MainPage));
        }
    }
}
