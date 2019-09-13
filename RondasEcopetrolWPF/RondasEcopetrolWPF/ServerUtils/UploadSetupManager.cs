namespace RondasEcopetrolWPF.ServerUtils
{
    using RondasEcopetrolWPF.Base;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;

    public class UploadSetupManager
    {
        private byte[] _currentRonda = (byte[])null;
        private string _idMessage = "";
        private string _usuario = "";
        private bool _sendOk;
        //public static DataRow row = (DataRow)null;
        public UploadSetupManager(byte[] CurrentRonda, string IDMessage, string Usuario)
        {
            _currentRonda = CurrentRonda;
            _idMessage = IDMessage;
            _usuario = Usuario;
        }

        public bool SendOK
        {
            get
            {
                return _sendOk;
            }
        }
        public async void Enviar()//RondasApp app, ConnectClass connect) 
        {
            _sendOk = false;
            if (ServerUtils.send("/servlet/ecopetrol.ris.rondas.subirRondas?userId=" + FileUtils.getActualUser() + "&pwd=" + FileUtils.getActualUserpwd(), "text/xml", _currentRonda))
            {
                //connect.setDetail("Recibiendo respuesta");
                StreamReader reader1 = new StreamReader(ServerUtils.getStream());
                string msg = reader1.ReadToEnd();
                MessageBox.Show(msg, "Server", MessageBoxButton.OK, MessageBoxImage.Information);
                //MessageBox.Show(msg, "Server");
                reader1.Close();
                ServerUtils.close();

                if (!msg.StartsWith("Error::"))
                {
                    _sendOk = true;
                    FileUtils.deleteRonda(_usuario, _idMessage);
                    //MessageBox.Show("Ronda enviada con exito", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                    //DataTable table = UploadSetupManager.row.Table;
                    //table.Rows.Remove(UploadSetupManager.row);
                    //table.AcceptChanges();
                }
                //app.showMenuDialog();
            }
            else
            {
                //ErrorMessage.ERRROR_II = true;
                //app.showCanvas(typeof(ErrorMessage));
                await MessageDialogError.ImprimirAsync("Se genero un error en el servidor, para mas detalle consulte el Log");
            }
        }
    }
}
