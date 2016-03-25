using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using DepthViewer.ViewModels;
using DepthViewer.Views.CustomControls;
using MvvmCross.Droid.FullFragging.Views;
using Org.Libsdl.App;
using Urho.Droid;

namespace DepthViewer.Views
{
    [Activity(Label = "View for MappingViewModel")]
    public class MappingView : MvxActivity<MappingViewModel>
    {
        private SDLSurface urhoSurface;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MappingView);

            var urhoLayout = FindViewById<AbsoluteLayout>(Resource.Id.urhoLayout);
            urhoSurface = UrhoSurface.CreateSurface<Map3D>(this);

            urhoLayout.AddView(urhoSurface);
        }

        protected override void OnResume()
        {
            UrhoSurface.OnResume();
            base.OnResume();
        }

        protected override void OnPause()
        {
            UrhoSurface.OnPause();
            base.OnPause();
        }

        public override void OnLowMemory()
        {
            UrhoSurface.OnLowMemory();
            base.OnLowMemory();
        }

        protected override void OnDestroy()
        {
            UrhoSurface.OnDestroy();
            base.OnDestroy();
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (!UrhoSurface.DispatchKeyEvent(e))
                return false;
            return base.DispatchKeyEvent(e);
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            UrhoSurface.OnWindowFocusChanged(hasFocus);
            base.OnWindowFocusChanged(hasFocus);
        }
    } 
}