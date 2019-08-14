using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RondasEcopetrol.Base;
using Windows.UI.Xaml.Navigation;
using System.Windows.Input;
using RondasEcopetrol.Views;

namespace RondasEcopetrol.ViewModels
{
    public class CapturaTareaViewModel : ViewModelBase
    {


        private DelegateCommand<string> _navigationCommand;
        private ICommand _aceptarCommand;
        private ICommand _cancelarCommand;
        public DelegateCommand<string> NavigationCommand
        {
            get { return _navigationCommand = _navigationCommand ?? new DelegateCommand<string>(NavigationExecute); }
        }
        private void NavigationExecute(string viewFrame)
        {
            switch (viewFrame)
            {
                case "Anterior":
                    //AppFrame.Navigate(typeof(HacerRonda));
                    break;
                case "Siguiente":
                    AppFrame.Navigate(typeof(CapturaEquipo));
                    break;
            }
        }

        public ICommand AceptarCommand
        {
            get { return _aceptarCommand = _aceptarCommand ?? new DelegateCommand(AceptarExecute); }
        }
        public ICommand CancelarCommand
        {
            get { return _cancelarCommand = _cancelarCommand ?? new DelegateCommand(CancelarExecute); }
        }
        private void AceptarExecute()
        {

        }
        private void CancelarExecute()
        {
            AppFrame.GoBack();
        }

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
    }
}
