using System.Linq;
using System.Reflection;
using DepthViewer.X.Views;
using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.Presenter.Core;
using MvvmCross.Platform.IoC;

namespace DepthViewer.X
{
    public class DepthViewerViewLoader : MvxFormsPageLoader
    {

        protected override string GetPageName(MvxViewModelRequest request)
        {
            var name = base.GetPageName(request);

            return name;
        }

        protected override System.Type GetPageType(MvxViewModelRequest request)
        {
            var type = typeof(LocalMappingsPage).GetTypeInfo().Assembly.CreatableTypes().FirstOrDefault(t => t.Name.Equals(GetPageName(request)));

            return type;
        }

    }
}

