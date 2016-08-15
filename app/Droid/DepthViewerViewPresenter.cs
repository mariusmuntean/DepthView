using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;

namespace DepthViewer.X.Droid
{
    public class DepthViewerViewPresenter : DepthViewerViewPresenterBase, IMvxAndroidViewPresenter
    {
        public override void Show(MvxViewModelRequest request)
        {
            base.Show(request);
        }

        public override void ChangePresentation(MvxPresentationHint hint)
        {
            base.ChangePresentation(hint);
        }
    }
}

