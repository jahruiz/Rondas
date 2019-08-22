using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace RondasEcopetrol.Base
{
    public abstract class WarningPopUp
    {
        private bool _yes;
        protected bool navFromSheet;

        public abstract string getTitle();
        public abstract string getDescription();
        public abstract void noClick();

        public abstract void yesClick();

        public async Task<bool> showAsync()
        {
            var messageDialog = new MessageDialog(getDescription());
            messageDialog.Title = getTitle();
            messageDialog.Commands.Add(new UICommand(
                "Sí", new UICommandInvokedHandler(yesCommand)));
            messageDialog.Commands.Add(new UICommand(
                "No", new UICommandInvokedHandler(noCommand)));
            await messageDialog.ShowAsync();
            return _yes;
        }

        private void noCommand(IUICommand command)
        {
            _yes = false;
            noClick();
        }

        private void yesCommand(IUICommand command)
        {
            _yes = true;
            yesClick();
        }
    }
}
