using Acr.UserDialogs;
using Fr.Fisher01.IngressExactAp.ViewModels.SubViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Fr.Fisher01.IngressExactAp.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel(IUserDialogs dialogs) : base(dialogs)
        {
        }

        public override void ViewDidAppear()
        {
            base.ViewDidAppear();
            this.ReloadData();
        }

        #region Properties

        public ObservableCollection<RewardActionSubViewModel> RewardActions { get; set; }

        #endregion

        #region Commands

        public ICommand ShowTipCommand => new Command<RewardActionSubViewModel>(ShowTip);
        public ICommand SaveCommand => new Command(Save);
        public ICommand ResetCommand => new Command(Reset);

        private void ShowTip(RewardActionSubViewModel action)
        {
            if (action.HasExtraTip)
                Dialogs.Alert(action.GetExtraTip(), "Default AP gain in game :");
        }

        private void Save()
        {
            App.RewardActions = new ObservableCollection<RewardActionSubViewModel>(this.RewardActions);
        }

        private void Reset()
        {
            foreach (var action in RewardActions)
            {
                action.Reset();
            }
        }

        #endregion

        private void ReloadData()
        {
            RewardActions = new ObservableCollection<RewardActionSubViewModel>(App.RewardActions);
        }
    }
}