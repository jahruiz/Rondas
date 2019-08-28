namespace RondasEcopetrol.ViewModels
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using RondasEcopetrol.Base;
    using RondasEcopetrol.ServerUtils;
    using RondasEcopetrol.Views;
    using Windows.UI.Xaml.Navigation;
    public class IniciarSesionViewModel : ViewModelBase
    {
        //Variables
        public static string Eco;
        #region Propiedades
        public string User
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
                Info = "";
            }
        }

        public string Password
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
                Info = "";
            }
        }

        public string Info
        {
            get { return GetPropertyValue<string>(); ; }
            set
            {
                SetPropertyValue(value);
            }
        }

        #endregion Propiedades
        #region Command
        private ICommand _iniciarSesionCommand;
        private ICommand _cancelarSesionCommand;
        public ICommand IniciarSesionCommand
        {
            get { return _iniciarSesionCommand = _iniciarSesionCommand ?? new DelegateCommandAsync(IniciarSesionExecuteAsync); }
        }
        public ICommand CancelarSesionCommand
        {
            get { return _cancelarSesionCommand = _cancelarSesionCommand ?? new DelegateCommand(CancelarSesionExecute); }
        }
        private async Task IniciarSesionExecuteAsync()
        {
            if (string.IsNullOrEmpty(User))
            {
                Info = "Debe ingresar un nombre de usuario." + "\r\n";
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                Info = "Debe ingresar el password." + "\r\n";
                return;
            }
            try
            {
                string escapePassword = WebUtility.UrlEncode(Password);
                if (validText(User.ToUpper()))
                {
                    FileUtils.initPath();
                    Eco = ServerUtils.initServer();
                    if (await loginAsync(User, escapePassword))
                    {
                        FileUtils.createUser(User.ToUpper());
                        FileUtils.configure_user(User, escapePassword);
                        AppFrame.Navigate(typeof(MainPage), true);
                    }
                }
                else
                {
                    Info="El nombre de usuario deben ser carcateres en el rango 'a'-'z', 'A'-'Z', '0'-'9'";
                }
            }
            catch (Exception ex)
            {
                Info = "Error::interno reinicie el programa " + ex.Message + "\r\n";
            }
        }
        private void CancelarSesionExecute()
        {
            if (AppFrame.CanGoBack) AppFrame.GoBack();
        }
        #endregion Command
        public override Task OnNavigatedFrom(NavigationEventArgs args)
        {
            return null;
        }

        public override Task OnNavigatedTo(NavigationEventArgs args)
        {
            return null;
        }


        #region Metodos
       
        private async Task<bool> loginAsync(string user, string pwd)
        {
            bool IsValidUser = false;
            if (ServerUtils.send("/validateUser", "user=" + user + "&pwd=" + pwd))
            {
                try
                {
                    System.IO.StreamReader reader = new System.IO.StreamReader(ServerUtils.getStream());
                    string returnValue = reader.ReadToEnd();
                    IsValidUser = (returnValue != null && returnValue.Equals("true"));
                    if (!IsValidUser)
                    {
                        if (returnValue != null && returnValue.Equals("block"))
                            await MessageDialogError.ImprimirAsync("El usuario esta bloqueado consulte a su administrador para que pueda restablecerle contraseña.");
                        else
                            await MessageDialogError.ImprimirAsync("Usuario y / o Clave Incorrecta");
                    }
                }
                catch (System.Exception e)
                {
                    await MessageDialogError.ImprimirAsync(e.Message);
                    IsValidUser = false;
                }
            }
            else
            {
                await MessageDialogError.ImprimirAsync("Error en la conexion con la BD, intente más tarde");
                IsValidUser = false;
            }
            ServerUtils.close();
            return IsValidUser;
        }
        private bool validText(String text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (!Char.IsLetterOrDigit(c)) return false;
            }
            return true;
        }
        #endregion Metodos
    }
}
