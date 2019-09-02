using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RondasEcopetrolWPF.Base
{
    public class MessageDialogError
    {
        public static async System.Threading.Tasks.Task ImprimirAsync(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //var messageDialog = new MessageDialog(message);
            //messageDialog.Title = "Error";
            //await messageDialog.ShowAsync();
        }

        public static async System.Threading.Tasks.Task ImprimirAsync(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            //var messageDialog = new MessageDialog(message);
            //messageDialog.Title = title;
            //await messageDialog.ShowAsync();
        }

    }
}
