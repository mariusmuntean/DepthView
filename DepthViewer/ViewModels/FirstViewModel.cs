using System.Collections.ObjectModel;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using DepthViewer.Contracts;
using DepthViewer.Models;

namespace DepthViewer.ViewModels
{
    public class FirstViewModel:MvxViewModel
    {
        private ObservableCollection<Mapping> _mappings = new ObservableCollection<Mapping>();
        private MvxCommand<Mapping> _mappingTappedCommand;

        public FirstViewModel()
        {
            InitMappings();
        }

        private async void InitMappings()
        {
            var allMappings = await Mvx.Resolve<IParseDataService>().GetAllMappings();
            foreach (var mapping in allMappings)
            {
                _mappings.Add(mapping);
            }
        }


        public ObservableCollection<Mapping> Mappings
        {
            get { return _mappings; }
            private set { _mappings = value; }
        }


        #region Commands

        public MvxCommand<Mapping> MappingTappedCommand
        {
            get
            {
                _mappingTappedCommand = _mappingTappedCommand ?? new MvxCommand<Mapping>(mapping =>
                {
                    ShowViewModel<MappingViewModel>();
                });
                return _mappingTappedCommand;
            }
        }

        #endregion
    }
}