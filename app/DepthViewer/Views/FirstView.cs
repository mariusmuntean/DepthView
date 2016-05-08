using Android.App;
using Android.OS;
using DepthViewer.ViewModels;
using DepthViewer.Views.Fragments;
using DepthViewer.Views.Presenter;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views;
using MvvmCross.Platform;

namespace DepthViewer.Views
{
    [Activity(Label = "View for FirstViewModel")]
    public class FirstView : MvxFragmentActivity<FirstViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FirstView);

            var depthViewerPresenter = Mvx.Resolve<IMvxAndroidViewPresenter>() as DepthViewerPresenter;
            var initialFrag = new LocalMappingsFrag() {ViewModel = ViewModel};

            depthViewerPresenter.RegisterFragmentManager(FragmentManager, initialFrag);

        }
    }
}