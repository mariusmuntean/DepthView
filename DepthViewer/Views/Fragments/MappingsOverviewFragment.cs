using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using DepthViewer.Models;
using DepthViewer.ViewModels;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Binding.Droid.Views;
using MvvmCross.Droid.Support.V7.Fragging.Fragments;

namespace DepthViewer.Views.Fragments
{
    public class MappingsOverviewFragment : MvxDialogFragment<MappingsOverviewViewModel>
    {
        Context _context;

        public MappingsOverviewFragment(Context context)
        {
            _context = context;
        }

        public MappingsOverviewViewModel MappingsOverviewViewModel
        {
            get { return ViewModel; }
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            //return base.OnCreateDialog(savedInstanceState); 
            base.EnsureBindingContextSet(savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.MappingsOverviewView, null);
            var lstRemoteMappings = view.FindViewById<MvxListView>(Resource.Id.lstViewRemoteMappings);
            lstRemoteMappings.ChoiceMode = ChoiceMode.Multiple;
            lstRemoteMappings.ItemsCanFocus = true;

            var dialog = new AlertDialog.Builder(_context);
            dialog.SetTitle("Remote Mappings");
            dialog.SetView(view);
            dialog.SetPositiveButton("OK", (sender, args) =>
            {
                var selectedPositions = lstRemoteMappings.CheckedItemPositions;
                if (selectedPositions == null || selectedPositions.Size() == 0)
                {
                    return;
                }

                var listSelectedMappings = new List<Mapping>();
                for(int i = 0;i<selectedPositions.Size();i++)
                {
                    var currentMappingPos = selectedPositions.KeyAt(i);
                    var currentMapping = ViewModel.Mappings.ElementAt(currentMappingPos);
                    listSelectedMappings.Add(currentMapping);
                }

                ViewModel.OkCommand.Execute(listSelectedMappings);
            });

            return dialog.Create();
        }
    }
}