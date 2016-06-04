using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using DepthViewer.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;

namespace DepthViewer.Views
{
    [Activity(Label = "DepthViewer")]
    public class FirstView : MvxCachingFragmentCompatActivity<FirstViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FirstView);
            
            // Set the toolbar as the actionbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.my_toolbar);
            SetSupportActionBar(toolbar);

            if (bundle == null)
            {
                ViewModel.ShowMenu();
            }

        }
    }
}