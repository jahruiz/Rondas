﻿using RondasEcopetrolWPF.Base;
using RondasEcopetrolWPF.Models;
using RondasEcopetrolWPF.Views;
using RondasEcopetrolWPF.ViewModels;
using System.Windows.Controls;

namespace RondasEcopetrolWPF.PopUps
{
    public class RondasSuspenderPopUp : WarningPopUp
    {
        private ViewModelBase _viewModel;

        public RondasSuspenderPopUp(ViewModelBase viewModel, bool navFromSheet)
        {
            _viewModel = viewModel;
            this.navFromSheet = navFromSheet;
        }
        public override string getDescription()
        {
            return "Desea Suspender la Ronda? \n\n(Si) La ronda será almacenada en disco y podrá continuarla en otro momento";
        }

        public override string getTitle()
        {
            return "Suspender Ronda";
        }

        public override void noClick()
        {
            if (navFromSheet && _viewModel is CapturaDatos1ViewModel)
            {
                CapturaDatos2ViewModel.NEXT_TRIGGER = false;
                _viewModel.Navigated(typeof(CapturaDatos2));
                //CapturaDatos2ViewModel.currentInstance.initPanel();
            }
            else if (!navFromSheet && _viewModel is CapturaDatos2ViewModel)
            {
                _viewModel.Navigated(typeof(CapturaDatos1));
            }
        }

        public override void yesClick()
        {
            suspendRound();
            //Ir al menú principal
            _viewModel.Navigated(typeof(MainPage));
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
