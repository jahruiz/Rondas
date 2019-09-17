using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RondasEcopetrolWPF.Base
{
    class MessageDialogWarning
    {
        public static void ImprimirAsync(string message)
        {
            MessageBox.Show(message, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            //var messageDialog = new MessageDialog(message);
            //messageDialog.Title = "Advertencia";
            //await messageDialog.ShowAsync();
        }
        public static void ImprimirAsync(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
            //var messageDialog = new MessageDialog(message);
            //messageDialog.Title = "Advertencia";
            //await messageDialog.ShowAsync();
        }
    }
}
