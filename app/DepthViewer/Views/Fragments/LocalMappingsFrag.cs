using System;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using DepthViewer.ViewModels;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Binding.Droid.Views;
using MvvmCross.Binding.ExtensionMethods;
using MvvmCross.Droid.Support.V4;
using MvxFragment = MvvmCross.Droid.FullFragging.Fragments.MvxFragment;

namespace DepthViewer.Views.Fragments
{
    public class LocalMappingsFrag : MvxFragment, AdapterView.IOnItemLongClickListener, ActionMode.ICallback
    {
        private MvxListView _lstViewLocalMappings = null;
        private View _rootView;
        private ActionMode _actionMode;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            _rootView = this.BindingInflate(Resource.Layout.LocalMappingsView, null);

            WireUpControls();

            return _rootView;
        }

        private void WireUpControls()
        {
           
            // Set an empty view and longpressed action for the list of mappings
            _lstViewLocalMappings = _rootView.FindViewById<MvxListView>(Resource.Id.lstViewLocalMappings);
            var txtViewNoMappings = _rootView.FindViewById<TextView>(Resource.Id.txtViewNoLocalMappings);

            _lstViewLocalMappings.EmptyView = txtViewNoMappings;
            _lstViewLocalMappings.OnItemLongClickListener = this;

            // Wire up fab's click event handler
            var fab = _rootView.FindViewById<FloatingActionButton>(Resource.Id.BtnDownloadMapping);
            fab.Click += DownloadMappingClicked;
        }


        private void DownloadMappingClicked(object sender, EventArgs eventArgs)
        {
            var remoteMappingsDialog = new MappingsOverviewFragment(this.Activity);
            remoteMappingsDialog.ViewModel = (ViewModel as FirstViewModel)?.Sub;

            // ToDo: make this nicer
            remoteMappingsDialog.Show((this.Activity as MvxFragmentActivity).SupportFragmentManager, "Remote mappings dialog");
        }

        #region IOnItemLongClickListener
        public bool OnItemLongClick(AdapterView parent, View view, int position, long id)
        {
            // Inform VM
            (ViewModel as FirstViewModel).MappingLongClickCommand.Execute(position);

            // Show CAB
            if (_actionMode == null)
            {
                _actionMode = this.Activity.StartActionMode(this);
            }

            // The selection gets lost so ..
            _lstViewLocalMappings.SetItemChecked(position, true);


            // Mark event as handled
            return true;
        }
        #endregion

        #region ActionMode.ICallback
        public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
        {
            if (item.ItemId != Resource.Id.Delete)
            {
                return false;
            }

            (ViewModel as FirstViewModel).DeleteCommand.Execute(null);
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
    }
}