using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RondasEcopetrolWPF.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly Dictionary<string, object> _propertyBackingDictionary = new Dictionary<string, object>();

        protected T GetPropertyValue<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            object value;
            if (_propertyBackingDictionary.TryGetValue(propertyName, out value))
            {
                return (T)value;
            }

            return default(T);
        }

        protected bool SetPropertyValue<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            if (EqualityComparer<T>.Default.Equals(newValue, GetPropertyValue<T>(propertyName))) return false;

            _propertyBackingDictionary[propertyName] = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }
        public void Navigated(Type tipo)
        {
            Page.NavigationService.Navigate(new Uri("Views/" + tipo.Name + ".xaml", UriKind.Relative));
        }
        //private Frame _appFrame;
        //public Frame AppFrame => _appFrame;

        private PageBase _page;
        public Page Page => _page;

        public abstract Task OnNavigatedTo(EventArgs args);
        //public abstract Task OnNavigatedFrom(EventArgs args);

        //internal void SetAppFrame(Frame viewFrame)
        //{
        //    _appFrame = viewFrame;
        //}

        internal void SetPage(PageBase page)
        {
            _page = page;
        }
    }
}
