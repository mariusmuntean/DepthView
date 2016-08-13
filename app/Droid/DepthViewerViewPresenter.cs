using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;

namespace DepthViewer.X.Droid
{
    public class DepthViewerViewPresenter : SenovoMvxViewPresenterBase, IMvxAndroidViewPresenter
    {
        public override void Show(MvxViewModelRequest request)
        {
            /*
             * Create the page for the modal ViewModel. 
             * Create class attribute(yay) for the Page class and evaluate it here, pushing the new page with PushModal
             * 
             * var detailPage = new DetailPage ();
    ...
    await Navigation.PushModalAsync (detailPage);
             */
            base.Show(request);
        }

        public override void ChangePresentation(MvxPresentationHint hint)
        {
            base.ChangePresentation(hint);
        }
    }
}

