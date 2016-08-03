using MvvmCross.Core.ViewModels;

namespace DepthViewer.Core.ViewModels
{
    public class FirstViewModel : MvxViewModel
    {
        public void ShowMenu()
        {
            ShowViewModel<LocalMappingsViewModel>();
            ShowViewModel<NavigationMenuViewModel>();
        }
    }
}