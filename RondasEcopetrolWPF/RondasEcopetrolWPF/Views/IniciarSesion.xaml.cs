using RondasEcopetrolWPF.Base;
using System.Windows;
using System.Windows.Controls;

namespace RondasEcopetrolWPF.Views
{
    /// <summary>
    /// Lógica de interacción para IniciarSesion.xaml
    /// </summary>
    public partial class IniciarSesion : PageBase
    {
        public IniciarSesion()
        {
            InitializeComponent();
        }

        // helper to hide watermark hint in password field
        private void passwordChanged(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password.Length == 0)
                txtPassword.Background.Opacity = 1;
            else
                txtPassword.Background.Opacity = 0;
        }

    }
}
