using Acr.UserDialogs;
using Fr.Fisher01.IngressExactAp.ViewModels.SubViewModels;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using ActionType = Fr.Fisher01.IngressExactAp.ViewModels.SubViewModels.RewardActionSubViewModel.ActionType;

namespace Fr.Fisher01.IngressExactAp.ViewModels
{
    public class CalculatorViewModel : BaseViewModel
    {
        public CalculatorViewModel(IUserDialogs dialogs) : base(dialogs)
        {
            this.PropertyChanged += CalculatorViewModel_PropertyChanged;
        }

        public override void ViewDidAppear()
        {
            base.ViewDidAppear();

            this.ReloadData();
        }

        #region Properties

        public ObservableCollection<RewardActionSubViewModel> RewardActions { get; set; }
        public string CurrentApString { get; set; }
        public string TargetApString { get; set; }
        public bool IsDoubleApEnabled { get; set; }

        #endregion

        #region Commands

        public ICommand ShowTipCommand => new Command<RewardActionSubViewModel>(ShowTip);
        public ICommand PickNumberCommand => new Command<RewardActionSubViewModel>(PickNumber);
        public ICommand ActionDoneOnceCommand => new Command<RewardActionSubViewModel>(ActionDoneOnce);

        private void ShowTip(RewardActionSubViewModel action)
        {
            Dialogs.Alert(GetTipForActionType(action.Type));
        }

        private void PickNumber(RewardActionSubViewModel action)
        {

        }

        private void ActionDoneOnce(RewardActionSubViewModel action)
        {
            var parsed = int.TryParse(CurrentApString, NumberStyles.Integer, CultureInfo.InvariantCulture, out _currentAp);
            if (parsed)
            {
                if (action.Count == 0)
                    Dialogs.Toast("Warning : Your last action changed the rest of actions to do ! Read carefully what you need now !",
                        TimeSpan.FromSeconds(10));
                        
                _currentAp += action.ApGainWithModifier;
                CurrentApString = _currentAp.ToString();
            }
        }

        #endregion
        
        private void CalculatorViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsDoubleApEnabled))
            {
                foreach (var action in RewardActions) 
                    action.Modifier = IsDoubleApEnabled ? 2 : 1;
            }

            if (e.PropertyName is nameof(IsDoubleApEnabled) or nameof(CurrentApString) or nameof(TargetApString) or nameof(RewardActions))
                Refresh();
        }

        private void ReloadData()
        {
            RewardActions = new ObservableCollection<RewardActionSubViewModel>(App.RewardActions);
        }

        private bool _success = false;
        private int _goalAp = 0;
        private int _currentAp = 0;
        private int _targetAp = 0;
        private int _retries = 0;
        
        private void Refresh()
        {
            _success = false;

            var parsed = int.TryParse(CurrentApString, NumberStyles.Integer, CultureInfo.InvariantCulture, out _currentAp);
            parsed &= int.TryParse(TargetApString, NumberStyles.Integer, CultureInfo.InvariantCulture, out _targetAp);

            if (!parsed)
                return;

            if (_currentAp > _targetAp)
            {
                this.SetCounters(-1);
                return;
            }

            this.SetCounters(0);
            var goal = _goalAp = _targetAp - _currentAp;

            foreach (var action in RewardActions.Where(x => x.IsLocked))
                goal -= action.ApGainWithModifier * action.LockedValue;

            _retries = 0;
            if ((!IsDoubleApEnabled || _goalAp % 2 == 0) && goal >= 0 && RecurseCalculateAp(goal)) {
                _success = true;
            }
            else
            {
                this.SetCounters(-1);

                if (_retries > 10000)
                {
                    // Too many retry
                }
            }
        }

        private bool RecurseCalculateAp(int remainingAp, int actionIndex = 0)
        {
            _retries++;
            
            if (remainingAp == 0)
                return true;

            if (RewardActions[actionIndex].Type == ActionType.Recharge && 
                remainingAp % RewardActions[actionIndex].ApGainWithModifier != 0)
                return false;

            if (actionIndex < RewardActions.Count)
            {
                if (RewardActions[actionIndex].IsLocked) {
                    return RecurseCalculateAp(remainingAp, actionIndex + 1);
                }
                
                var remains = remainingAp % RewardActions[actionIndex].ApGainWithModifier;
                while (remains <= remainingAp && _retries < 100000)
                {
                    RewardActions[actionIndex].Count = (remainingAp - remains) / RewardActions[actionIndex].ApGainWithModifier;
                    
                    if (RecurseCalculateAp(remains, actionIndex + 1))
                        return true;
                    
                    remains += RewardActions[actionIndex].ApGainWithModifier;
                }
            }
            return false;

        }

        private void SetCounters(int count)
        {
            foreach (var action in RewardActions.Where(x => x.IsLocked is false)) 
                action.Count = count;
        }

        private string GetTipForActionType(ActionType actionType)
        {
            return actionType switch
            {
                ActionType.Capture => "Capture portal by deploying one and only one resonator",
                ActionType.Deploy => "Deploy a mod or a resonator",
                ActionType.Complete8ThReso => "Deploy the 8th resonator",
                ActionType.CreateLink => "Create a link without creating a control field",
                ActionType.CreateField => "Create one link that creates one control field",
                ActionType.MultiField => "Create one link that creates two control fields at the same time",
                ActionType.Hack => "Hack an enemy portal",
                ActionType.UpgradeReso => "Upgrade a resonator",
                ActionType.Recharge => "Recharge portal once",
                _ => $"Unknown action {actionType}"
            };
        }
    }
}