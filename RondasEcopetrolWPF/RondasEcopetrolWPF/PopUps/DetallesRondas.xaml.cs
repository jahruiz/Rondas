﻿using System;
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
using RondasEcopetrolWPF.Models;

namespace RondasEcopetrolWPF.PopUps
{
    /// <summary>
    /// Lógica de interacción para DetallesRondas.xaml
    /// </summary>
    public partial class DetallesRondas : Window, IDisposable
    {
        private bool _respondOK { get; set; }
        public DetallesRondas(Object ronda)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.WindowStyle = WindowStyle.None; //sin barra de titulo

            if (ronda is RondaDescargada)
            {
                this.txtNombre.Text = ((RondaDescargada)ronda).Nombre;
                this.txtFecha.Text = ((RondaDescargada)ronda).Fecha_Gen;
                this.txtHora.Text = ((RondaDescargada)ronda).Hora_Gen;
                this.txtPlanta.Text = ((RondaDescargada)ronda).Planta;
                this.txtPuesto.Text = ((RondaDescargada)ronda).Puesto;
            }
            else if (ronda is RondaCompletada)
            {
                this.txtNombre.Text = ((RondaCompletada)ronda).name;
                this.txtFecha.Text = ((RondaCompletada)ronda).Fecha_Gen;
                this.txtHora.Text = ((RondaCompletada)ronda).Hora_Gen;
                this.txtPlanta.Text = ((RondaCompletada)ronda).Planta;
                this.txtPuesto.Text = ((RondaCompletada)ronda).Puesto;
            }
        }

        public bool mostrar()
        {
            _respondOK = false;
            this.ShowDialog();
            return _respondOK;
        }
        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            _respondOK = true;
            Close();
        }
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void Dispose()
        {
        }
    }
}
