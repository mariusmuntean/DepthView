namespace DepthViewer.X.ViewModels
{
    public class FirstViewModel : BaseViewModel
    {
        private string _hello = "Hello Marius";
        public string Hello
        { 
            get { return _hello; }
            set { SetProperty (ref _hello, value); }
        }
    }
}
