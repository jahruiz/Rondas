namespace RondasEcopetrolWPF.ViewModels
{
    using System;
	using System.Windows;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using RondasEcopetrolWPF.Base;
    using RondasEcopetrolWPF.Models;
    using RondasEcopetrolWPF.ServerUtils;
    using RondasEcopetrolWPF.Views;
    using RondasEcopetrolWPF.PopUps;

    public class EnviarRondaViewModel : ViewModelBase
    {
        private bool _listUpdate;
        public EnviarRondaViewModel()
        {
            //LoadRondasCompl();
        }
        #region Propiedades
        public ObservableCollection<RondaCompletada> RondasaSubir
        {
            get { return GetPropertyValue<ObservableCollection<RondaCompletada>>(); }
            set
            {
                SetPropertyValue(value);
            }
        }
        public RondaCompletada SelectedItem
        {
            get { return GetPropertyValue<RondaCompletada>(); }
            set
            {
                SetPropertyValue(value);
                this.ClickItemListAsync();
            }
        }
        #endregion Propiedades
        #region Comandos
        private ICommand _actualizarCommand;
        private ICommand _cancelarCommand;
        public ICommand ActualizarCommand
        {
            get { return _actualizarCommand = _actualizarCommand ?? new DelegateCommand(ActualizarExecute); }
        }
        public ICommand CancelarCommand
        {
            get { return _cancelarCommand = _cancelarCommand ?? new DelegateCommand(CancelarExecute); }
        }
        #endregion Comandos
        //public override Task OnNavigatedFrom(NavigationEventArgs args)
        //{
        //    //throw new NotImplementedException();
        //    return null;
        //}

        public override Task OnNavigatedTo(EventArgs args)
        {
            LoadRondasCompl();
            //((EnviarRonda)this.Page).lstRondas.GotTouchCapture += ListView_Click;
            return null;
        }

        private void ListView_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
                ClickItemListAsync();                
        }
        private void LoadRondasCompl()
        {
            LoadRondasCompletas();
            if (RondasaSubir.Count == 0)
            {
                if (!_listUpdate)
                {
                    //Ir al menú principal
                    Navigated(typeof(MainPage));
                }
                MessageDialogWarning.ImprimirAsync("No hay rondas por enviar");
                return;
            }
        }
        private void ActualizarExecute()
        {
            _listUpdate = true;
            LoadRondasCompl();
        }
        private void CancelarExecute()
        {
            //AppFrame.GoBack();
            Page.NavigationService.GoBack();
        }
        private void ClickItemListAsync()
        {
            if (SelectedItem == null) return;
            //string textoRonda = "";
            //textoRonda =
            //    SelectedItem.name.ToString() + "\n" + "\n" +
            //    "Fecha: " + SelectedItem.Fecha_Gen.ToString() + "\n" +
            //    "Hora: " + SelectedItem.Hora_Gen.ToString() + "\n" +
            //    "Planta: " + SelectedItem.Planta.ToString() + "\n" +
            //    "Puesto: " + SelectedItem.Puesto.ToString();
            //DetallesRondaAsync(textoRonda);
            using (DetallesRondas detallesRonda = new DetallesRondas(SelectedItem))
            {
                /*if (detallesRonda.mostrar())
                {
                    EnviarRonda();
                }
                else
                {
                    SelectedItem = null;
                }*/

                detallesRonda.mostrar(this, EnviarRonda, resetSelectedItem);
            }
        }
        //     public async void DetallesRondaAsync(string texto)
        //     {
        //         //var messageDialog = new MessageDialog(texto);
        //         //messageDialog.Commands.Add(new UICommand(
        //         //    "Aceptar", new UICommandInvokedHandler(HacerCommand)));
        //         //messageDialog.Commands.Add(new UICommand(
        //         //    "Cancelar"));
        //         //await messageDialog.ShowAsync();
        //MessageBoxResult result = MessageBox.Show(texto, "Detalle Ronda", MessageBoxButton.OKCancel, MessageBoxImage.Information);
        //         if (result == MessageBoxResult.OK)
        //         {
        //             EnviarRonda();
        //         }
        //     }
        //private void HacerCommand(IUICommand command)
        //{

        //}

        private void resetSelectedItem()
        {
            SelectedItem = null;
        }
        public void LoadRondasCompletas()
        {
            ObservableCollection<RondaCompletada> rondas = new ObservableCollection<RondaCompletada>();

            try
            {
				foreach (var file in FileUtils.GetArchivosRondasASubir(FileUtils.getActualUser()))
				{
					Rondas_Completadas rondas_actuales = FileUtils.Deserialize<Rondas_Completadas>(FileUtils.GetXmlRonda(file));

					foreach (RondaCompletada ronda in rondas_actuales)
					{
						ronda.Usuario = FileUtils.getActualUser();
						rondas.Add(ronda);
					}
				}
            }
            catch (System.Exception ex)
            {
                MessageDialogError.ImprimirAsync("Error listando las rondas a enviar: " + ex.Message);
            }
            RondasaSubir = rondas;
        }
        public void EnviarRonda()
        {
            try
            {
                byte[] currentRonda = FileUtils.getUTF8BytesFromXml("rnd" + SelectedItem.message_id + ".drxml");
                UploadSetupManager uploadSetupManager = new UploadSetupManager(currentRonda, SelectedItem.message_id, SelectedItem.Usuario);
                using (Loading loading = new Loading(uploadSetupManager.Enviar, "Enviando..."))
                {
                    loading.ShowDialog();
                }
                if (uploadSetupManager.SendOK)
                {
                    //Actualizar la lista de rondas por enviar
                    LoadRondasCompl();
                }
            }
            catch (Exception e)
            {
                //Error en los datos de la cache de la ronda
                MessageDialogError.ImprimirAsync("Error cargando la ronda suspendida (Ronda ID: " + SelectedItem.message_id + "): " + e.Message);
                return;
            }
        }
    }
}
