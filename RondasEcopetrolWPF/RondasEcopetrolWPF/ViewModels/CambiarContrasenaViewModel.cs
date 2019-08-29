using RondasEcopetrol.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;

namespace RondasEcopetrol.ViewModels
{
    public class CambiarContrasenaViewModel : ViewModelBase
    {
        private ICommand _cancelarCommand;
        public override Task OnNavigatedFrom(NavigationEventArgs args)
        {
            //throw new NotImplementedException();
            return null;
        }

        public override Task OnNavigatedTo(NavigationEventArgs args)
        {
            //throw new NotImplementedException();
            return null;
        }
        public ICommand CancelarCommand
        {
            get { return _cancelarCommand = _cancelarCommand ?? new DelegateCommand(CancelarExecute); }
        }

        private void CancelarExecute()
        {
            AppFrame.GoBack();
        }

    }
}
