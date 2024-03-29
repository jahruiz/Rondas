﻿namespace RondasEcopetrolWPF.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using RondasEcopetrolWPF.Base;
    using RondasEcopetrolWPF.ServerUtils;
    using RondasEcopetrolWPF.Views;
    using RondasEcopetrolWPF.Models;
    using System.Windows;

    public class MainPageViewModel : ViewModelBase
    {
        public static string Eco;
        public static bool Isvalid;

        private const string _FILENAMEXML = @".xml";
        private const string _FILECONTINUARNAMEDRXML = @".drxml";

        public MainPageViewModel()
        {
            SuspendRound.LoadSuspends();
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
        public string CantRondasPorContinuar
        {
            get { return GetPropertyValue<string>(); ; }
            set
            {
                SetPropertyValue(value);
            }
        }
        public string CantRondasPorHacer
        {
            get { return GetPropertyValue<string>(); ; }
            set
            {
                SetPropertyValue(value);
            }
        }
        public string InfoUsuario
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
                    //SuspendRound.SaveSuspends();
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
            (this.Page.Parent as Window).WindowState = WindowState.Maximized;
            int Rondasdescargadas = 0, RondasporSubir = 0;
            this.IsButtonSesionEnable = true;
            if (Isvalid)
            {
                IsButtonEnable = Isvalid;
                IsButtonSesionEnable = false;
                InfoUsuario = "USUARIO: " + FileUtils.getActualUser().ToUpper();
            }
            else
            {
                InfoUsuario = "USUARIO: <DESCONECTADO>";
            }
            try
            {
                foreach (string usuario in FileUtils.GetUsuariosRondasDescargadas())
                {
                    foreach (var file in FileUtils.GetArchivosRonda(usuario))
                    {                        
                       if (file.EndsWith(_FILENAMEXML))
                        {
                             Rondasdescargadas+= 1;
                        }
                       else if(file.EndsWith(_FILECONTINUARNAMEDRXML))
                        {
                             RondasporSubir +=1;
                        }                        
                    }
                }
                int rondasPorContinuar = SuspendRound.getSuspendRoundCount();
                int rondasPorHacer = Rondasdescargadas - RondasporSubir - rondasPorContinuar;

                //Registrar datos de estado global de la app
                CantRondasdescargadas = "Total descargadas: " + Rondasdescargadas.ToString();
                CantRondasPorHacer = "Por hacer: " + rondasPorHacer.ToString();
                CantRondasPorContinuar = "Por continuar: " + rondasPorContinuar.ToString();
                CantRondasporSubir = "Por enviar: " + RondasporSubir.ToString();
            }
            catch (Exception)
            {
                
            }
            return null;
        }
    }
}
