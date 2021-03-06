using Android.App;
using Android.Content.PM;
using MvvmCross.Droid.Views;

namespace DepthViewer.Android
{
    [Activity(
        Label = "DepthViewer"
        , MainLauncher = true
        , Icon = "@drawable/ic_launcher"
        , Theme = "@style/Theme.Splash"
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenActivity
    {
        public SplashScreen()
            : base(Resource.Layout.SplashScreen)
        {
        }
    }
}
