using Android.App;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using DepthViewer.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace DepthViewer.Views
{
    [Activity(Label = "DepthViewer")]
    public class FirstView : MvxCachingFragmentCompatActivity<FirstViewModel>
    {
        private MvxActionBarDrawerToggle _abdt;
        private DrawerLayout _drawerLayout;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FirstView);

            _drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);

            // Set the toolbar as the actionbar
            var toolbar = FindViewById<Toolbar>(Resource.Id.my_toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);


            _abdt = new MvxActionBarDrawerToggle(this,
                _drawerLayout,
                Resource.String.OpenDrawerText,
                Resource.String.CloseDrawerText);
            _abdt.DrawerOpened += (sender, args) =>
            {
                Toast.MakeText(this, "Opened", ToastLength.Short).Show();
            };
            _abdt.DrawerClosed += (sender, args) =>
            {
                Toast.MakeText(this, "Closed", ToastLength.Short).Show();
            };

            _drawerLayout.AddDrawerListener(_abdt);

            if (bundle == null)
            {
                ViewModel.ShowMenu();
            }

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Check if the drawer toggle can handle the touch event
            if (_abdt.OnOptionsItemSelected(item))
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            _abdt.SyncState();
        }
    }
}