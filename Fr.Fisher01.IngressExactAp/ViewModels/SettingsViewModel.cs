using Acr.UserDialogs;
using Fr.Fisher01.IngressExactAp.ViewModels.SubViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using ActionType = Fr.Fisher01.IngressExactAp.ViewModels.SubViewModels.RewardActionSubViewModel.ActionType;

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

        private void ShowTip(RewardActionSubViewModel action)
        {
            if (action.HasExtraTip)
                Dialogs.Alert(GetTipForActionType(action.Type), "Default AP gain in game :");
        }

        private void Save()
        {
            App.RewardActions = new ObservableCollection<RewardActionSubViewModel>(this.RewardActions);
        }

        #endregion

        private void ReloadData()
        {
            RewardActions = new ObservableCollection<RewardActionSubViewModel>(App.RewardActions);
        }

        private string GetTipForActionType(ActionType actionType)
        {
            return actionType switch
            {
                ActionType.CreateField => "link + field = 313 + 1250",
                ActionType.MultiField => "(2 * field) + link = (2 * 1250) + 313",
                ActionType.Complete8ThReso => "deploy + bonus = 125 + 250",
                _ => null
            };
        }
    }
}