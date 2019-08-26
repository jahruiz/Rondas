﻿using System;
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
            this.Causa.Add("");
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
                //this.currFocus = (TextBox)null;
                //if (IsEnabledPicker) 
                if (pickEnabled)
                {
                    //this.txtValor.GotFocus -= this.pickHandler;
                    this.pickEnabled = false;
                }
                if (RondasLector.CurrentWork == null)
                    return;
                Paso = RondasLector.CurrentWork.Step.Alias;
                Tarea = (RondasLector.CurrentWork.Obligatorio ? "*" : "") + RondasLector.CurrentWork.Description;
                if (RondasLector.CurrentWork.Tipo.Equals("VP CARACTER"))
                {
                    if (RondasLector.CurrentWork.Values.Length == 0)
                    {
                        this.IsEnabledComboValor = false;
                        this.IsEnabledValorText = true;
                        //this.txtValor.GotFocus += this.pickHandler;
                        //this.IsEnabledPicker = true;
                        this.pickEnabled = true;
                        this.ValorText = RondasLector.CurrentWork.Valor;
                        //this.Focus();
                        ((CapturaDatos2)this.Page).txtValorText.Focus(FocusState.Programmatic);
                    }
                    else
                    {
                        this.IsEnabledComboValor = true;
                        this.IsEnabledValorText = false;
                        this.ValorCombo = new ObservableCollection<string>();
                        for (int index = 0; index < RondasLector.CurrentWork.Values.Length - 1; ++index)
                        {
                            this.ValorCombo.Add(RondasLector.CurrentWork.Values[index]);
                        }
                        this.SelectedValueValorCombo = RondasLector.CurrentWork.Valor;
                    }
                }
                else
                {
                    this.IsEnabledComboValor = false;
                    this.IsEnabledValorText = true;
                    ((CapturaDatos2)this.Page).txtValorText.Focus(FocusState.Programmatic);
                    //this.txtValor.Focus();
                    //this.currFocus = this.txtValor;
                    this.ValorText = RondasLector.CurrentWork.Valor;
                }
                this.Comentario = RondasLector.CurrentWork.Description;
                //this.noComment.Selected = Sheet.current.NoComment;
                this.FechaOld1 = RondasLector.CurrentWork.OldValue1;
                this.FechaOld2 = RondasLector.CurrentWork.OldValue2;
                this.Unidad = "(" + RondasLector.CurrentWork.UM + ")";
                this.NombreRonda = "Ronda: " + RondasLector.CurrentWork.Nombre;
                this.SelectedValueCausa = RondasLector.CurrentWork.Causa;
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
            bool flag = true;
            int num = 0;
            if (!INIT_STATE)
            {
                int[] numArray = await this.ValidarEntrada();
                num = numArray[0];
                flag = numArray[1] == 1;
            }
            INIT_STATE = false;
            if (!flag)
                return;
            if (RondasLector.CurrentWork != null)
            {
                RondasLector.CurrentWork.Valor = this.IsEnabledValorText ? this.ValorText : this.SelectedValueValorCombo;
                RondasLector.CurrentWork.Descripcion = this.Comentario.Trim();
                //RondasLector.CurrentWork.NoComment = this.noComment.Selected;
                RondasLector.CurrentWork.Causa = this.SelectedValueCausa;
                RondasLector.CurrentWork.fechar();
            }
            object obj;
            do
            {
                obj = RondasLector.CurrentRonda.next();
                if (obj == null)
                {
                    //RondasAdvertenciaManager.sheet = true;
                    //this.Form.App.showCanvas(typeof(AdvertenciaPopUp));
                    return;
                }
                if (obj is Work && ((Work)obj).isValidForThisState())
                {
                    RondasLector.CurrentWork = (Work)obj;
                    this.showActual();
                    return;
                }
            }
            while (!(obj is Steps));
            RondasLector.Step = (Steps)obj;
            //this.Form.App.showCanvas(typeof(StateMachine));
            AppFrame.Navigate(typeof(CapturaDatos1));
        }
        private async Task<int[]> ValidarEntrada()
        {
            if (RondasLector.CurrentWork != null)
            {
                int num1 = 0;
                string valor = this.IsEnabledValorText ? this.ValorText : this.SelectedValueValorCombo;
                if (valor.Length == 0 && this.Comentario.Trim().Length == 0)
                {
                    //int num2 = (int)MessageBox.Show("Debe digitar valor o un comentario");
                    await MessageDialogError.ImprimirAsync("Debe digitar valor o un comentario");
                    //this.txtValor.Focus();
                    ((CapturaDatos2)this.Page).txtValorText.Focus(FocusState.Programmatic);
                    //this.currFocus = this.txtValor;
                    return new int[2];
                }
                if (RondasLector.CurrentWork.Obligatorio)
                {
                    if (valor.Length == 0 && this.Comentario.Trim().Length == 0)
                    {
                        //int num2 = (int)MessageBox.Show("El valor es obligatorio");
                        await MessageDialogError.ImprimirAsync("El valor es obligatorio");
                        //this.txtValor.Focus();
                        ((CapturaDatos2)this.Page).txtValorText.Focus(FocusState.Programmatic);
                        //this.currFocus = this.txtValor;
                        return new int[2];
                    }
                    if (valor.Length == 0)
                        return new int[2] { 0, 1 };
                }
                if (valor.Length != 0)
                    num1 = this.validValue(valor);
                switch (num1)
                {
                    case 0:
                        ((CapturaDatos2)this.Page).txtValorText.Focus(FocusState.Programmatic);
                        //this.txtValor.Focus();
                        return new int[2];
                    case 1:
                        //if (!this.noComment.Selected && (this.txtComentario.Text.Trim().Length == 0 || this.cmbCausas.Text.Length == 0))
                        if (!(this.Comentario.Trim().Length == 0 || this.SelectedValueCausa.Length == 0))
                        {
                            if (this.Comentario.Trim().Length == 0)
                                //this.Comentario.Focus();
                                ((CapturaDatos2)this.Page).txtComentario.Focus(FocusState.Programmatic);
                            else
                                //this.cmbCausas.Focus();
                                ((CapturaDatos2)this.Page).cmbCausas.Focus(FocusState.Programmatic);
                            return new int[2] { num1, 0 };
                        }
                        break;
                }
                return new int[2] { num1, 1 };
            }
            return new int[2] { 0, 1 };
        }
        private int validValue(string valor)
        {
            string resultMsgTitle = null, resultMsgDetail = null;
            //bool showMsg = !this.noComment.Selected && (this.txtComentario.Text.Length == 0 || this.cmbCausas.Text.Length == 0);
            bool showMsg = !(this.Comentario.Length == 0 || this.SelectedValueCausa.Length == 0);
            if (!RondasLector.CurrentWork.Tipo.Equals("VP CARACTER"))
                return RondasLector.CurrentWork.validEntry(valor, showMsg, out resultMsgTitle, out resultMsgDetail);
            if (this.IsEnabledComboValor)
            {
                int selectedIndex = this.SelectedIndexValorCombo;
                string str = "" + this.SelectedValueValorCombo;
                if (selectedIndex != -1)
                    return RondasLector.CurrentWork.validEntryText(selectedIndex, str, showMsg, out resultMsgTitle, out resultMsgDetail);
            }
            return 2;
        }
        public async void home()
        {
            RondasCancelarPopUp _popUp = new RondasCancelarPopUp(this.AppFrame, true);
            await _popUp.showAsync();
        }
    }
}