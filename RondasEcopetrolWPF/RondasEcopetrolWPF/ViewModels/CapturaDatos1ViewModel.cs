using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RondasEcopetrolWPF.Base;
using System.Windows.Input;
using RondasEcopetrolWPF.Views;
using RondasEcopetrolWPF.Models;
using RondasEcopetrolWPF.PopUps;
using System.Windows;
using System.Windows.Controls;

namespace RondasEcopetrolWPF.ViewModels
{
    public class CapturaDatos1ViewModel : ViewModelBase
    {
        public CapturaDatos1ViewModel()
        {
            //initPanel();
        }

        #region Propiedades
        //Campos del Step mostrado actualmente
        public string Paso
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }

        public string RefPaso
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }

        public string Direccion
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }

        public string Comentario
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }

        public string NombreRonda
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }

        public ObservableCollection<string> EstadosEquipo
        {
            get { return GetPropertyValue<ObservableCollection<string>>(); }
            set
            {
                SetPropertyValue(value);
            }
        }

        public int SelectedIndexEstado
        {
            get { return GetPropertyValue<int>(); }
            set
            {
                SetPropertyValue(value);
            }
        }

        public string SelectedValueEstado
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }

        #endregion Propiedades

        #region Commands
        private DelegateCommand<string> _navigationCommand;
        private ICommand _cortarCommand;
        private ICommand _guardarCommand;
        private ICommand _cancelarCommand;
        public DelegateCommand<string> NavigationCommand
        {
            get { return _navigationCommand = _navigationCommand ?? new DelegateCommand<string>(NavigationExecute); }
        }
        public ICommand CortarCommand
        {
            get { return _cortarCommand = _cortarCommand ?? new DelegateCommand(CortarExecute); }
        }
        public ICommand GuardarCommand
        {
            get { return _guardarCommand = _guardarCommand ?? new DelegateCommand(GuardarExecute); }
        }
        public ICommand CancelarCommand
        {
            get { return _cancelarCommand = _cancelarCommand ?? new DelegateCommand(CancelarExecute); }
        }
        private void NavigationExecute(string viewFrame)
        {
            switch (viewFrame)
            {
                case "Anterior":
                    this.anterior();
                    break;
                case "Siguiente":
                    this.siguiente();
                    break;
            }
        }

        private void CortarExecute()
        {
            cortar();
        }
        private void GuardarExecute()
        {
            suspender();
        }
        private void CancelarExecute()
        {
            //AppFrame.Navigate(typeof(HacerRonda));
            this.home();
        }
        #endregion Commands

        //public override Task OnNavigatedFrom(NavigationEventArgs args)
        //{
        //    return null;
        //}

        public override Task OnNavigatedTo(EventArgs args)
        {
            initPanel();
            return null;
        }

        //Codigo traido de StateMachine
        // Methods

        public void suspender()
        {
            RondasSuspenderPopUp _popUp = new RondasSuspenderPopUp(this, false);
            _popUp.showAsync();
        }

        public void buscar(object sender, RoutedEventArgs e)
        {
            TreeViewItem node = RondasLector.CurrentRonda.getRoot();
            if (node != TreeRound.root)
            {
                TreeRound.root = node;
            }
            using (TreeRound treeround = new TreeRound())
            {
                treeround.ShowDialog();
            }
            showActual();
        }

        public void showActual()
        {
            try
            {
                this.NombreRonda = RondasLector.CurrentRonda.Nombre;
                this.RefPaso = "Paso " + RondasLector.Step.Orden + " de " + RondasLector.CurrentRonda.TotalPasos;

                this.Paso = RondasLector.Step.Alias;
                string[] textArray1 = RondasLector.Step.States;
                //this.EstadosEquipo.Clear();
                this.EstadosEquipo = new ObservableCollection<string>();
                for (int num1 = 0; num1 < (textArray1.Length - 1); num1++)
                {
                    this.EstadosEquipo.Add(textArray1[num1]);
                }
                Steps temp = RondasLector.Step;
                if (!RondasLector.Step.isFather)
                {
                    temp = RondasLector.CurrentRonda.getStepByAlias(RondasLector.Step.Alias);
                    if (temp == null) temp = RondasLector.Step;
                }

                if (temp.SelectedValue == -1)
                    this.SelectedValueEstado = "";
                else
                {
                    this.SelectedIndexEstado = temp.SelectedValue;
                    this.SelectedValueEstado = this.EstadosEquipo[temp.SelectedValue];
                }

                this.Direccion = temp.Direccion;
                this.Comentario = temp.Commentary;
                this.NombreRonda = "Ronda: " + RondasLector.CurrentRonda.Nombre;
            }
            catch (Exception)
            {
            }
            ((CapturaDatos1)this.Page).btnAnterior.IsEnabled = !RondasLector.Step.Equals(RondasLector.StartStep);
        }

        public void anterior()
        {
            object obj1 = RondasLector.CurrentRonda.prev();
            if (obj1 == null)
            {
                RondasLector.CurrentRonda.Lector.Close();
                //TODO Definir a donde ir desde este punto
                //AppFrame.Navigate(typeof(HacerRonda));
                Navigated(typeof(HacerRonda));
            }
            else if (obj1 is Work)
            {
                Work w = (Work)obj1;
                if (w.isValidForThisState())
                {
                    if (w.Step.isValid())
                    {
                        CapturaDatos2ViewModel.NEXT_TRIGGER = false;
                        RondasLector.Step = w.Step;
                        RondasLector.CurrentWork = (Work)obj1;
                        //AppFrame.Navigate(typeof(CapturaDatos2));
                        Navigated(typeof(CapturaDatos2));
                        //CapturaDatos2ViewModel.currentInstance.initPanel();
                    }
                    else
                    {
                        RondasLector.Step = w.Step;
                        RondasLector.CurrentRonda.Current = w.Step;
                        showActual();
                    }
                }
                else
                {
                    this.anterior();
                    return;
                }
            }
            else if (obj1 is Steps)
            {
                RondasLector.Step = (Steps)obj1;
                showActual();
            }
        }

        public void cortar()
        {
            CortarRondaViewModel.Navigate(this);
        }

        public void home()
        {
            RondasCancelarPopUp _popUp = new RondasCancelarPopUp(this, false);
            _popUp.showAsync();
        }

        public void initPanel()
        {
            if (RondasLector.CurrentRonda.Show_tree)
            {
                ((CapturaDatos1)this.Page).btnBuscar.Visibility = Visibility.Visible;
                ((CapturaDatos1)this.Page).btnBuscar.Click += buscar;
            }
            showActual();
        }

        public async void siguiente()
        {
            string text1 = this.SelectedValueEstado;
            if (text1.Length == 0)
            {
                await MessageDialogError.ImprimirAsync("Usted debe seleccionar un estado");
            }
            else
            {
                if ((text1.Equals("OF") || text1.Equals("SF") || text1.Equals("EF")) && this.Comentario.Trim().Length == 0)
                {
                    await MessageDialogWarning.ImprimirAsync("Se sugiere documentar");
                    //((CapturaDatos1)this.Page).txtCommentary.Focus(FocusState.Programmatic);
                    ((CapturaDatos1)this.Page).txtCommentary.Focus();
                }
                else
                {
                    RondasLector.Step.SelectedValue = this.SelectedIndexEstado;
                    RondasLector.Step.Commentary = this.Comentario;
                    RondasLector.Step.fechar();
                    //Ir al siguiente nodo (Paso/Tarea) de la ronda
                    CapturaDatos2ViewModel.INIT_STATE = true;
                    if (RondasLector.Step.Works.Count > 0)
                    {
                        /*//AppFrame.Navigate(typeof(CapturaDatos2));
                        Navigated(typeof(CapturaDatos2));
                        //CapturaDatos2ViewModel.currentInstance.initPanel();*/
                        CapturaDatos2ViewModel.navigateNext(this);
                    }
                    else
                    {
                        object obj1 = RondasLector.CurrentRonda.next();
                        if (obj1 != null)
                        {
                            RondasLector.Step = (Steps)obj1;
                            showActual();
                        }
                        else
                        {
                            RondasLector.CurrentRonda.Current = RondasLector.Step;
                            RondasLector.EndObj = RondasLector.Step;
                            RondasFinalizarPopUp _popUp = new RondasFinalizarPopUp(this, false);
                            if (_popUp.showAsync())
                            {
                                //Ir al menú principal
                                //AppFrame.Navigate(typeof(MainPage));
                                Navigated(typeof(MainPage));
                            }
                        }
                    }
                }
            }
        }
    }
}
