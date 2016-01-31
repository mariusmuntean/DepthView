using Android.App;
using Android.OS;
using Cirrious.MvvmCross.Droid.Views;
using Org.Libsdl.App;

namespace DepthViewer.Views
{
    [Activity(Label = "View for FirstViewModel")]
    public class FirstView : MvxActivity
    {
        private SDLSurface urhoSurface;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FirstView);
        }

    } 
}