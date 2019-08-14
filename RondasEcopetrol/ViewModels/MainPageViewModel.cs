﻿namespace RondasEcopetrol.ViewModels
{
    using System.Threading.Tasks;
    using RondasEcopetrol.Base;
    using RondasEcopetrol.ServerUtils;
    using RondasEcopetrol.Views;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Navigation;
    public class MainPageViewModel : ViewModelBase
    {
        public static string Eco;
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
                case "EnviarRonda":
                    AppFrame.Navigate(typeof(EnviarRonda));
                    break;
                case "HacerRonda":
                    AppFrame.Navigate(typeof(HacerRonda));
                    break;
                case "BajarRonda":
                    AppFrame.Navigate(typeof(BajarRonda));
                    break;
                case "IniciarSesion":
                    AppFrame.Navigate(typeof(IniciarSesion));
                    break;
                case "Salir":
                    Application.Current.Exit();
                    break;
            }
        }
        public override Task OnNavigatedFrom(NavigationEventArgs args)
        {
            return null;
        }

        public override Task OnNavigatedTo(NavigationEventArgs args)
        {
            IsButtonSesionEnable = true;
            if (  args.Parameter != null && args.Parameter != "")
            {
                IsButtonEnable = (bool)args.Parameter;
                IsButtonSesionEnable = false;
            }
            return null;
        }
    }
}
