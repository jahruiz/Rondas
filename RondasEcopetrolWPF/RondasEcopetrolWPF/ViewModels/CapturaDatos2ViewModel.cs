namespace RondasEcopetrolWPF.ViewModels
{
    using System;
    using System.Windows;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using RondasEcopetrolWPF.Base;
    using System.Windows.Input;
    using RondasEcopetrolWPF.Views;
    using System.Collections.ObjectModel;
    using RondasEcopetrolWPF.Models;
    using RondasEcopetrolWPF.PopUps;
    public class CapturaDatos2ViewModel : ViewModelBase
    {
        public static bool NEXT_TRIGGER = true;
        public static bool INIT_STATE = false;

        public CapturaDatos2ViewModel()
        {
            //initPanel();
        }
        #region Propiedades
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
        public string Tarea
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

        public bool SinComentario
        {
            get { return GetPropertyValue<bool>(); }
            set
            {
                SetPropertyValue(value);
                IsComentarioEnabled = !value;
            }
        }
        public bool IsComentarioEnabled
        {
            get { return GetPropertyValue<bool>(); }
            set
            {
                SetPropertyValue(value);
                if (!value)
                    Comentario = "";
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
        public string Unidad
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }
        public string FechaOld1
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }
        public string FechaOld2
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }

        public ObservableCollection<string> Causa
        {
            get { return GetPropertyValue<ObservableCollection<string>>(); }
            set
            {
                SetPropertyValue(value);
            }
        }

        public int SelectedIndexCausa
        {
            get { return GetPropertyValue<int>(); }
            set
            {
                SetPropertyValue(value);
            }
        }

        public string SelectedValueCausa
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }
        public string ValorText
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }
        public bool IsEnabledValorText
        {
            get { return ((CapturaDatos2)this.Page).txtValorText.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                    ((CapturaDatos2)this.Page).txtValorText.Visibility = Visibility.Visible;
                else
                    ((CapturaDatos2)this.Page).txtValorText.Visibility = Visibility.Hidden;
            }
        }
        public ObservableCollection<string> ValorCombo
        {
            get { return GetPropertyValue<ObservableCollection<string>>(); }
            set
            {
                SetPropertyValue(value);
            }
        }

        public int SelectedIndexValorCombo
        {
            get { return GetPropertyValue<int>(); }
            set
            {
                SetPropertyValue(value);
            }
        }

        public string SelectedValueValorCombo
        {
            get { return GetPropertyValue<string>(); }
            set
            {
                SetPropertyValue(value);
            }
        }
        public bool IsEnabledComboValor
        {
            get { return ((CapturaDatos2)this.Page).cmbValor.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                    ((CapturaDatos2)this.Page).cmbValor.Visibility = Visibility.Visible;
                else
                    ((CapturaDatos2)this.Page).cmbValor.Visibility = Visibility.Hidden;
            }
        }
        public bool IsEnabledPicker
        {
            get { return ((CapturaDatos2)this.Page).dteValor.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                    ((CapturaDatos2)this.Page).dteValor.Visibility = Visibility.Visible;
                else
                    ((CapturaDatos2)this.Page).dteValor.Visibility = Visibility.Hidden;
            }
        }
        public DateTime? ValorDatePicker
        {
            get { return GetPropertyValue<DateTime?>(); }
            set
            {
                SetPropertyValue(value);
            }
        }
        #endregion Propiedades


        private DelegateCommand<string> _navigationCommand;
        private ICommand _guardarCommand;
        private ICommand _cancelarCommand;
        public DelegateCommand<string> NavigationCommand
        {
            get { return _navigationCommand = _navigationCommand ?? new DelegateCommand<string>(NavigationExecute); }
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

        public ICommand GuardarCommand
        {
            get { return _guardarCommand = _guardarCommand ?? new DelegateCommand(GuardarExecute); }
        }
        public ICommand CancelarCommand
        {
            get { return _cancelarCommand = _cancelarCommand ?? new DelegateCommand(CancelarExecute); }
        }
        private void GuardarExecute()
        {
            suspender();
        }
        private void CancelarExecute()
        {
            this.home();
        }

        //public override Task OnNavigatedFrom(NavigationEventArgs args)
        //{
        //    return null;
        //}

        public override Task OnNavigatedTo(EventArgs args)
        {
            initPanel();
            return null;
        }
        private void CargarComboCausa()
        {
            Causa = new ObservableCollection<string>();
            Causa.Clear();
            this.Causa.Add(" ");
            this.Causa.Add("Limites");
            this.Causa.Add("Proceso");
            this.Causa.Add("Instrumento");
        }
        public void initPanel()
        {
            //    //SToolBar barra = this.Form.Barra;
            //    //barra.Editar = false;
            //    //barra.Primero = false;
            //    //barra.Ultimo = false;
            //    //barra.Buscar = Sheet.CurrentRonda.Show_tree;
            //    //this.Form.Text = "Rondas Pocket";
            this.SinComentario = false;
            this.CargarComboCausa();
            if (NEXT_TRIGGER)
            {
                this.siguiente();
            }
            else
            {
                NEXT_TRIGGER = true;
                this.showActual();
            }
        }
        public void showActual()
        {
            try
            {
                if (RondasLector.CurrentWork != null)
                {
                    this.NombreRonda = RondasLector.CurrentRonda.Nombre;
                    this.RefPaso = "Paso " + RondasLector.Step.Orden + " de " + RondasLector.CurrentRonda.TotalPasos;

                    this.Paso = RondasLector.CurrentWork.Step.Alias;
                    this.Tarea = (RondasLector.CurrentWork.Obligatorio ? "*" : "") + RondasLector.CurrentWork.Description;
                    if (RondasLector.CurrentWork.Tipo.Equals("VP CARACTER"))
                    {
                        if (RondasLector.CurrentWork.Values.Length == 0)
                        {
                            this.IsEnabledComboValor = false;
                            this.IsEnabledValorText = false;
                            this.IsEnabledPicker = true;
                            try
                            {
                                this.ValorDatePicker = DateTime.ParseExact(RondasLector.CurrentWork.Valor, "yyyy-MM-dd HH:mm", null);
                            }
                            catch (Exception)
                            {
                                this.ValorDatePicker = null;
                            }
                            ((CapturaDatos2)this.Page).dteValor.Focus();
                        }
                        else
                        {
                            this.IsEnabledComboValor = true;
                            this.IsEnabledValorText = false;
                            this.IsEnabledPicker = false;
                            ((CapturaDatos2)this.Page).cmbValor.Focus();
                            this.ValorCombo = new ObservableCollection<string>();
                            //this.ValorCombo.Clear();
                            for (int num1 = 0; num1 < (RondasLector.CurrentWork.Values.Length - 1); num1++)
                            {
                                this.ValorCombo.Add(RondasLector.CurrentWork.Values[num1]);
                            }
                            this.SelectedValueValorCombo = RondasLector.CurrentWork.Valor;
                        }
                    }
                    else
                    {
                        this.IsEnabledComboValor = false;
                        this.IsEnabledValorText = true;
                        this.IsEnabledPicker = false;
                        ((CapturaDatos2)this.Page).txtValorText.Focus();
                        this.ValorText = RondasLector.CurrentWork.Valor;
                    }
                    this.Comentario = RondasLector.CurrentWork.Descripcion;
                    this.SinComentario = RondasLector.CurrentWork.NoComment;
                    this.FechaOld1 = RondasLector.CurrentWork.OldValue1;
                    this.FechaOld2 = RondasLector.CurrentWork.OldValue2;
                    this.Unidad = "Valor (" + RondasLector.CurrentWork.UM + ")";
                    this.NombreRonda = "Ronda: " + RondasLector.CurrentRonda.Nombre;
                    this.SelectedValueCausa = RondasLector.CurrentWork.Causa;
                }
            }
            catch (Exception)
            {
            }
        }
        public void suspender()
        {
            RondasSuspenderPopUp _popUp = new RondasSuspenderPopUp(this, true);
            _popUp.showAsync();
        }

        public void anterior()
        {
            object obj;
            do
            {
                obj = RondasLector.CurrentRonda.prev();
                if (obj == null)
                {
                    //RondasAdvertenciaManager.sheet = true;
                    //this.Form.App.showCanvas(typeof(AdvertenciaPopUp));
                    RondasFinalizarPopUp _popUp = new RondasFinalizarPopUp(this, true);
                    if (_popUp.showAsync())
                    {
                        //Ir al menú principal
                        //AppFrame.Navigate(typeof(MainPage));
                        Navigated(typeof(MainPage));
                    }
                    return;
                }
                else if (obj is Work && ((Work)obj).isValidForThisState())
                {
                    RondasLector.CurrentWork = (Work)obj;
                    this.showActual();
                    return;
                }
            }
            while (!(obj is Steps));
            RondasLector.CurrentWork = (Work)null;
            RondasLector.Step = (Steps)obj;
            //this.Form.App.showCanvas(typeof(StateMachine));
            //AppFrame.Navigate(typeof(CapturaDatos1));
            Navigated(typeof(CapturaDatos1));
        }
        public async void siguiente()
        {
            bool b = true;
            bool prevEquipment = INIT_STATE;
            int value_Validation = 0;
            if (!INIT_STATE)
            {
                int[] result = await this.ValidarEntrada();
                value_Validation = result[0];
                b = (result[1] == 1);
            }
            INIT_STATE = false;
            if (b)
            {
                if (RondasLector.CurrentWork != null)
                {
                    RondasLector.CurrentWork.Valor = this.getValor();
                    RondasLector.CurrentWork.Descripcion = this.Comentario.Trim();
                    RondasLector.CurrentWork.NoComment = this.SinComentario;
                    RondasLector.CurrentWork.Causa = this.SelectedValueCausa;
                    RondasLector.CurrentWork.fechar();
                }
                while (true)
                {
                    object obj1 = RondasLector.CurrentRonda.next();
                    if (obj1 == null)
                    {
                        //RondasAdvertenciaManager.sheet = true;
                        //base.Form.App.showCanvas(typeof(AdvertenciaPopUp));
                        RondasFinalizarPopUp _popUp = new RondasFinalizarPopUp(this, !prevEquipment);
                        if (_popUp.showAsync())
                        {
                            //Ir al menú principal
                            //AppFrame.Navigate(typeof(MainPage));
                            Navigated(typeof(MainPage));
                        }
                        break;
                    }
                    else if (obj1 is Work && ((Work)obj1).isValidForThisState())
                    {
                        RondasLector.CurrentWork = (Work)obj1;
                        this.showActual();
                        break;
                    }
                    else if (obj1 is Steps)
                    {
                        RondasLector.CurrentWork = (Work)null;
                        RondasLector.Step = (Steps)obj1;
                        //AppFrame.Navigate(typeof(CapturaDatos1));
                        Navigated(typeof(CapturaDatos1));
                        break;
                    }
                }
            }
        }

        private string getValor()
        {
            if (this.IsEnabledPicker)
            {
                return this.ValorDatePicker.HasValue ? this.ValorDatePicker.Value.ToString("yyyy-MM-dd HH:mm") : "";
            }
            else
                return this.IsEnabledValorText ? this.ValorText : this.SelectedValueValorCombo;
        }
        private async Task<int[]> ValidarEntrada()
        {
            if (RondasLector.CurrentWork != null)
            {
                int returnValue = 0;
                string valor = this.getValor();
                if (valor.Length == 0 && this.Comentario.Trim().Length == 0)
                {
                    await MessageDialogError.ImprimirAsync("Debe digitar valor o un comentario");
                    ((CapturaDatos2)this.Page).txtValorText.Focus();
                    //currFocus = txtValor;
                    return new int[] { 0, 0 };
                }
                if (RondasLector.CurrentWork.Obligatorio)
                {
                    if (valor.Length == 0 && this.Comentario.Trim().Length == 0)
                    {
                        await MessageDialogError.ImprimirAsync("El valor es obligatorio");
                        ((CapturaDatos2)this.Page).txtValorText.Focus();
                        //currFocus = txtValor;
                        return new int[] { 0, 0 };
                    }
                    if (valor.Length == 0) return new int[] { 0, 1 };
                }
                if (valor.Length != 0)
                {
                    returnValue = await this.validValue(valor);
                }
                if (returnValue == 0)
                {
                    ((CapturaDatos2)this.Page).txtValorText.Focus();
                    return new int[] { 0, 0 }; ;
                }
                else if (returnValue == 1)
                {
                    //if (!noComment.Selected && (Comentario.Trim().Length == 0 || this.SelectedValueCausa.Length == 0))
                    if (!this.SinComentario && (Comentario.Trim().Length == 0 || this.SelectedValueCausa.Length == 0))
                    {
                        if (Comentario.Trim().Length == 0)
                        {
                            ((CapturaDatos2)this.Page).txtComentario.Focus();
                        }
                        else
                        {
                            ((CapturaDatos2)this.Page).cmbCausas.Focus();
                        }
                        return new int[] { returnValue, 0 };
                    }
                }
                return new int[] { returnValue, 1 };
            }
            return new int[] { 0, 1 };
        }
        private async Task<int> validValue(string valor)
        {
            //bool showMsg = !noComment.Selected && (Comentario.Length == 0 || this.SelectedValueCausa.Length == 0);
            bool showMsg = !this.SinComentario && (Comentario.Length == 0 || this.SelectedValueCausa.Length == 0);
            //showMsg = true; //Linea para debug
            int valorReturn = 0;
            string resultMsgTitle = null, resultMsgDetail = null;
            if (RondasLector.CurrentWork.Tipo.Equals("VP CARACTER"))
            {
                if (this.IsEnabledComboValor)
                {
                    int index = this.SelectedIndexValorCombo;
                    string value = "" + this.SelectedValueValorCombo;
                    if (index != -1)
                    {
                        valorReturn = RondasLector.CurrentWork.validEntryText(index, value, showMsg, out resultMsgTitle, out resultMsgDetail);
                        if (resultMsgTitle != null)
                        {
                            await MessageDialogError.ImprimirAsync(resultMsgDetail, resultMsgTitle);
                        }
                        return valorReturn;
                    }
                }
                return 2;
            }
            valorReturn = RondasLector.CurrentWork.validEntry(valor, showMsg, out resultMsgTitle, out resultMsgDetail);
            if (resultMsgTitle != null)
            {
                await MessageDialogError.ImprimirAsync(resultMsgDetail, resultMsgTitle);
            }
            return valorReturn;
        }
        public void home()
        {
            RondasCancelarPopUp _popUp = new RondasCancelarPopUp(this, true);
            _popUp.showAsync();
        }
    }
}
