using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace RondasEcopetrol.Base
{
    class MessageDialogWarning
    {
        public static async Task ImprimirAsync(string message)
        {
            var messageDialog = new MessageDialog(message);
            messageDialog.Title = "Advertencia";
            await messageDialog.ShowAsync();
        }
    }
}
