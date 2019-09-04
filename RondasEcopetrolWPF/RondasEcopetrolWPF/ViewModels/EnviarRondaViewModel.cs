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
        public EnviarRondaViewModel()
        {
            LoadRondasCompl();
        }
        #region Propiedades
        public ObservableCollection<RondaDescargada> RondasaSubir
        {
            get { return GetPropertyValue<ObservableCollection<RondaDescargada>>(); }
            set
            {
                SetPropertyValue(value);
            }
        }
        public RondaDescargada SelectedItem
        {
            get { return GetPropertyValue<RondaDescargada>(); }
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
            ((EnviarRonda)this.Page).lstRondas.PreviewMouseLeftButtonUp += ListView_Click;
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
        }
        private void ActualizarExecute()
        {
            LoadRondasCompl();
        }
        private void CancelarExecute()
        {
            //AppFrame.GoBack();
            Page.NavigationService.GoBack();
        }
        private void ClickItemListAsync()
        {
            string textoRonda = "";
            textoRonda =
                SelectedItem.Nombre.ToString() + "\n" + "\n" +
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
                
            }
        }
        //private void HacerCommand(IUICommand command)
        //{

        //}
        public async void LoadRondasCompletas()
        {
            ObservableCollection<RondaDescargada> rondas = new ObservableCollection<RondaDescargada>();

            try
            {
				foreach (var file in FileUtils.GetArchivosRondasASubir(FileUtils.getActualUser()))
				{
					Rondas_Descargadas rondas_actuales = FileUtils.Deserialize<Rondas_Descargadas>(FileUtils.GetXmlRonda(file));

					foreach (RondaDescargada ronda in rondas_actuales)
					{
						ronda.Usuario = FileUtils.getActualUser();
						rondas.Add(ronda);
					}
				}
            }
            catch (System.Exception)
            {
                await MessageDialogError.ImprimirAsync("Error listando las rondas a enviar");
            }
			if(rondas.Count==0)
			{
				MessageBox.Show("No tiene Rondas por Enviar", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
			}
            RondasaSubir = rondas;
        }
    }
}
