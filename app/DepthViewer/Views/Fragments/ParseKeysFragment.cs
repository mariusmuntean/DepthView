using Android.App;
using Android.OS;
using Android.Runtime;
using DepthViewer.ViewModels;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace DepthViewer.Views.Fragments
{
    [Register("depthviewer.views.fragments.ParseKeysFragment")]
    public class ParseKeysFragment: MvxDialogFragment<ParseKeysViewModel>
    {
        public ParseKeysViewModel ParseKeysViewModel
        {
            get { return ViewModel; }
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.EnsureBindingContextSet(savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.parse_keys_layout, null);

            var dialog = new AlertDialog.Builder(Activity);
            dialog.SetTitle("Update your Parse keys");
            dialog.SetView(view);

            return dialog.Create();

        }
    }
}