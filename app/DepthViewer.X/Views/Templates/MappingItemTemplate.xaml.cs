using System.Windows.Input;
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

        public static readonly BindableProperty MarkForDownloadCommandProperty = BindableProperty.Create(nameof(MarkForDownloadCommand),
                                                                                                    typeof(ICommand),
                                                                                                    typeof(MappingItemTemplate),
                                                                                                    null,
                                                                                                    BindingMode.TwoWay);

        public ICommand MarkForDownloadCommand
        {
            get { return (ICommand)GetValue(MarkForDownloadCommandProperty); }
            set { SetValue(MarkForDownloadCommandProperty, value); }
        }

        public static readonly BindableProperty UnMarkForDownloadCommandProperty = BindableProperty.Create(nameof(UnMarkForDownloadCommand),
                                                                                            typeof(ICommand),
                                                                                            typeof(MappingItemTemplate),
                                                                                            null,
                                                                                            BindingMode.TwoWay);

        public ICommand UnMarkForDownloadCommand
        {
            get { return (ICommand)GetValue(UnMarkForDownloadCommandProperty); }
            set { SetValue(UnMarkForDownloadCommandProperty, value); }
        }

        public MappingItemTemplate()
        {
            InitializeComponent();
        }

        private void Switch_OnToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                MarkForDownloadCommand?.Execute(this.BindingContext);
            }
            else
            {
                UnMarkForDownloadCommand?.Execute(sender);
            }
        }
    }
}

