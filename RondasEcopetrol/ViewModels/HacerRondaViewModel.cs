namespace RondasEcopetrol.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using RondasEcopetrol.Base;
    using RondasEcopetrol.Models;
    using RondasEcopetrol.ServerUtils;
    using Windows.UI.Popups;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;
    using RondasEcopetrol.Views;
    public class HacerRondaViewModel : ViewModelBase
    {
        public HacerRondaViewModel()
        {
            LoadRondasDescargadas();
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
        private ICommand _actualizarCommand;
        private ICommand _cancelarCommand;

        #region Commands
        public ICommand ActualizarCommand
        {
            get { return _actualizarCommand = _actualizarCommand ?? new DelegateCommand(ActualizarExecute); }
        }
        public ICommand CancelarCommand
        {
            get { return _cancelarCommand = _cancelarCommand ?? new DelegateCommand(CancelarExecute); }
        }
        private void ActualizarExecute()
        {

        }
        private void CancelarExecute()
        {
            //AppFrame.GoBack();
            AppFrame.Navigate(typeof(MainPage));
        }

        #endregion Commands
        public override Task OnNavigatedFrom(NavigationEventArgs args)
        {
            return null;
        }

        public override Task OnNavigatedTo(NavigationEventArgs args)
        {
            return null;
        }
        #region Metodos
        public async void LoadRondasDescargadas()
        {
            ObservableCollection<RondaDescargada> rondas = new ObservableCollection<RondaDescargada>();

            try
            {
                foreach (string usuario in await FileUtils.GetUsuariosRondasDescargadasAsync())
                {
                    foreach (var file in await FileUtils.GetArchivosRondasDescargadasAsync(usuario))
                    {
                        Rondas_Descargadas rondas_actuales = FileUtils.Deserialize<Rondas_Descargadas>(await FileUtils.GetXmlRondaAsync(file));

                        foreach (RondaDescargada ronda in rondas_actuales)
                        {
                            ronda.Usuario = usuario;
                            rondas.Add(ronda);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                await MessageDialogError.ImprimirAsync("Error listando las rondas descargadas");
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
            DetallesRondaAsync(textoRonda);
        }
        public async void DetallesRondaAsync(string texto)
        {
            var messageDialog = new MessageDialog(texto);
            messageDialog.Commands.Add(new UICommand(
                "Aceptar", new UICommandInvokedHandler(HacerCommand)));
            messageDialog.Commands.Add(new UICommand(
                "Cancelar"));
            await messageDialog.ShowAsync();
        }
        private void HacerCommand(IUICommand command)
        {
            RondasLector lector1 = new RondasLector(FileUtils.loadXMLFromUser("rnd" + SelectedUser.Message_ID + ".xml", SelectedUser.Usuario), SelectedUser.Usuario);
            RondasLector.CurrentRonda = lector1.Current;
            object obj1 = lector1.Current.next();
            if ((obj1 != null) && (obj1 is Steps))
            {
                RondasLector.StartStep = (Steps)obj1;
                RondasLector.Step = (Steps)obj1;
                AppFrame.Navigate(typeof(CapturaDatos1));
            }
            
        }

        #endregion Metodos
    }
}
