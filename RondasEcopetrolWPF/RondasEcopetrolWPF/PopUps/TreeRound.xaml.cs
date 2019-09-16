using RondasEcopetrolWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RondasEcopetrolWPF.PopUps
{
    /// <summary>
    /// Lógica de interacción para TreeRound.xaml
    /// </summary>
    public partial class TreeRound : Window, IDisposable
    {
        public static TreeViewItem root;
        private bool _cancelar;
		public bool NavegarCapturadatos1;//permita navegar a capturadatos1 o no.
		
        public TreeRound()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.WindowStyle = WindowStyle.None; //sin barra de titulo
        }

        
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            _cancelar = true;
            Treeview1.Items.Clear();
            this.Close();
        }
        public void Dispose()
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (root != null)
            {
                Treeview1.Items.Clear();
                Treeview1.Items.Add(root);
            }
        }

        private void Treeview1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
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
                        NavegarCapturadatos1 = true;
                        Treeview1.Items.Clear();
                        this.Close();
                    }
                    else
                        ((Treeview1.Items[0] as TreeViewItem)).IsSelected=true;
                }               
            }
        }
    }
}
