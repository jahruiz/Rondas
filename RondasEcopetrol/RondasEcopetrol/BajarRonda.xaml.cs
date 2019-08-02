using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace RondasEcopetrol
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class BajarRonda : Page
    {
        public BajarRonda()
        {
            this.InitializeComponent();
        }

        public async System.Threading.Tasks.Task ClickItemListAsync(object sender, ItemClickEventArgs e)
        {
            string clickedItemText = e.ClickedItem.ToString();
            string textoRonda = "";

            System.Diagnostics.Debug.WriteLine(clickedItemText);

            if (clickedItemText == "OPERADOR ANALISTA (06:00)")
            {
                textoRonda =
                    clickedItemText +"\n" + "\n" +
                    "Fecha: 02/08/2019" + "\n" +
                    "Hora: 06:00" + "\n" +
                    "Planta:U-111" + "\n" +
                    "Puesto:PSI:Patio Unidad U-111";
            }
            else if (clickedItemText == "OPERADOR ANALISTA (14:00)")
            {
                textoRonda =
                clickedItemText + "\n" + "\n" +
                "Fecha: 02/08/2019" + "\n" +
                "Hora: 14:00" + "\n" +
                "Planta:U-112" + "\n" +
                "Puesto:PSI:Patio Unidad U-112";
            }
            else if (clickedItemText == "ANALISTA U-850 (06:00)")
            {
                textoRonda =
                clickedItemText + "\n" + "\n" +
                "Fecha: 02/08/2019" + "\n" +
                "Hora: 06:00" + "\n" +
                "Planta:U-113" + "\n" +
                "Puesto:PSI:Patio Unidad U-113";
            }
            else if (clickedItemText == "ANALISTA U-850 (14:00)")
            {
                textoRonda =
                clickedItemText + "\n" + "\n" +
                "Fecha: 02/08/2019" + "\n" +
                "Hora: 14:00" + "\n" +
                "Planta:U-114" + "\n" +
                "Puesto:PSI:Patio Unidad U-114";
            }
            else if (clickedItemText == "OPERADOR 850 PATIO (06:00)")
            {
                textoRonda =
                clickedItemText + "\n" + "\n" +
                "Fecha: 02/08/2019" + "\n" +
                "Hora: 06:00" + "\n" +
                "Planta:U-115" + "\n" +
                "Puesto:PSI:Patio Unidad U-115";
            }
            else if (clickedItemText == "OPERADOR 850 PATIO (14:00)")
            {
                textoRonda =
                clickedItemText + "\n" + "\n" +
                "Fecha: 02/08/2019" + "\n" +
                "Hora: 14:00" + "\n" +
                "Planta:U-116" + "\n" +
                "Puesto:PSI:Patio Unidad U-116";
            }
            await ImprimirAsync(textoRonda);
            //var messageDialog = new MessageDialog(clickedItemText);
            //await messageDialog.ShowAsync();
        }

        public async System.Threading.Tasks.Task ImprimirAsync(string texto)
        {
            var messageDialog = new MessageDialog(texto);
            messageDialog.Commands.Add(new UICommand(
                "Descargar", new UICommandInvokedHandler( this.CommandInvokedHandler)));
            messageDialog.Commands.Add(new UICommand(
                "Cancelar"));
            await messageDialog.ShowAsync();
        } 

        private async void CommandInvokedHandler(IUICommand command)
        {
            await DescargaAsync();
        }

        public async System.Threading.Tasks.Task DescargaAsync()
        {
            var messageDialog = new MessageDialog("Descargando ronda estructurada del sistema RIS...");
            messageDialog.Commands.Add(new UICommand(
                "Cancelar"));
            await messageDialog.ShowAsync();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
        }


        private void Button_Update(object sender, RoutedEventArgs e)
        {

        }
    }
}
