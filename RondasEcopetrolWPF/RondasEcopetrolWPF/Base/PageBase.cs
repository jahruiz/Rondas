
using System;
using System.Windows.Controls;

namespace RondasEcopetrolWPF.Base
{
    public class PageBase : Page
    {
        private ViewModelBase _viewModel;

        public PageBase()
        {
			ShowsNavigationUI = false;
            this.Loaded += Inicialezar;
        }

        protected void Inicialezar(Object sender, EventArgs e)
        {
            _viewModel = (ViewModelBase)this.DataContext;
            //_viewModel.SetAppFrame(this.Frame);
            _viewModel.SetPage(this);
        }
            /*
            protected override void OnNavigatedTo(NavigationEventArgs e)
            {
                _viewModel = (ViewModelBase)this.DataContext;
                _viewModel.SetAppFrame(this.Frame);
                _viewModel.SetPage(this);
                base.OnNavigatedTo(e);
                _viewModel.OnNavigatedTo(e);

                //Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

                //Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += PageBase_BackRequested;
            }

            private void PageBase_BackRequested(object sender, BackRequestedEventArgs e)
            {
                if (Frame != null)
                {
                    if (Frame.CanGoBack)
                    {
                        e.Handled = true;
                        Frame.GoBack();
                    }
                }
            }

            protected override void OnNavigatedFrom(NavigationEventArgs e)
            {
                base.OnNavigatedFrom(e);
                _viewModel.OnNavigatedFrom(e);
            }
            */
        }
}
