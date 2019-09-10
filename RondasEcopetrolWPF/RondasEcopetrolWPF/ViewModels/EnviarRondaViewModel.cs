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
            ((EnviarRonda)this.Page).lstRondas.GotTouchCapture += ListView_Click;
            return null;
        }

        private void ListView_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
                ClickItemListAsync();
        }
        private void LoadRondasCompl()
        {
            using (Loading loading = new Loading(LoadRondasCompletas, "Buscando..."))
            {
                loading.ShowDialog();
            }
            if (RondasaSubir.Count == 0)
            {
                if (!_listUpdate)
                {
                    //Ir al menú principal
                    Navigated(typeof(MainPage));
                }
                MessageBox.Show("No tiene Rondas por Enviar", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
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
            string textoRonda = "";
            textoRonda =
                SelectedItem.name.ToString() + "\n" + "\n" +
                "Fecha: " + SelectedItem.Fecha_Gen.ToString() + "\n" +
                "Hora: " + SelectedItem.Hora_Gen.ToString() + "\n" +
                "Planta: " + SelectedItem.Planta.ToString() + "\n" +
                "Puesto: " + SelectedItem.Puesto.ToString();
            DetallesRondaAsync(textoRonda);
        }
        public async void DetallesRondaAsync(string texto)
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
                EnviarRonda();
            }
        }
        //private void HacerCommand(IUICommand command)
        //{

        //}
        public async void LoadRondasCompletas()
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
                await MessageDialogError.ImprimirAsync("Error listando las rondas a enviar: " + ex.Message);
            }
            RondasaSubir = rondas;
        }
        public void EnviarRonda()
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
    }
}
