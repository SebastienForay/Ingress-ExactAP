using Acr.UserDialogs;
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
        public CalculatorViewModel(IUserDialogs dialogs) : base(dialogs)
        {
            RewardActions = new ObservableCollection<RewardAction>(new []
                { 
                    new RewardAction(ActionType.MultiField, "Multi field", (1250 * 2) + 313),
                    new (ActionType.CreateField, "Field", 1250 + 313),
                    new(ActionType.Capture, "Capture", 500 + 125),
                    new(ActionType.Complete8ThReso, "Complete", 250 + 125),
                    new(ActionType.CreateLink, "Link", 313),
                    new(ActionType.Deploy, "Deploy", 125),
                    new(ActionType.Hack, "Hack Enemy", 100),
                    new(ActionType.UpgradeReso, "Upgrade", 65),
                    new(ActionType.Recharge, "Recharge", 65) // because au SARS-CoV-2 pandemic, else it should be 10
                }.OrderByDescending(x => x.ApGain).ToList()); // This ensure list is ordered correctly
        }

        #region Properties

        public ObservableCollection<RewardAction> RewardActions { get; set; }
        
        private string _currentApString;
        public string CurrentApString
        {
            get => _currentApString;
            set
            {
                if (SetProperty(ref _currentApString, value))
                    Refresh();
            }
        }

        private string _targetApString;
        public string TargetApString
        {
            get => _targetApString;
            set
            {
                if (SetProperty(ref _targetApString, value))
                    Refresh();
            }
        }

        private bool _isDoubleApEnabled;
        public bool IsDoubleApEnabled
        {
            get => _isDoubleApEnabled;
            set
            {
                if (SetProperty(ref _isDoubleApEnabled, value))
                {
                    foreach (var action in RewardActions) 
                        action.Modifier = _isDoubleApEnabled ? 2 : 1;

                    Refresh();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand ShowTipCommand => new Command<RewardAction>(ShowTip);
        public ICommand PickNumberCommand => new Command<RewardAction>(PickNumber);
        public ICommand ActionDoneOnceCommand => new Command<RewardAction>(ActionDoneOnce);

        private void ShowTip(RewardAction action)
        {
            Dialogs.Alert(GetTipForActionType(action.Type));
        }

        private void PickNumber(RewardAction action)
        {

        }

        private void ActionDoneOnce(RewardAction action)
        {
            var parsed = int.TryParse(CurrentApString, NumberStyles.Integer, CultureInfo.InvariantCulture, out _currentAp);
            if (parsed)
            {
                if (action.Count == 0)
                    Dialogs.Toast("Warning : Your last action changed the rest of actions to do ! Read carefully what you need now !",
                        TimeSpan.FromSeconds(10));
                        
                _currentAp += action.ApGain;
                CurrentApString = _currentAp.ToString();
            }
        }

        #endregion

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
                goal -= action.ApGain * action.LockedValue;

            _retries = 0;
            if ((!_isDoubleApEnabled || _goalAp % 2 == 0) && goal >= 0 && RecurseCalculateAp(goal)) {
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
                remainingAp % RewardActions[actionIndex].ApGain != 0)
                return false;

            if (actionIndex < RewardActions.Count)
            {
                if (RewardActions[actionIndex].IsLocked) {
                    return RecurseCalculateAp(remainingAp, actionIndex + 1);
                }
                
                var remains = remainingAp % RewardActions[actionIndex].ApGain;
                while (remains <= remainingAp && _retries < 100000)
                {
                    RewardActions[actionIndex].Count = (remainingAp - remains) / RewardActions[actionIndex].ApGain;
                    
                    if (RecurseCalculateAp(remains, actionIndex + 1))
                        return true;
                    
                    remains += RewardActions[actionIndex].ApGain;
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

    public enum ActionType
    {
        Capture,
        Deploy,
        Complete8ThReso,
        CreateLink,
        CreateField,
        MultiField,
        Hack,
        UpgradeReso,
        Recharge
    }

    public class RewardAction : ViewModel
    {
        public ActionType Type { get; }
        public string Text { get; }

        public RewardAction(ActionType type, string text, int apGain)
        {
            Text = text;
            ApGain = apGain;
            Type = type;
        }

        #region Properties

        private int _apGain;
        public int ApGain
        {
            get => _apGain * Modifier;
            private set => SetProperty(ref _apGain, value);
        }

        private int _modifier = 1;
        public int Modifier
        {
            get => _modifier;
            set => SetProperty(ref _modifier, value);
        }

        private int _count = 0;
        public int Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }

        private int _lockedValue = 0;
        public int LockedValue
        {
            get => _lockedValue;
            set => SetProperty(ref _lockedValue, value);
        }

        private bool _isLocked = false;
        public bool IsLocked
        {
            get => _isLocked;
            set => SetProperty(ref _isLocked, value);
        }

        #endregion
    }
}