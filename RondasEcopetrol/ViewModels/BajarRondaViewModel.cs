namespace RondasEcopetrol.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using RondasEcopetrol.Base;
    using RondasEcopetrol.Models;
    using RondasEcopetrol.ServerUtils;
    using Windows.UI.Popups;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;
    public class BajarRondaViewModel : ViewModelBase
    {
        public BajarRondaViewModel()
        {
            LoadRondasDisponibles();
        }
        public ObservableCollection<Ronda> RondasDisponibles
        {
            get;
            set;
        }
        public Ronda SelectedUser
        {
            get { return GetPropertyValue<Ronda>(); }
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
        public async void LoadRondasDisponibles()
        {
            ObservableCollection<Ronda> rondas = new ObservableCollection<Ronda>();

            if (ServerUtils.send("/rondasDisponibles", "usuario=" + FileUtils.getActualUser() + "&pwd=" + FileUtils.getActualUserpwd()))
            {
                if (ServerUtils.isMIME("text/xml"))
                {
                    try
                    {
                        Rondas_Disponibles rondas_Disponibles = FileUtils.Deserialize<Rondas_Disponibles>(FileUtils.loadDataFromStreamS(ServerUtils.getStream()));

                        foreach (Ronda ronda in rondas_Disponibles)
                        {
                            rondas.Add(ronda);
                        }
                    }
                    catch (System.Exception e)
                    {
                        await MessageDialogError.ImprimirAsync("La ronda no esta en su formato correcto");
                    }
                }
                else
                {
                    System.IO.StreamReader reader =
                        new System.IO.StreamReader(ServerUtils.getStream());
                    string str = reader.ReadToEnd();
                    reader.Close();
                    await MessageDialogError.ImprimirAsync(str);
                }
            }
            else
            {
                //Error en la conexión, Asegúrese de dispones servicio de red y que la pocket este conectada correctamente.
                //RondasApp.app.showCanvas(typeof(ErrorMessage));
            }
            ServerUtils.close();
            RondasDisponibles = rondas;
        }
        private void ClickItemListAsync()
        {
            string textoRonda = "";
            textoRonda =
                SelectedUser.Nombre_Ronda.ToString() + "\n" + "\n" +
                "Fecha: " + SelectedUser.Fecha_Gen.ToString() + "\n" +
                "Hora: " + SelectedUser.Hora_Gen.ToString() + "\n" +
                "Planta: " + SelectedUser.Nombre_Planta.ToString() + "\n" +
                "Puesto: " + SelectedUser.Nombre_Puesto.ToString();
            DetallesRondaAsync(textoRonda);
        }
        public async void DetallesRondaAsync(string texto)
        {
            var messageDialog = new MessageDialog(texto);
            messageDialog.Commands.Add(new UICommand(
                "Descargar", new UICommandInvokedHandler(DescargarCommand)));
            messageDialog.Commands.Add(new UICommand(
                "Cancelar"));
            await messageDialog.ShowAsync();
        }
        private async void DescargarCommand(IUICommand command)
        {
            await DescargaAsync();
        }
        public async System.Threading.Tasks.Task DescargaAsync()
        {
            var messageDialog = new MessageDialog("Descargando ronda estructurada del sistema RIS...");
            messageDialog.Commands.Add(new UICommand(
                "Cancelar"));
            await messageDialog.ShowAsync();

            object[] objArray1 = new object[10] { "rondaid=", SelectedUser.ID_Ronda, "&msgId=", SelectedUser.Message_ID,"&pdate=",DateTime.Now.ToString("yyyyMMddHHmmss"),"&user=",FileUtils.getActualUser(),"&pwd=",FileUtils.getActualUserpwd() };
            if (ServerUtils.send("/getRonda", string.Concat(objArray1)))
            {
                if (ServerUtils.isMIME("text/xml"))
                {
                    String msgId = "" + SelectedUser.Message_ID;
                    FileUtils.writeXmlData("rnd" + msgId + ".xml", ServerUtils.getStream());
                    await MessageDialogError.ImprimirAsync("La ronda ha sido descargada a su Pocket con éxito");
                }
            }
        }
        #endregion Metodos
    }
}
