using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RondasEcopetrol.Base;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using System.Windows.Input;
using RondasEcopetrol.Views;
using RondasEcopetrol.Models;
using RondasEcopetrol.PopUps;

namespace RondasEcopetrol.ViewModels
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
        private ICommand _guardarCommand;
        private ICommand _cancelarCommand;
        public DelegateCommand<string> NavigationCommand
        {
            get { return _navigationCommand = _navigationCommand ?? new DelegateCommand<string>(NavigationExecute); }
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

        public override Task OnNavigatedFrom(NavigationEventArgs args)
        {
            return null;
        }

        public override Task OnNavigatedTo(NavigationEventArgs args)
        {
            initPanel();
            return null;
        }

        //Codigo traido de StateMachine
        // Methods

        public async void suspender()
        {
            RondasSuspenderPopUp _popUp = new RondasSuspenderPopUp(this.AppFrame, false);
            await _popUp.showAsync();
        }

        //TODO Pendiente Buscar en àrbol
        /*public void buscar()
        {
            Cursor.Current = Cursors.WaitCursor;
            Cursor.Show();
            TreeNode node = RondasLector.CurrentRonda.getRoot();
            if (node != TreeRound.root)
            {
                TreeRound.root = node;
            }
            Cursor.Current = Cursors.Default;
            Cursor.Hide();
            base.Form.App.showCanvas(typeof(TreeRound));
        }*/

        public void showActual()
        {
            try
            {
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
        }

        public void anterior()
        {
            object obj1 = RondasLector.CurrentRonda.prev();
            if (obj1 == null)
            {
                RondasLector.CurrentRonda.Lector.Close();
                //TODO Definir a donde ir desde este punto
                AppFrame.Navigate(typeof(HacerRonda));
            }
            else if (obj1 is Work)
            {
                Work w = (Work)obj1;
                if (w.isValidForThisState())
                {
                    if (w.Step.isValid())
                    {
                        CapturaDatos2ViewModel.NEXT_TRIGGER = false;
                        RondasLector.CurrentWork = (Work)obj1;
                        AppFrame.Navigate(typeof(CapturaDatos2));
                        CapturaDatos2ViewModel.currentInstance.initPanel();
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

        //TODO Pendiente opcion de cortar ronda
        /*public void eliminar()
        {
            jump.fbased.Application application1 = base.Form.App;
            RondasAdvertenciaManager.sheet = true;
            AdvertenciaCutManager.sheet = false;
            AdvertenciaPopUp.manager = new AdvertenciaCutManager();
            application1.showCanvas(typeof(AdvertenciaPopUp));
        }*/

        public async void home()
        {
            RondasCancelarPopUp _popUp = new RondasCancelarPopUp(this.AppFrame, false);
            await _popUp.showAsync();
        }

        //TODO Mirar que se toma de aqui
        /*private void InitializeComponent()
        {
            ResourceManager manager1 = new ResourceManager(typeof(StateMachine));
            this.cmbEstado = new ComboBox();
            this.txtPaso = new TextBox();
            this.txtDireccion = new TextBox();
            this.txtCommentary = new TextBox();
            this.btnAceptar = new SButton();
            this.lblNameRonda = new Label();

            this.cmbEstado.Location = new Point(0x49, 0x3f + (0x17 * 3) + 6);
            this.cmbEstado.Size = new Size(0x9d, 0x18);
            this.cmbEstado.SelectedValueChanged += new EventHandler(this.itemChanged);

            this.txtCommentary.Location = new Point(0x49, 0x3f + (0x17 * 3) + 0x18 + 9);
            this.txtCommentary.BackColor = Color.White;
            this.txtCommentary.Multiline = true;
            this.txtCommentary.Size = new Size(0x9d, 0x17 * 2);
            this.txtCommentary.MaxLength = 2000;

            this.txtPaso.Location = new Point(0x49, 0x23);
            this.txtPaso.BackColor = Color.White;
            //this.txtPaso.ReadOnly = true;
            this.txtPaso.Multiline = true;
            this.txtPaso.Size = new Size(0x9d, 0x17 * 2);


            this.txtDireccion.Location = new Point(0x49, 0x3f + 0x17 + 3);
            this.txtDireccion.BackColor = Color.White;
            //this.txtPaso.ReadOnly = true;
            this.txtDireccion.Multiline = true;
            this.txtDireccion.Size = new Size(0x9d, 0x17 * 2);

            this.btnAceptar.Location = new Point(0x7a, 0xc9 + 15);
            this.btnAceptar.ForeColor = Color.White;
            this.btnAceptar.Size = new Size(0x6b, 30);
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.Click += new EventHandler(this.aceptarClick);

            this.lblNameRonda.Font = new Font("Verdana", 8.25f, FontStyle.Regular);
            this.lblNameRonda.Bounds = new Rectangle(6, 0xde + 15 + 10, 0xe2 * 3, 15);
            this.lblNameRonda.BackColor = Color.FromArgb(222, 222, 222);

            base.Fondo = (Image)manager1.GetObject("fondo");
            base.Controls.Add(this.btnAceptar);
            base.Controls.Add(this.cmbEstado);
            base.Controls.Add(this.txtPaso);
            base.Controls.Add(this.txtDireccion);
            base.Controls.Add(this.lblNameRonda);
            base.Controls.Add(this.txtCommentary);
            base.Size = new Size(240, 0x115);

            this.txtPaso.TextChanged += new EventHandler(this.txtUnChanged);
            this.txtDireccion.TextChanged += new EventHandler(this.txtUnChanged);
            base.KeyDown += new KeyEventHandler(this.keyImpl);
        }*/

        public void initPanel()
        {
            //TODO Pendiente Buscar en Arbol de la ronda
            /*SToolBar bar1 = base.Form.Barra;
            bar1.Editar = false;
            bar1.Primero = false;
            bar1.Ultimo = false;
            if (step == StartStep) bar1.Anterior = false;

            bar1.Buscar = RondasLector.CurrentRonda.Show_tree;*/
            showActual();
        }

        /*protected void itemChanged(object sender, EventArgs a)
        {
            base.Focus();
        }*/

        /*protected void keyImpl(object sender, KeyEventArgs a)
        {
            if (a.KeyCode == Keys.Down)
            {
                int num1 = this.cmbEstado.Items.Count;
                int num2 = this.cmbEstado.SelectedIndex;
                if (num2 == (num1 - 1))
                {
                    this.cmbEstado.SelectedIndex = 0;
                }
                else
                {
                    this.cmbEstado.SelectedIndex = num2 + 1;
                }
            }
            else if (a.KeyCode == Keys.Up)
            {
                int num3 = this.cmbEstado.Items.Count;
                int num4 = this.cmbEstado.SelectedIndex;
                if (num4 == -1) num4 = 0;
                if (num4 == 0)
                {
                    this.cmbEstado.SelectedIndex = num3 - 1;
                }
                else
                {
                    this.cmbEstado.SelectedIndex = num4 - 1;
                }
            }
            else if (a.KeyCode == Keys.Left)
            {
                this.anterior();
            }
            else if (a.KeyCode == Keys.Right)
            {
                this.siguiente();
            }
        }*/

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
                    ((CapturaDatos1)this.Page).txtCommentary.Focus(FocusState.Programmatic);
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
                        AppFrame.Navigate(typeof(CapturaDatos2));
                        CapturaDatos2ViewModel.currentInstance.initPanel();
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
                            RondasFinalizarPopUp _popUp = new RondasFinalizarPopUp(this.AppFrame, false);
                            if (await _popUp.showAsync())
                            {
                                //Ir al menú principal
                                AppFrame.Navigate(typeof(MainPage));
                            }
                        }
                    }
                }
            }
        }
    }
}
