namespace Fr.Fisher01.IngressExactAp.ViewModels.SubViewModels
{
    public class RewardActionSubViewModel : ViewModel
    {
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

        public ActionType Type { get; }
        public string Text { get; }

        public RewardActionSubViewModel(ActionType type, string text, int apGain)
        {
            Text = text;
            ApGain = apGain;
            DefaultApGain = apGain;
            Type = type;
        }

        #region Properties
        
        public int DefaultApGain { get; private set; }
        public int ApGain { get; set; }
        public int ApGainWithModifier => ApGain * Modifier;

        public int Modifier { get; set; } = 1;
        public int Count { get; set; }
        public int LockedValue { get; set; }
        public bool IsLocked { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool HasExtraTip => Type is ActionType.Complete8ThReso or ActionType.CreateField or ActionType.MultiField or ActionType.Recharge;

        #endregion

        public void Reset()
        {
            this.ApGain = this.DefaultApGain;
        }

        public string GetTip()
        {
            return Type switch
            {
                ActionType.Capture => "Capture portal by deploying one and only one resonator",
                ActionType.Deploy => "Deploy a mod or a resonator",
                ActionType.Complete8ThReso => "Deploy the 8th resonator",
                ActionType.CreateLink => "Create a link without creating a control field",
                ActionType.CreateField => "Create one link that creates one control field",
                ActionType.MultiField => "Create one link that creates two control fields at the same time",
                ActionType.Hack => "Hack an enemy portal (it must NOT be your first day hack !)",
                ActionType.UpgradeReso => "Upgrade a resonator",
                ActionType.Recharge => "Recharge portal once",
                _ => $"Unknown action {Type}"
            };
        }

        public string GetExtraTip()
        {
            return Type switch
            {
                ActionType.CreateField => "link + field = 313 + 1250",
                ActionType.MultiField => "(2 * field) + link = (2 * 1250) + 313",
                ActionType.Complete8ThReso => "deploy + bonus = 125 + 250",
                ActionType.Recharge => "Because of SARS-CoV-2 pandemic it's 65, else it should be 10 by default",
                _ => null
            };
        }
    }
}