﻿namespace RondasEcopetrolWPF.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using RondasEcopetrolWPF.Base;
    using RondasEcopetrolWPF.Models;
    using RondasEcopetrolWPF.ServerUtils;
    using RondasEcopetrolWPF.Views;
    using System.Windows;
    public class HacerRondaViewModel : ViewModelBase
    {
        public static bool showSuspendRounds = false;
        public HacerRondaViewModel()
        {
            if (showSuspendRounds)
            {
                tituloPantalla = "RONDAS POR CONTINUAR";
                LoadRondasSuspendidas();
            }
            else
            {
                tituloPantalla = "RONDAS POR REALIZAR";
                LoadRondasDescargadas();
            }
        }

        public string tituloPantalla
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }
        public ObservableCollection<RondaDescargada> RondasDescargadas
        {
            get { return GetPropertyValue<ObservableCollection<RondaDescargada>>(); }
            set
            {
                SetPropertyValue(value);
            }
        }
        public RondaDescargada SelectedUser
        {
            get { return GetPropertyValue<RondaDescargada>(); }
            set
            {
                SetPropertyValue(value);
                this.ClickItemListAsync();
            }
        }

        private ICommand _cancelarCommand;

        #region Commands
        public ICommand CancelarCommand
        {
            get { return _cancelarCommand = _cancelarCommand ?? new DelegateCommand(CancelarExecute); }
        }
        private void CancelarExecute()
        {
            //AppFrame.GoBack();
            base.Navigated(typeof(MainPage));
        }

        #endregion Commands

        //public override Task OnNavigatedFrom(NavigationEventArgs args)
        //{
        //    return null;
        //}

        public override Task OnNavigatedTo(EventArgs args)
        {
            ((HacerRonda)this.Page).lstRondas.PreviewMouseLeftButtonUp += ListView_Click;
            return null;
        }

        #region Metodos
        public async void LoadRondasDescargadas()
        {
            ObservableCollection<RondaDescargada> rondas = new ObservableCollection<RondaDescargada>();

            try
            {
                foreach (string usuario in FileUtils.GetUsuariosRondasDescargadas())
                {
                    foreach (var file in FileUtils.GetArchivosRondasDescargadas(usuario))
                    {
                        Rondas_Descargadas rondas_actuales = FileUtils.Deserialize<Rondas_Descargadas>(FileUtils.GetXmlRonda(file));

                        foreach (RondaDescargada ronda in rondas_actuales)
                        {
                            if (!SuspendRound.isRoundSuspend(ronda.Message_ID))
                            {
                                ronda.Usuario = usuario;
                                rondas.Add(ronda);
                            }
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                await MessageDialogError.ImprimirAsync("Error listando las rondas descargadas");
            }

            RondasDescargadas = rondas;
        }

        public async void LoadRondasSuspendidas()
        {
            ObservableCollection<RondaDescargada> rondas = new ObservableCollection<RondaDescargada>();

            try
            {
                foreach (var rondaSuspendida in SuspendRound.getSuspendRoundsList())
                {
                    RondaDescargada ronda = new RondaDescargada();
                    ronda.Ronda_ID = rondaSuspendida.Id;
                    ronda.Nombre = rondaSuspendida.Nombre;
                    ronda.Planta = rondaSuspendida.Planta;
                    ronda.Puesto = rondaSuspendida.Puesto;
                    ronda.Message_ID = rondaSuspendida.MessageID;
                    ronda.Fecha_Gen = rondaSuspendida.Fecha;
                    ronda.Hora_Gen = rondaSuspendida.Hora;

                    ronda.Usuario = rondaSuspendida.Usuario;

                    rondas.Add(ronda);
                }
            }
            catch (System.Exception)
            {
                await MessageDialogError.ImprimirAsync("Error listando las rondas suspendidas");
            }

            RondasDescargadas = rondas;
        }

        private void ClickItemListAsync()
        {
            string textoRonda = "";
            textoRonda =
                SelectedUser.Nombre.ToString() + "\n" + "\n" +
                "Fecha: " + SelectedUser.Fecha_Gen.ToString() + "\n" +
                "Hora: " + SelectedUser.Hora_Gen.ToString() + "\n" +
                "Planta: " + SelectedUser.Planta.ToString() + "\n" +
                "Puesto: " + SelectedUser.Puesto.ToString();
            DetallesRonda(textoRonda);
        }

        private void ListView_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedUser != null)
                ClickItemListAsync();
        }
        public void DetallesRonda(string texto)
        {
            //var messageDialog = new MessageDialog(texto);
            //messageDialog.Commands.Add(new UICommand(
            //    "Aceptar", new UICommandInvokedHandler(HacerCommand)));
            //messageDialog.Commands.Add(new UICommand(
            //    "Cancelar"));
            //await messageDialog.ShowAsync();
            MessageBoxResult result = MessageBox.Show(texto, "Detalle Ronda", MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (result == MessageBoxResult.OK)
            {
                HacerRonda();
            }

        }

        private void HacerRonda()
        {
            if (showSuspendRounds)
            {
                Rondas ronda = SuspendRound.getSuspendRound("" + SelectedUser.Message_ID);
                if (ronda != null)
                {
                    RondasLector.CurrentRonda = ronda;
                    RondasLector.StartStep = (Steps)ronda.Steps[0];
                    Object current = ronda.Current;
                    if (current is Steps)
                    {
                        RondasLector.CurrentWork = (Work)null;
                        RondasLector.Step = (Steps)current;
                        this.Navigated(typeof(CapturaDatos1));
                    }
                    else
                    {
                        RondasLector.CurrentWork = (Work)current;
                        RondasLector.Step = RondasLector.CurrentWork.Step;
                        CapturaDatos2ViewModel.NEXT_TRIGGER = false;
                        this.Navigated(typeof(CapturaDatos2));
                        CapturaDatos2ViewModel.currentInstance.initPanel();
                    }
                }
            }
            else
            {
                RondasLector lector1 = new RondasLector(FileUtils.loadXMLFromUser("rnd" + SelectedUser.Message_ID + ".xml", SelectedUser.Usuario), SelectedUser.Usuario);
                RondasLector.CurrentRonda = lector1.Current;
                object obj1 = lector1.Current.next();
                if ((obj1 != null) && (obj1 is Steps))
                {
                    RondasLector.StartStep = (Steps)obj1;
                    RondasLector.CurrentWork = (Work)null;
                    RondasLector.Step = (Steps)obj1;
                    this.Navigated(typeof(CapturaDatos1));
                }
            }
        }

        #endregion Metodos
    }
}
