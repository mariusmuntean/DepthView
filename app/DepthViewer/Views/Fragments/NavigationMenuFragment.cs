using System;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using DepthViewer.Contracts;
using DepthViewer.ViewModels;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Shared.Attributes;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platform;
using Debug = System.Diagnostics.Debug;

namespace DepthViewer.Views.Fragments
{
    [MvxFragment(typeof(FirstViewModel), Resource.Id.drawer)]
    [Register("depthviewer.views.fragments.NavigationMenuFragment")]
    class NavigationMenuFragment:MvxFragment<NavigationMenuViewModel>
    {
        private NavigationView _navigationView;
        /// <summary>
        /// Empty constructor for Mvvmcross
        /// </summary>
        public NavigationMenuFragment()
        {
            
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.nav_menu_fragment, null);

            // Wire up selection handler/listener
            _navigationView = view.FindViewById<NavigationView>(Resource.Id.navigationLayout);
            _navigationView.NavigationItemSelected += NavigationViewOnNavigationItemSelected;

            // Wire up the displaying of dialog fragments
            ViewModel.UpdateParseKeysAction = () =>
            {
                var parseDataService = Mvx.Resolve<IParseDataService>();
                var parseKeysFrag = new ParseKeysFragment
                {
                    DataContext = new ParseKeysViewModel(parseDataService)
                };
                parseKeysFrag.Show(FragmentManager, "NavMenuFrag");
            };

            return view;

        }

        public override void OnDestroyView()
        {
            _navigationView.NavigationItemSelected -= NavigationViewOnNavigationItemSelected;
            base.OnDestroyView();
        }

        private void NavigationViewOnNavigationItemSelected(object sender,
            NavigationView.NavigationItemSelectedEventArgs navigationItemSelectedEventArgs)
        {
            if (navigationItemSelectedEventArgs.MenuItem.ItemId == Resource.Id.ParseKeys)
            {
                ViewModel.UpdateParseKeys();
            }

            if (navigationItemSelectedEventArgs.MenuItem.ItemId == Resource.Id.AzureLogin)
            {
                ViewModel.UpdateAzureLogin();
            }
        }
    }

}