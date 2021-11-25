using Acr.UserDialogs;
using System.ComponentModel;

namespace Fr.Fisher01.IngressExactAp.ViewModels
{
    public abstract class BaseViewModel : ViewModel
    {
        protected BaseViewModel(IUserDialogs dialogs)
        {
            this.Dialogs = dialogs;
        }

        protected IUserDialogs Dialogs { get; }
    }

    public abstract class ViewModel : INotifyPropertyChanged
    {
        public bool IsBusy { get; set; }

        public virtual void ViewDidAppear() { }
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
