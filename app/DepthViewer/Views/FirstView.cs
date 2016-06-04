using Android.App;
using Android.OS;
using DepthViewer.ViewModels;
using MvvmCross.Droid.Support.V4;

namespace DepthViewer.Views
{
    [Activity(Label = "View for FirstViewModel")]
    public class FirstView : MvxFragmentActivity<FirstViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FirstView);

        }
    }
}