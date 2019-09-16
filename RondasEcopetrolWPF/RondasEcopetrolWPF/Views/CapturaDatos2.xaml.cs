using RondasEcopetrolWPF.Base;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace RondasEcopetrolWPF.Views
{
    /// <summary>
    /// Lógica de interacción para CapturaDatos2.xaml
    /// </summary>
    public partial class CapturaDatos2 : PageBase
    {
        public CapturaDatos2()
        {
            InitializeComponent();
        }
        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;

                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);

                    if (foundChild != null) break;
                }
                else
                    if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;

                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                    else
                    {
                        foundChild = FindChild<T>(child, childName);

                        if (foundChild != null)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        private void PageBase_Loaded(object sender, RoutedEventArgs e)
        {
            // Find the Popup in template
            Popup MyPopup = FindChild<Popup>(dteValor, "PART_Popup");

            // Get Calendar in child of Popup
            Calendar MyCalendar = (Calendar)MyPopup.Child;

            MyCalendar.LayoutTransform = new ScaleTransform(1.7,1.7);
            
            // For test

        }
    }
}
