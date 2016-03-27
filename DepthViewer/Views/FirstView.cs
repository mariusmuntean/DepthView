using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using DepthViewer.Models;
using DepthViewer.ViewModels;
using DepthViewer.Views.Fragments;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Droid.Views;
using MvvmCross.Binding.ExtensionMethods;
using MvvmCross.Droid.Support.V7.Fragging;
using Org.Libsdl.App;


namespace DepthViewer.Views
{
    [Activity(Label = "View for FirstViewModel")]
    public class FirstView : MvxFragmentActivity<FirstViewModel>, ActionMode.ICallback, AdapterView.IOnItemLongClickListener
    {
        private MvxListView _lstViewLocalMappings = null;
        private ActionMode _actionMode;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FirstView);

            // Set an empty view and longpressed action for the list of mappings
            _lstViewLocalMappings = FindViewById<MvxListView>(Resource.Id.lstViewLocalMappings);
            var txtViewNoMappings = FindViewById<TextView>(Resource.Id.txtViewNoLocalMappings);

            _lstViewLocalMappings.EmptyView = txtViewNoMappings;
            _lstViewLocalMappings.OnItemLongClickListener = this;

            // Wire up fab's click event handler
            var fab = FindViewById<FloatingActionButton>(Resource.Id.BtnDownloadMapping);
            fab.Click += DownloadMappingClicked;
        }

        private void DownloadMappingClicked(object sender, EventArgs eventArgs)
        {
            var remoteMappingsDialog = new MappingsOverviewFragment(this);
            remoteMappingsDialog.ViewModel = (ViewModel as FirstViewModel)?.Sub;
            remoteMappingsDialog.Show(SupportFragmentManager, "Remote mappings dialog");
        }

        #region ActionMode.ICallback
        public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
        {
            if (item.ItemId != Resource.Id.Delete)
            {
                return false;
            }

            ViewModel.DeleteCommand.Execute(null);
            _actionMode.Finish();
            return false;
        }

        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            mode.MenuInflater.Inflate(Resource.Menu.HomeContextActions, menu);
            return true;
        }

        public void OnDestroyActionMode(ActionMode mode)
        {
            // Uncheck all
            for (var i = 0; i < _lstViewLocalMappings.ItemsSource.Count(); i++)
            {
                _lstViewLocalMappings.SetItemChecked(i, false);
            }

            _actionMode = null;
        }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
        {
            return false;
        }

        #endregion ActionMode.ICallback

        #region IOnItemLongClickListener
        public bool OnItemLongClick(AdapterView parent, View view, int position, long id)
        {
            // Inform VM
            ViewModel.MappingLongClickCommand.Execute(position);

            // Show CAB
            if (_actionMode == null)
            {
                _actionMode = StartActionMode(this);
            }

            // The selection gets lost so ..
            _lstViewLocalMappings.SetItemChecked(position, true);


            // Mark event as handled
            return true;
        }
        #endregion
    }
}