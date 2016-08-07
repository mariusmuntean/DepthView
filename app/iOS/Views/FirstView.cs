using DepthViewer.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;

namespace DepthViewer.X.iOS.Views
{
    public partial class FirstView : MvxViewController
    {
        public FirstView() : base("FirstView", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<FirstView, LocalMappingsViewModel>();
            set.Bind(Label).To(vm => vm.GetType().FullName);
            set.Bind(TextField).To(vm => vm.GetType().Assembly.FullName);
            set.Apply();
        }
    }
}
