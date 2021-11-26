using Fr.Fisher01.IngressExactAp.ViewModels.SubViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using ActionType = Fr.Fisher01.IngressExactAp.ViewModels.SubViewModels.RewardActionSubViewModel.ActionType;

namespace Fr.Fisher01.IngressExactAp
{
    public partial class App : Application
    {
        private static ObservableCollection<RewardActionSubViewModel> _rewardActions;
        public static ObservableCollection<RewardActionSubViewModel> RewardActions
        {
            get => _rewardActions;
            set => _rewardActions = new ObservableCollection<RewardActionSubViewModel>(
                value.OrderByDescending(x => x.ApGain)); // This ensure list is ordered correctly;
        }

        public App()
        {
            InitializeComponent();

            RewardActions = new ObservableCollection<RewardActionSubViewModel>(new RewardActionSubViewModel[]
            { 
                new(ActionType.MultiField, "Multi field", (1250 * 2) + 313),
                new(ActionType.CreateField, "Field", 1250 + 313),
                new(ActionType.Capture, "Capture", 500 + 125),
                new(ActionType.Complete8ThReso, "Complete", 250 + 125),
                new(ActionType.CreateLink, "Link", 313),
                new(ActionType.Deploy, "Deploy", 125),
                new(ActionType.Hack, "Hack Enemy (!)", 100),
                new(ActionType.UpgradeReso, "Upgrade", 65),
                new(ActionType.Recharge, "Recharge", 65) // because of SARS-CoV-2 pandemic, else it should be 10 by default
            });

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
