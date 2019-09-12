using System;
using RondasEcopetrolWPF.Base;
using RondasEcopetrolWPF.Models;
using RondasEcopetrolWPF.Views;
using RondasEcopetrolWPF.ServerUtils;
using RondasEcopetrolWPF.ViewModels;
using System.Windows.Controls;

namespace RondasEcopetrolWPF.PopUps
{
    public class RondasFinalizarPopUp : WarningPopUp
    {
        private ViewModelBase _viewModel;

        public RondasFinalizarPopUp(ViewModelBase viewModel, bool navFromSheet)
        {
            _viewModel = viewModel;
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
            /*if (navFromSheet)
            {
                CapturaDatos2ViewModel.NEXT_TRIGGER = false;
                RondasLector.CurrentRonda.getLastWork();
                _viewModel.Navigated(typeof(CapturaDatos2));
                //CapturaDatos2ViewModel.currentInstance.initPanel();
            }
            else
            {
                RondasLector.CurrentRonda.getLastStep();
                _viewModel.Navigated(typeof(CapturaDatos1));
            }*/
        }

        public override void yesClick()
        {
            Rondas rondas1 = RondasLector.CurrentRonda;
            rondas1.ensureProccessAll();
            String rondaXML = rondas1.getStringSave();
            FileUtils.writeXmlData("rnd" + rondas1.MessageID + ".drxml", rondaXML, rondas1.Usuario);

            //TODO Pendiente traducir esto
            /*DataRow[] rowArray1 = RondasApp.app.DsetRondasDown.Tables["Ronda"].Select("Message_ID=" + rondas1.MessageID);
            rowArray1[0]["Complete"] = "true";
            rowArray1[0].AcceptChanges();
            rowArray1[0].Table.AcceptChanges();*/

            if (rondas1.Suspend)
            {
                SuspendRound.deleteSuspend(rondas1.MessageID);
            }
        }
    }
}
