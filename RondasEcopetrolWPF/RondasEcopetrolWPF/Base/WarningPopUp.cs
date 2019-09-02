using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RondasEcopetrolWPF.Base
{
    public abstract class WarningPopUp
    {
        private bool _yes;
        protected bool navFromSheet;

        public abstract string getTitle();
        public abstract string getDescription();
        public abstract void noClick();

        public abstract void yesClick();

        public bool showAsync()
        {
            //var messageDialog = new MessageDialog(getDescription());
            //messageDialog.Title = getTitle();
            //messageDialog.Commands.Add(new UICommand(
            //    "Sí", new UICommandInvokedHandler(yesCommand)));
            //messageDialog.Commands.Add(new UICommand(
            //    "No", new UICommandInvokedHandler(noCommand)));
            //await messageDialog.ShowAsync();
            MessageBoxResult result = MessageBox.Show(getDescription(), getTitle(), MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _yes = true;
                yesClick();
            }
            else if (result == MessageBoxResult.No)
            {
                _yes = false;
                noClick();
            }
            return _yes;
        }

        //private void noCommand(IUICommand command)
        //{
        //    _yes = false;
        //    noClick();
        //}

        //private void yesCommand(IUICommand command)
        //{
        //    _yes = true;
        //    yesClick();
        //}
    }
}
