using RondasEcopetrolWPF.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RondasEcopetrolWPF.Views;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using RondasEcopetrolWPF.Models;
using System.Windows.Input;

namespace RondasEcopetrolWPF.ViewModels
{
    public class TreeRoundViewModel : ViewModelBase
    {
        //public static TreeNode root;
        public static TreeViewItem root;
        private bool _cancelar;

        private ICommand _cancelarCommand;
        public ICommand CancelarCommand
        {
            get { return _cancelarCommand = _cancelarCommand ?? new DelegateCommand(CancelarExecute); }
        }
        private void CancelarExecute()
        {
            //AppFrame.Navigate(typeof(HacerRonda));
            _cancelar = true;
            ((TreeRound)this.Page).Treeview1.Items.Remove(root);
            Page.NavigationService.GoBack();
        }
        public override Task OnNavigatedTo(EventArgs args)
        {
            if (root != null)
            {
                ((TreeRound)this.Page).Treeview1.Items.Clear();
                ((TreeRound)this.Page).Treeview1.Items.Add(root);
            }
            ((TreeRound)this.Page).Treeview1.SelectedItemChanged += ListView_Click;
            return null;
        }
        private void ListView_Click(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!_cancelar)//sin esta validaciòn se muestre  el mensaje cada vez que se elimina la coleccion.
            {
                TreeViewItem item = (TreeViewItem)e.NewValue;
                object obj1 = item.Tag;
                if (obj1 is Steps)
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("Ir a este Paso?", "Paso", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    if (result == MessageBoxResult.OK)
                    {
                        RondasLector.Step = (Steps)obj1;
                        RondasLector.CurrentRonda.Current = obj1;
                        _cancelar = true;
                        ((TreeRound)this.Page).Treeview1.Items.Remove(root);
                        Navigated(typeof(CapturaDatos1));
                    }
                }
            }
        }
    }
}
