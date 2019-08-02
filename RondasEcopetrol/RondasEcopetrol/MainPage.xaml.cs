using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace RondasEcopetrol
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string acceso { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            this.acceso = "";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(IniciarSesion));
        }
        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                string acces = (string)e.Parameter;
                if (acces == "true")
                {
                    this.bajar_ronda.IsEnabled = true;
                    this.enviar_ronda.IsEnabled = true;
                    this.cerrar_sesion.IsEnabled = true;
                    this.iniciar_sesion.IsEnabled = false;
                }
            }
        }
        private void Button_SessionClose(object sender, RoutedEventArgs e)
        {
            this.bajar_ronda.IsEnabled = false;
            this.enviar_ronda.IsEnabled = false;
            this.cerrar_sesion.IsEnabled = false;
            this.iniciar_sesion.IsEnabled = true;
        }
        private void Button_BajarRonda(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BajarRonda));
        }
        private void Button_HacerRonda(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HacerRonda));
        }
    }
}

