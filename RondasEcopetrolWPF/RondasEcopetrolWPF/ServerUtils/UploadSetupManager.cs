namespace RondasEcopetrolWPF.ServerUtils
{
    using RondasEcopetrolWPF.Base;
    using System.IO;
    using System.Threading.Tasks;
    public class UploadSetupManager
    {
        private byte[] _currentRonda = (byte[])null;
        private string _idMessage = "";
        private string _usuario = "";
        //public static DataRow row = (DataRow)null;
        public UploadSetupManager(byte[] CurrentRonda, string IDMessage, string Usuario)
        {
            _currentRonda = CurrentRonda;
            _idMessage = IDMessage;
            _usuario = Usuario;
        }

        public async Task<bool> Enviar()//RondasApp app, ConnectClass connect) 
        {
            if (ServerUtils.send("/servlet/ecopetrol.ris.rondas.subirRondas?userId=" + FileUtils.getActualUser() + "&pwd=" + FileUtils.getActualUserpwd(), "text/xml", _currentRonda))
            {
                //connect.setDetail("Recibiendo respuesta");
                StreamReader streamReader = new StreamReader(ServerUtils.getStream());
                string end = streamReader.ReadToEnd();
                //int num = (int)MessageBox.Show(end, "Server");
                streamReader.Close();
                ServerUtils.close();
                if (!end.StartsWith("Error::"))
                {
                    FileUtils.deleteUserasync(_usuario, _idMessage);
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
                await MessageDialogError.ImprimirAsync("Fallas de Conexión no permiten almacenar esta ronda en RIS.");
            }
            return true;
        }
    }
}
