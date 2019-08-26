using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RondasEcopetrol.Base;
using Windows.UI.Xaml.Navigation;
using System.Windows.Input;
using RondasEcopetrol.Views;
using System.Collections.ObjectModel;
using RondasEcopetrol.Models;
using RondasEcopetrol.PopUps;
using Windows.UI.Xaml;

namespace RondasEcopetrol.ViewModels
{
    public class CapturaDatos2ViewModel : ViewModelBase
    {
        public static bool NEXT_TRIGGER = true;
        public static bool INIT_STATE = false;
        private bool pickEnabled = false;

        public static CapturaDatos2ViewModel currentInstance;
        public CapturaDatos2ViewModel()
        {
            currentInstance = this;
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
            get { return GetPropertyValue<bool>(); }
            set
            {
                SetPropertyValue(value);
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
            get { return GetPropertyValue<bool>(); }
            set
            {
                SetPropertyValue(value);
            }
        }
        public bool IsEnabledPicker
        {
            get { return GetPropertyValue<bool>(); }
            set
            {
                SetPropertyValue(value);
            }
        }
        public DateTime ValorDatePicker
        {
            get { return GetPropertyValue<DateTime>(); }
            set
            {
                SetPropertyValue(value);
            }
        }
        #endregion Propiedades


        private DelegateCommand<string> _navigationCommand;
        private ICommand _aceptarCommand;
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

        public ICommand AceptarCommand
        {
            get { return _aceptarCommand = _aceptarCommand ?? new DelegateCommand(AceptarExecute); }
        }
        public ICommand CancelarCommand
        {
            get { return _cancelarCommand = _cancelarCommand ?? new DelegateCommand(CancelarExecute); }
        }
        private void AceptarExecute()
        {

        }
        private void CancelarExecute()
        {
            this.home();
        }

        public override Task OnNavigatedFrom(NavigationEventArgs args)
        {
            return null;
        }

        public override Task OnNavigatedTo(NavigationEventArgs args)
        {
            //initPanel();
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
            //    //this.noComment.Selected = false;
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
                //currFocus = null;
                if (pickEnabled)
                {
                    //txtValor.GotFocus -= this.pickHandler;
                    pickEnabled = false;
                }
                if (RondasLector.CurrentWork != null)
                {
                    this.Paso = RondasLector.CurrentWork.Step.Alias;
                    this.Tarea = (RondasLector.CurrentWork.Obligatorio ? "*" : "") + RondasLector.CurrentWork.Description;
                    if (RondasLector.CurrentWork.Tipo.Equals("VP CARACTER"))
                    {
                        if (RondasLector.CurrentWork.Values.Length == 0)
                        {
                            this.IsEnabledComboValor = false;
                            this.IsEnabledValorText = true;
                            //this.txtValor.GotFocus += this.pickHandler;
                            pickEnabled = true;
                            this.ValorText = RondasLector.CurrentWork.Valor;
                            ((CapturaDatos2)this.Page).txtValorText.Focus(FocusState.Programmatic);
                        }
                        else
                        {
                            this.IsEnabledComboValor = true;
                            this.IsEnabledValorText = false;
                            ((CapturaDatos2)this.Page).cmbValor.Focus(FocusState.Programmatic);
                            this.ValorCombo.Clear();
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
                        ((CapturaDatos2)this.Page).txtValorText.Focus(FocusState.Programmatic);
                        //currFocus = this.txtValor;
                        this.ValorText = RondasLector.CurrentWork.Valor;
                    }
                    this.Comentario = RondasLector.CurrentWork.Descripcion;
                    //this.noComment.Selected = RondasLector.CurrentWork.NoComment;
                    this.FechaOld1 = RondasLector.CurrentWork.OldValue1;
                    this.FechaOld2 = RondasLector.CurrentWork.OldValue2;
                    this.Unidad = "(" + RondasLector.CurrentWork.UM + ")";
                    this.NombreRonda = "Ronda: " + RondasLector.CurrentRonda.Nombre;
                    this.SelectedValueCausa = RondasLector.CurrentWork.Causa;
                }
            }
            catch (Exception)
            {
            }
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
            AppFrame.Navigate(typeof(CapturaDatos1));
        }
        public async void siguiente()
        {
            bool b = true;
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
                    RondasLector.CurrentWork.Valor = this.IsEnabledValorText ? this.ValorText : this.SelectedValueValorCombo;
                    RondasLector.CurrentWork.Descripcion = this.Comentario.Trim();
                    //RondasLector.CurrentWork.NoComment = this.noComment.Selected;
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
                        RondasFinalizarPopUp _popUp = new RondasFinalizarPopUp(this.AppFrame, true);
                        await _popUp.showAsync();
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
                        RondasLector.Step = (Steps)obj1;
                        AppFrame.Navigate(typeof(CapturaDatos1));
                        break;
                    }
                }
            }
        }
        private async Task<int[]> ValidarEntrada()
        {
            if (RondasLector.CurrentWork != null)
            {
                int returnValue = 0;
                string valor = this.IsEnabledValorText ? this.ValorText : this.SelectedValueValorCombo;
                if (valor.Length == 0 && this.Comentario.Trim().Length == 0)
                {
                    await MessageDialogError.ImprimirAsync("Debe digitar valor o un comentario");
                    ((CapturaDatos2)this.Page).txtValorText.Focus(FocusState.Programmatic);
                    //currFocus = txtValor;
                    return new int[] { 0, 0 };
                }
                if (RondasLector.CurrentWork.Obligatorio)
                {
                    if (valor.Length == 0 && this.Comentario.Trim().Length == 0)
                    {
                        await MessageDialogError.ImprimirAsync("El valor es obligatorio");
                        ((CapturaDatos2)this.Page).txtValorText.Focus(FocusState.Programmatic);
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
                    ((CapturaDatos2)this.Page).txtValorText.Focus(FocusState.Programmatic);
                    return new int[] { 0, 0 }; ;
                }
                else if (returnValue == 1)
                {
                    //if (!noComment.Selected && (Comentario.Trim().Length == 0 || this.SelectedValueCausa.Length == 0))
                    if (!false && (Comentario.Trim().Length == 0 || this.SelectedValueCausa.Length == 0))
                    {
                        if (Comentario.Trim().Length == 0)
                        {
                            ((CapturaDatos2)this.Page).txtComentario.Focus(FocusState.Programmatic);
                        }
                        else
                        {
                            ((CapturaDatos2)this.Page).cmbCausas.Focus(FocusState.Programmatic);
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
            bool showMsg = !false && (Comentario.Length == 0 || this.SelectedValueCausa.Length == 0);
            showMsg = true;
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
                            await MessageDialogError.ImprimirAsync(resultMsgDetail + ": " + resultMsgTitle);
                            return valorReturn;
                        }
                    }
                }
                return 2;
            }
            valorReturn = RondasLector.CurrentWork.validEntry(valor, showMsg, out resultMsgTitle, out resultMsgDetail);
            if (resultMsgTitle != null)
            {
                await MessageDialogError.ImprimirAsync(resultMsgDetail + ": " + resultMsgTitle);
            }
            return valorReturn;
        }
        public async void home()
        {
            RondasCancelarPopUp _popUp = new RondasCancelarPopUp(this.AppFrame, true);
            await _popUp.showAsync();
        }
    }
}
