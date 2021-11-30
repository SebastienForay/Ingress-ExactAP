using Acr.UserDialogs;
using Fr.Fisher01.IngressExactAp.ViewModels.SubViewModels;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Fr.Fisher01.IngressExactAp.ViewModels
{
    public class CalculatorViewModel : BaseViewModel
    {
        private const int MAX_RETRIES = 100000;

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
        public int ApexCount { get; set; }

        #endregion

        #region Commands

        public ICommand ShowTipCommand => new Command<RewardActionSubViewModel>(ShowTip);
        public ICommand PickNumberCommand => new Command<RewardActionSubViewModel>(PickNumber);
        public ICommand ActionDoneOnceCommand => new Command<RewardActionSubViewModel>(ActionDoneOnce);

        private void ShowTip(RewardActionSubViewModel action)
        {
            Dialogs.Alert(action.GetTip());
        }

        private void PickNumber(RewardActionSubViewModel action)
        {
            // TODO
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
            _isApexEnabled = ApexCount > 0;

            if (e.PropertyName == nameof(IsDoubleApEnabled))
            {
                foreach (var action in RewardActions)
                    action.Modifier = IsDoubleApEnabled ? _isApexEnabled ? 4 : 2 : 1;
            }

            if (e.PropertyName == nameof(ApexCount))
            {
                foreach (var action in RewardActions)
                    action.Modifier = _isApexEnabled ? IsDoubleApEnabled ? 4 : 2 : 1;
            }

            Refresh();
        }

        private void ReloadData()
        {
            RewardActions = new ObservableCollection<RewardActionSubViewModel>(App.RewardActions);
        }

        private bool _isApexEnabled = false;
        private bool _success = false;
        private int _goalAp = 0;
        private int _currentAp = 0;
        private int _targetAp = 0;
        private int _retries = 0;

        public void Refresh()
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

            foreach (var action in RewardActions.Where(x => x.IsLocked && x.IsEnabled))
                goal -= action.ApGainWithModifier * action.LockedValue;

            if (_isApexEnabled)
                goal -= ApexCount * 1000;

            _retries = 0;
            if (((!IsDoubleApEnabled && ApexCount == 0) || _goalAp % 2 == 0) && goal >= 0 && RecurseCalculateAp(goal)) {
                _success = true;
            }
            else
            {
                this.SetCounters(-1);

                if (_retries >= MAX_RETRIES)
                {
                    Dialogs.Alert("Your current values can't be used to find a solution");
                }
            }
        }

        private bool RecurseCalculateAp(int remainingAp, int actionIndex = 0)
        {
            _retries++;
            
            if (remainingAp == 0)
                return true;

            if (actionIndex >= RewardActions.Count)
                return false;

            if (actionIndex == (RewardActions.Count - 1) && 
                remainingAp % RewardActions[actionIndex].ApGainWithModifier != 0)
                return false;

            if (actionIndex < RewardActions.Count)
            {
                if (RewardActions[actionIndex].IsLocked || !RewardActions[actionIndex].IsEnabled) {
                    return RecurseCalculateAp(remainingAp, actionIndex + 1);
                }
                
                var remains = remainingAp % RewardActions[actionIndex].ApGainWithModifier;
                while (remains <= remainingAp && _retries < MAX_RETRIES)
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
    }
}