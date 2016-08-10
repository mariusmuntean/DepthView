using System;
using DepthViewer.Core.ViewModels;
using MvvmCross.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DepthViewer.X.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocalMappingsPage
    {
        private LocalMappingsViewModel _viewModel;

        public LocalMappingsPage()
        {
            InitializeComponent();
            BindingContextChanged += OnBindingContextChanged;
        }

        private void OnBindingContextChanged(object sender, EventArgs eventArgs)
        {
            _viewModel = BindingContext as LocalMappingsViewModel;
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            LstViewMappings.SelectedItem = null;
        }

        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            _viewModel?.MappingTappedCommand.Execute(e.Item);
        }
    }
}

