using Xamarin.Forms;

namespace DepthViewer.X.Views.Templates
{
    public partial class MappingItemTemplate : ContentView
    {
        public static readonly BindableProperty IsDownloadedProperty = BindableProperty.Create(nameof(IsDownloaded),
                                                                                                typeof(bool),
                                                                                                typeof(MappingItemTemplate),
                                                                                                false,
                                                                                                BindingMode.TwoWay);

        public bool IsDownloaded
        {
            get { return (bool)GetValue(IsDownloadedProperty); }
            set { SetValue(IsDownloadedProperty, value); }
        }

        public MappingItemTemplate()
        {
            InitializeComponent();
        }
    }
}

