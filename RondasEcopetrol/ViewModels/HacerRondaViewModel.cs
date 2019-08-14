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
            AppFrame.GoBack();
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
            //await DescargaAsync();
            //DescargaAsync();
            AppFrame.Navigate(typeof(CapturaEquipo));
        }

        /*private async void DescargaAsync()
        {
            //var messageDialog = new MessageDialog("Descargando ronda estructurada del sistema RIS...");
            //messageDialog.Commands.Add(new UICommand(
            //    "Cancelar"));
            //await messageDialog.ShowAsync();

            object[] objArray1 = new object[10] { "rondaid=", SelectedUser.ID_Ronda, "&msgId=", SelectedUser.Message_ID, "&pdate=", DateTime.Now.ToString("yyyyMMddHHmmss"), "&user=", FileUtils.getActualUser(), "&pwd=", FileUtils.getActualUserpwd() };
            if (ServerUtils.send("/getRonda", string.Concat(objArray1)))
            {
                if (ServerUtils.isMIME("text/xml"))
                {
                    String msgId = "" + SelectedUser.Message_ID;
                    FileUtils.writeXmlData("rnd" + msgId + ".xml", ServerUtils.getStream());
                    await MessageDialogError.ImprimirAsync("La ronda ha sido descargada con éxito");
                }
                else
                {
                    StreamReader reader1 = new StreamReader(ServerUtils.getStream());
                    await MessageDialogError.ImprimirAsync(reader1.ReadToEnd());//, "Server");
                    reader1.Close();
                }
            }
            else
            {
                //Error en la conexión, Asegúrese de dispones servicio de red y que la pocket este conectada correctamente.
                //app1.showCanvas(typeof(ErrorMessage));
            }
            ServerUtils.close();
        }*/

        #endregion Metodos
    }
}
