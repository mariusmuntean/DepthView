using System;
using Android.App;
using Android.Content.PM;
using DepthViewer.X.Droid.Views;
using MvvmCross.Droid.Views;
using MvvmCross.Platform;

namespace DepthViewer.X.Droid
{
    [Activity(
        Label = "DepthViewer.X"
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

        private bool _isInitializationComplete = false;
        public override void InitializationComplete()
        {
            if (!_isInitializationComplete)
            {
                try
                {
                    _isInitializationComplete = true;
                    StartActivity(typeof(FirstView));
                }
                catch (Exception ex)
                {
                    Mvx.Error("InitializationComplete() " + ex.Message);
                }
            }
        }
    }
}
