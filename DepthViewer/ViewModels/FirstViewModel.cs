using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.ViewModels;
using DepthViewer.Contracts;
using DepthViewer.Models;

namespace DepthViewer.ViewModels
{
    public class FirstViewModel : MvxViewModel
    {
        private ObservableCollection<Mapping> _mappings = new ObservableCollection<Mapping>();
        private MvxCommand<Mapping> _mappingTappedCommand;
        private bool _isRefreshing;
        private IMvxCommand _refreshMappingsCommand;

        public FirstViewModel()
        {
            ReadLocalMappings();
        }

        private async Task ReadLocalMappings()
        {
            IsRefreshing = true;

            var localMappingService = Mvx.Resolve<ILocalMappingServices>();
            var localMappings = await localMappingService.GetAllLocalMappings();

            RepopulateMappings(localMappings);

            if (IsRefreshing)
            {
                IsRefreshing = false;
            }
        }

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                RaisePropertyChanged(() => IsRefreshing);
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
                    ShowViewModel<MappingViewModel>(mapping);
                });
                return _mappingTappedCommand;
            }
        }

        public IMvxCommand RefreshMappingsCommand
        {
            get
            {
                _refreshMappingsCommand = _refreshMappingsCommand ?? new MvxCommand(async () =>
                {
                    IsRefreshing = true;

                    var localMappingService = Mvx.Resolve<ILocalMappingServices>();
                    var newerMappings = await localMappingService.RefreshAllLocalMappings();
                    RepopulateMappings(newerMappings);

                    IsRefreshing = false;
                });
                return _refreshMappingsCommand;
            }

        }

        #endregion

        #region Helpers

        private void RepopulateMappings(List<Mapping> localMappings)
        {
            Mvx.Resolve<IMvxMainThreadDispatcher>().RequestMainThreadAction(() =>
            {
                Mappings.Clear();
                foreach (var localMapping in localMappings)
                {
                    Mappings.Add(localMapping);
                }
            });
        }

        #endregion helpers
    }
}