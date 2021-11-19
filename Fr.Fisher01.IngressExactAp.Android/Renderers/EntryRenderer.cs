using Android.Content;
using Fr.Fisher01.IngressExactAp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]
namespace Fr.Fisher01.IngressExactAp.Droid.Renderers
{
    public class CustomEntryRenderer: EntryRenderer  
    {
        public CustomEntryRenderer(Context context) : base(context)
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            
            if(Control != null) Control.Background = null;
        }
    }
}