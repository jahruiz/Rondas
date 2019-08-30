namespace RondasEcopetrolWPF.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using RondasEcopetrolWPF.Base;
    using RondasEcopetrolWPF.Models;
    using RondasEcopetrolWPF.ServerUtils;
 
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
            Page.NavigationService.GoBack();
            //AppFrame.GoBack();
        }

        #endregion Commands
        //public override Task OnNavigatedFrom(NavigationEventArgs args)
        //{
        //    return null;
        //}

        //public override Task OnNavigatedTo(NavigationEventArgs args)
        //{
        //    return null;
        //}
        #region Metodos
        public async void LoadRondasDisponibles()
        {
            ObservableCollection<Ronda> rondas = new ObservableCollection<Ronda>();

            if (IniciarSesionViewModel.Eco.Equals("gcb"))
            {
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
                        catch (System.Exception)
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
            }
            else
            {
                if (ServerUtils.send("/servlet/ecopetrol.ris.rondas.RondasDisponibles", "usuario=" + FileUtils.getActualUser() + "&pwd=" + FileUtils.getActualUserpwd()))
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
                        catch (Exception)
                        {
                            await MessageDialogError.ImprimirAsync("La ronda no esta en su formato correcto");
                            //int num = (int)MessageBox.Show("La ronda no esta en su formato correcto");
                            //app.showMenuDialog();
                        }
                    }
                    else
                    {
                        StreamReader streamReader = new StreamReader(ServerUtils.getStream());
                        string end = streamReader.ReadToEnd();
                        streamReader.Close();
                        //int num = (int)MessageBox.Show(end);
                        await MessageDialogError.ImprimirAsync(end);
                    }
                }
                else
                {
                    //Error en la conexión, Asegúrese de dispones servicio de red y que la pocket este conectada correctamente.
                }

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
            //var messageDialog = new MessageDialog(texto);
            //messageDialog.Commands.Add(new UICommand(
            //    "Descargar", new UICommandInvokedHandler(DescargarCommand)));
            //messageDialog.Commands.Add(new UICommand(
            //    "Cancelar"));
            //await messageDialog.ShowAsync();
        }
        //private void DescargarCommand(IUICommand command)
        //{
        //    //await DescargaAsync();
        //    DescargaAsync();
        //}
        private async void DescargaAsync()
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
        }
        #endregion Metodos
    }
}
