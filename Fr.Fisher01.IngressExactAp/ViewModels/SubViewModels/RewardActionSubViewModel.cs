namespace Fr.Fisher01.IngressExactAp.ViewModels.SubViewModels
{
    public class RewardActionSubViewModel : ViewModel
    {
        public ActionType Type { get; }
        public string Text { get; }

        public RewardActionSubViewModel(ActionType type, string text, int apGain)
        {
            Text = text;
            ApGain = apGain;
            Type = type;
        }

        #region Properties
        
        public int ApGain { get; set; }
        public int ApGainWithModifier => ApGain * Modifier;

        public int Modifier { get; set; } = 1;
        public int Count { get; set; }
        public int LockedValue { get; set; }
        public bool IsLocked { get; set; }
        public bool HasExtraTip => Type is ActionType.Complete8ThReso or ActionType.CreateField or ActionType.MultiField;

        #endregion

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
    }
}