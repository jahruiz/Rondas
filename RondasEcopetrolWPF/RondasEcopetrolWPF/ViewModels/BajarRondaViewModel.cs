namespace RondasEcopetrolWPF.ViewModels
{
    using System;
	using System.Windows;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using RondasEcopetrolWPF.Base;
    using RondasEcopetrolWPF.Models;
    using RondasEcopetrolWPF.PopUps;
    using RondasEcopetrolWPF.ServerUtils;
	using RondasEcopetrolWPF.Views;
 
    public class BajarRondaViewModel : ViewModelBase
    {
        private bool _descargaOk;
        private bool _listUpdate;
        public BajarRondaViewModel()
        {
            //LoadRondas();
        }
        public ObservableCollection<Ronda> RondasDisponibles
        {
            get { return GetPropertyValue<ObservableCollection<Ronda>>(); }
            set
            {
                SetPropertyValue(value);
            }
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
            _listUpdate = true;
            LoadRondas();
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

        public override Task OnNavigatedTo(EventArgs args)
        {
            LoadRondas();
            //((BajarRonda)this.Page).lstRondas.GotTouchCapture += ListView_Click;
            return null;
        }
        #region Metodos
		/*private void ListView_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedUser != null)
                ClickItemListAsync();                
        }*/
        private void LoadRondas()
        {
            using (Loading loading = new Loading(LoadRondasDisponibles, "Buscando..."))
            {
                loading.ShowDialog();
            }
            if (RondasDisponibles.Count == 0)
            {
                if (!_listUpdate)
                {
                    //Ir al menú principal
                    Navigated(typeof(MainPage));
                }
                MessageBox.Show("No hay rondas disponibles para este turno", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }
        public void LoadRondasDisponibles()
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
                                if (!FileUtils.EsRondaYaDescargada(ronda.Message_ID, FileUtils.getActualUser()))
                                    rondas.Add(ronda);
                            }
                        }
                        catch (System.Exception e)
                        {
                            MessageDialogError.ImprimirAsync("La información de las rondas no esta en el formato correcto");
                            LogError.CustomErrorLog(e);
                        }
                    }
                    else
                    {
                        System.IO.StreamReader reader =
                            new System.IO.StreamReader(ServerUtils.getStream());
                        string str = reader.ReadToEnd();
                        reader.Close();
                        MessageDialogError.ImprimirAsync(str);
                    }
                }
                else
                {
                    MessageDialogError.ImprimirAsync("Se genero un error en el servidor, para mas detalle consulte el Log");
                    //Error en la conexión, Asegúrese de dispones servicio de red y que la pocket este conectada correctamente.
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
                                if (!FileUtils.EsRondaYaDescargada(ronda.Message_ID, FileUtils.getActualUser()))
                                    rondas.Add(ronda);
                            }
                        }
                        catch (Exception)
                        {
                            MessageDialogError.ImprimirAsync("La ronda no esta en su formato correcto");
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
                        MessageDialogError.ImprimirAsync(end);
                    }
                }
                else
                {
                    MessageDialogError.ImprimirAsync("Se genero un error en el servidor, para mas detalle consulte el Log");
                    //Error en la conexión, Asegúrese de dispones servicio de red y que la pocket este conectada correctamente.
                }

            }
            ServerUtils.close();
            RondasDisponibles = rondas;
        }

        private void ClickItemListAsync()
        {
            if (SelectedUser == null) return;
            //string textoRonda = "";
            //textoRonda =
            //    SelectedUser.Nombre_Ronda.ToString() + "\n" + "\n" +
            //    "Fecha: " + SelectedUser.Fecha_Gen.ToString() + "\n" +
            //    "Hora: " + SelectedUser.Hora_Gen.ToString() + "\n" +
            //    "Planta: " + SelectedUser.Nombre_Planta.ToString() + "\n" +
            //    "Puesto: " + SelectedUser.Nombre_Puesto.ToString();
            //DetallesRondaAsync(textoRonda);
            using (DetallesRondas detallesRonda = new DetallesRondas(SelectedUser))
            {
                /*if (detallesRonda.mostrar())
                {
                    DescargaAsync(); 
                }
                else
                {
                    SelectedUser = null;
                }*/

                detallesRonda.mostrar(this, DescargaAsync, resetSelectedItem);
            }
        }
        //public  void DetallesRondaAsync(string texto)
        //{
        //    //var messageDialog = new MessageDialog(texto);
        //    //messageDialog.Commands.Add(new UICommand(
        //    //    "Descargar", new UICommandInvokedHandler(DescargarCommand)));
        //    //messageDialog.Commands.Add(new UICommand(
        //    //    "Cancelar"));
        //    //await messageDialog.ShowAsync();
        //    MessageBoxResult result = MessageBox.Show(texto, "Detalle Ronda", MessageBoxButton.OKCancel, MessageBoxImage.Information);
        //    if (result == MessageBoxResult.OK)
        //    {
        //        DescargaAsync();
        //    }
        //}

        private void resetSelectedItem()
        {
            SelectedUser = null;
        }
        private void DescargaAsync()
        {
            _descargaOk = false;
            using (Loading loading = new Loading(DescargarRonda, "Descargando..."))
            {
                loading.ShowDialog();
            }
            if (_descargaOk)
                RondasDisponibles.Remove(SelectedUser);
            else
            {
                resetSelectedItem();
            }
        }
        private void DescargarRonda()
        {
            try
            {
                object[] objArray1 = new object[10] { "rondaid=", SelectedUser.ID_Ronda, "&msgId=", SelectedUser.Message_ID, "&pdate=", DateTime.Now.ToString("yyyyMMddHHmmss"), "&user=", FileUtils.getActualUser(), "&pwd=", FileUtils.getActualUserpwd() };
                if (ServerUtils.send("/getRonda", string.Concat(objArray1)))
                {
                    if (ServerUtils.isMIME("text/xml"))
                    {
                        String msgId = "" + SelectedUser.Message_ID;
                        FileUtils.writeXmlData("rnd" + msgId + ".xml", ServerUtils.getStream());
                        MessageBox.Show("La ronda ha sido descargada con éxito", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                        _descargaOk = true;
                    }
                    else
                    {
                        StreamReader reader1 = new StreamReader(ServerUtils.getStream());
                        MessageBox.Show(reader1.ReadToEnd(), "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                        //await MessageDialogError.ImprimirAsync(reader1.ReadToEnd());//, "Server");
                        reader1.Close();
                    }
                }
                else
                {
                    MessageDialogError.ImprimirAsync("Se genero un error en el servidor, para mas detalle consulte el Log");
                }
                ServerUtils.close();
            }
            catch(Exception e)
            {
                MessageDialogError.ImprimirAsync("Se genero un error descargando la ronda: " + e.Message);
                LogError.CustomErrorLog(e);
            }            
        }
        #endregion Metodos
    }
}
