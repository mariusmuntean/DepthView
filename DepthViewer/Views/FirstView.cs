using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using DepthViewer.ViewModels;
using DepthViewer.Views.Fragments;
using MvvmCross.Droid.Support.V7.Fragging;
using Org.Libsdl.App;

namespace DepthViewer.Views
{
    [Activity(Label = "View for FirstViewModel")]
    public class FirstView : MvxFragmentActivity<FirstViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FirstView);

            var fab = FindViewById<FloatingActionButton>(Resource.Id.BtnDownloadMapping);
            fab.Click +=  DownloadMappingClicked;
        }

        private void DownloadMappingClicked(object sender, EventArgs eventArgs)
        {
            var remoteMappingsDialog = new MappingsOverviewFragment(this);
            remoteMappingsDialog.ViewModel = (ViewModel as FirstViewModel)?.Sub;
            remoteMappingsDialog.Show(SupportFragmentManager, "Remote mappings dialog");
        }
    } 
}