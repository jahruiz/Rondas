using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace RondasEcopetrol.Base
{
    public class MessageDialogError
    {
        public static async System.Threading.Tasks.Task ImprimirAsync(string message)
        {
            var messageDialog = new MessageDialog(message);
            messageDialog.Title = "Error";
            await messageDialog.ShowAsync();
        }

        public static async System.Threading.Tasks.Task ImprimirAsync(string message, string title)
        {
            var messageDialog = new MessageDialog(message);
            messageDialog.Title = title;
            await messageDialog.ShowAsync();
        }

    }
}
