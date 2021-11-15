using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace Fr.Fisher01.IngressExactAp.ViewModels
{
    public class CalculatorViewModel : BaseViewModel
    {

        public CalculatorViewModel()
        {
            RewardActions = new ObservableCollection<RewardAction>()
            {
                new(ActionType.MultiField, "Multi field", (1250 * 2) + 313),
                new(ActionType.CreateField, "Field", 1250 + 313),
                new(ActionType.Capture, "Capture", 500 + 125),
                new(ActionType.Complete8ThReso, "Complete", 250 + 125),
                new(ActionType.CreateLink, "Link", 313),
                new(ActionType.DeployReso, "Deploy", 125),
                new(ActionType.Hack, "Hack Enemy", 100),
                new(ActionType.UpgradeReso, "Upgrade", 65),
                new(ActionType.Recharge, "Recharge", 65), // because au SARS-CoV-2 pandemic, else it should be 10
            };
        }

        public ObservableCollection<RewardAction> RewardActions { get; set; }
        
        private string currentApString;
        public string CurrentApString
        {
            get => currentApString;
            set
            {
                if (SetProperty(ref currentApString, value))
                    Refresh();
            }
        }

        private string targetApString;
        public string TargetApString
        {
            get => targetApString;
            set
            {
                if (SetProperty(ref targetApString, value))
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

        private int _goalAp = 0;
        private bool _success = false;
        
        private int _index = 0;

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

            Debug.WriteLine($"_currentAp = {_currentAp}");
            Debug.WriteLine($"_targetAp = {_targetAp}");

            this.SetCounters(0);
            _goalAp = _targetAp - _currentAp;

            var goal = _goalAp;

            foreach (var action in RewardActions)
            {
                if (action.IsLocked)
                    goal -= action.ApGain * action.LockedValue;
            }

            _retries = 0;
            if ((!_isDoubleApEnabled || _goalAp % 2 == 0) && goal >= 0 && RecurseCalculateAp(goal, 0)) {
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

        private bool RecurseCalculateAp(int remainingAp, int actionIndex)
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
            foreach (var action in RewardActions)
            {
                if (action.IsLocked is false) 
                    action.Count = count;
            }
        }

        public class RewardAction : BaseViewModel
        {
            public ActionType Type { get; }
            public string Text { get; }

            private int _apGain;
            public int ApGain
            {
                get => _apGain * Modifier;
                private set => SetProperty(ref _apGain, value);
            }

            public RewardAction(ActionType type, string text, int apGain)
            {
                Text = text;
                ApGain = apGain;
                Type = type;
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
        }

        public enum ActionType
        {
            Capture,
            DeployReso,
            Complete8ThReso,
            CreateLink,
            CreateField,
            MultiField,
            Hack,
            UpgradeReso,
            Recharge
        }
    }
}