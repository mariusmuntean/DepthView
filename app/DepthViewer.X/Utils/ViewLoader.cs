using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.Presenter.Core;

namespace DepthViewer.X
{
    public class ViewLoader : MvxFormsPageLoader
    {

        protected override string GetPageName(MvxViewModelRequest request)
        {
            var name = base.GetPageName(request);

            return name.Replace("Page", "View");
        }
    }
}

