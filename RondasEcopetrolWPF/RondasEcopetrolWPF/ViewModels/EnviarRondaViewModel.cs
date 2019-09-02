namespace RondasEcopetrolWPF.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using RondasEcopetrolWPF.Base;
    using RondasEcopetrolWPF.Models;
    using RondasEcopetrolWPF.ServerUtils;
    using RondasEcopetrolWPF.Views;
    public class EnviarRondaViewModel : ViewModelBase
    {
        public EnviarRondaViewModel()
        {
            LoadRondasCompletas();
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
        private ICommand _cancelarCommand;
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
            //throw new NotImplementedException();
            return null;
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
        }
        //private void HacerCommand(IUICommand command)
        //{

        //}
        public async void LoadRondasCompletas()
        {
            ObservableCollection<RondaDescargada> rondas = new ObservableCollection<RondaDescargada>();

            try
            {
                foreach (string usuario in FileUtils.GetUsuariosRondasDescargadas())
                {
                    foreach (var file in FileUtils.GetArchivosRondasASubir(usuario))
                    {
                        Rondas_Descargadas rondas_actuales = FileUtils.Deserialize<Rondas_Descargadas>(FileUtils.GetXmlRonda(file));

                        foreach (RondaDescargada ronda in rondas_actuales)
                        {
                            ronda.Usuario = usuario;
                            rondas.Add(ronda);
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                await MessageDialogError.ImprimirAsync("Error listando las rondas a enviar");
            }

            RondasaSubir = rondas;
        }
    }
}
