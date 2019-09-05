namespace RondasEcopetrolWPF.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using RondasEcopetrolWPF.Base;
    using RondasEcopetrolWPF.ServerUtils;
    using RondasEcopetrolWPF.Views;
    public class MainPageViewModel : ViewModelBase
    {
        public static string Eco;
        public static bool Isvalid;
        public static bool initProgram = true;

        public MainPageViewModel()
        {
            if (initProgram)
            {
                RondasEcopetrolWPF.Models.SuspendRound.LoadSuspends();
                initProgram = false;
            }
        }
        #region Propiedades
        public bool IsButtonEnable
        {
            get { return GetPropertyValue<bool>(); ; }
            set
            {
                SetPropertyValue(value);
            }
        }
        public bool IsButtonSesionEnable
        {
            get { return GetPropertyValue<bool>(); ; }
            set
            {
                SetPropertyValue(value);
            }
        }
        public string CantRondasdescargadas
        {
            get { return GetPropertyValue<string>(); ; }
            set
            {
                SetPropertyValue(value);
            }
        }
        public string CantRondasporSubir
        {
            get { return GetPropertyValue<string>(); ; }
            set
            {
                SetPropertyValue(value);
            }
        }
        #endregion Propiedades

        private DelegateCommand<string> _navigationCommand;
        public DelegateCommand<string> NavigationCommand
        {
            get { return _navigationCommand = _navigationCommand ?? new DelegateCommand<string>(NavigationExecute); }
        }
        private void NavigationExecute(string viewFrame)
        {
            switch (viewFrame)
            {
                case "IniciarSesion":
                    Navigated(typeof(IniciarSesion));
                    break;
                case "BajarRonda":
                    Navigated(typeof(BajarRonda));
                    break;
                case "EnviarRonda":
                    Navigated(typeof(EnviarRonda));
                    break;
                case "HacerRonda":
                    HacerRondaViewModel.showSuspendRounds = false;
                    Navigated(typeof(HacerRonda));
                    break;
                case "ContinuarRonda":
                    HacerRondaViewModel.showSuspendRounds = true;
                    Navigated(typeof(HacerRonda));
                    break;
                case "Salir":
                    RondasEcopetrolWPF.Models.SuspendRound.SaveSuspends();
                    App.Current.Shutdown();
                    break;
            }
        }
        //public override Task OnNavigatedFrom(NavigationEventArgs args)
        //{
        //    return null;
        //}

        public override Task OnNavigatedTo(EventArgs args)
        {
            int Rondasdescargadas = 0, RondasporSubir = 0;
            this.IsButtonSesionEnable = true;
            if (Isvalid)
            {
                IsButtonEnable = Isvalid;
                IsButtonSesionEnable = false;
            }
            try
            {
                foreach (string usuario in FileUtils.GetUsuariosRondasDescargadas())
                {
                    foreach (var file in FileUtils.GetArchivosRonda(usuario))
                    {                        
                       if (file.EndsWith(".xml"))
                        {
                             Rondasdescargadas+= 1;
                        }
                       else if(file.EndsWith(".drxml"))
                        {
                             RondasporSubir +=1;
                        }                        
                    }
                }
                CantRondasdescargadas = "Total descargadas: " + Rondasdescargadas.ToString();
                CantRondasporSubir = "Por enviar: " + RondasporSubir.ToString();
            }
            catch (System.Exception e)
            {
                
            }
            return null;
        }
    }
}
