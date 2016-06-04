using Android.OS;
using Android.Runtime;
using Android.Views;
using DepthViewer.ViewModels;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Shared.Attributes;
using MvvmCross.Droid.Support.V4;

namespace DepthViewer.Views.Fragments
{
    [MvxFragment(typeof(FirstViewModel), Resource.Id.drawer)]
    [Register("depthviewer.views.fragments.NavigationMenuFragment")]
    class NavigationMenuFragment:MvxFragment<NavigationMenuViewModel>
    {
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

            return view;

        }
    }

}