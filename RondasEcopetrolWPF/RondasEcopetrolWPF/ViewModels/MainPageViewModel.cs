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
        public MainPageViewModel()
        {
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
            this.IsButtonSesionEnable = true;
            if (Isvalid)
            {
                IsButtonEnable = Isvalid;
                IsButtonSesionEnable = false;
            }
            return null;
        }
    }
}
