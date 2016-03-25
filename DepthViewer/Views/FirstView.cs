using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Widget;
using DepthViewer.ViewModels;
using DepthViewer.Views.Fragments;
using MvvmCross.Binding.Droid.Views;
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

            // Set an empty view for the list of mappings
            var lstViewLocalMappings = FindViewById<MvxListView>(Resource.Id.lstViewLocalMappings);
            var txtViewNoMappings = FindViewById<TextView>(Resource.Id.txtViewNoLocalMappings);
            lstViewLocalMappings.EmptyView = txtViewNoMappings;

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