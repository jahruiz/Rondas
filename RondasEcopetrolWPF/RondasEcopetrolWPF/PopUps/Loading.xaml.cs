namespace RondasEcopetrolWPF.PopUps
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Lógica de interacción para Loading.xaml
    /// </summary>
    public partial class Loading : Window, IDisposable
    {
        //private string _title;
        //private string _detail;
        private Action _accion { get; set; }
        public Loading(Action Accion, string Detail)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.WindowStyle = WindowStyle.None; //sin barra de titulo

            this.Title = "Title";
            this.txbDetail.Text = Detail != "" ? Detail : "Conectando...";
            _accion = Accion;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(_accion).ContinueWith(t => { Close(); }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        public void Dispose()
        {
        }
    }
}
