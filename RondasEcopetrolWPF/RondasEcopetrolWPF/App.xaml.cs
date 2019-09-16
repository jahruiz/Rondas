using RondasEcopetrolWPF.Models;
using System.Threading;
using System.Windows;

namespace RondasEcopetrolWPF
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SuspendRound.SaveSuspends();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "RondasEcopetrolWPF";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                //MessageBox.Show("Ya existe una instancia de la aplicación ejecutandose","Advertencia",MessageBoxButton.OK,MessageBoxImage.Warning);
                Application.Current.Shutdown();
            }
            base.OnStartup(e);
        }
    }
}
