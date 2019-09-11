using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using RondasEcopetrolWPF.Base;
using RondasEcopetrolWPF.Models;
using RondasEcopetrolWPF.Views;
using System.Windows;
using RondasEcopetrolWPF.ServerUtils;

namespace RondasEcopetrolWPF.ViewModels
{
    public class CortarRondaViewModel : ViewModelBase
    {
        private static bool navFromSheet;

        public static void Navigate(ViewModelBase fromViewModel)
        {
            navFromSheet = fromViewModel is CapturaDatos2ViewModel;

            string texto = "Si corta la ronda en este momento podrá continuarla desde el portal RIS en la intranet.\n\nDesea cortar la ronda en este momento?";
            MessageBoxResult result = MessageBox.Show(texto, "Cortar Ronda", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                fromViewModel.Navigated(typeof(CortarRonda));
            }
        }

        #region Propiedades
        public string Comentario
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }

        public string NombreRonda
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }
        #endregion Propiedades

        #region Commands

        private ICommand _aceptarCommand;
        private ICommand _cancelarCommand;
        public ICommand AceptarCommand
        {
            get { return _aceptarCommand = _aceptarCommand ?? new DelegateCommand(CortarRonda); }
        }
        public ICommand CancelarCommand
        {
            get { return _cancelarCommand = _cancelarCommand ?? new DelegateCommand(Cancelar); }
        }

        #endregion Commands

        public override Task OnNavigatedTo(EventArgs args)
        {
            initPanel();
            return null;
        }

        #region Metodos
        public void initPanel()
        {
            this.NombreRonda = "RONDA: " + RondasLector.CurrentRonda.Nombre;
            this.Comentario = "";
        }

        public void CortarRonda()
        {
            if (Comentario.Trim().Length != 0)
            {
                RondasLector.CurrentRonda.ensureProccessAll();
                Rondas rondas1 = RondasLector.CurrentRonda;
                rondas1.Comentary = Comentario.Trim();
                String rondaXML = rondas1.getStringSave();
                FileUtils.writeXmlData("rnd" + rondas1.MessageID + ".drxml", rondaXML, rondas1.Usuario);

                if (rondas1.Suspend)
                {
                    SuspendRound.deleteSuspend(rondas1.MessageID);
                }

                //Ir al menú principal
                Navigated(typeof(MainPage));
            }
            else
            {
                MessageBox.Show("Debe documentar", "Error",MessageBoxButton.OK, MessageBoxImage.Error);
                ((CortarRonda)this.Page).txtComentario.Focus();
            }

        }
        public void Cancelar()
        {
            if (navFromSheet)
            {
                CapturaDatos2ViewModel.NEXT_TRIGGER = false;
                Navigated(typeof(CapturaDatos2));
            }
            else
            {
                Navigated(typeof(CapturaDatos1));
            }
        }
        #endregion Metodos
    }
}
