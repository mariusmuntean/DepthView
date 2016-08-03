using Android.App;
using Android.OS;
using Android.Runtime;
using DepthViewer.Core.ViewModels;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace DepthViewer.Android.Views.Fragments
{
    [Register("depthviewer.android.views.fragments.ParseKeysFragment")]
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
            dialog.SetIcon(Resource.Drawable.parse_logo);
            dialog.SetView(view);

            // Wire up vm to close the fragment
            ViewModel.DismissAction = Dismiss;

            return dialog.Create();

        }
    }
}