using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DepthViewer.Core.Contracts;
using DepthViewer.Core.Utils;
using DepthViewer.Shared.Models;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Core;

namespace DepthViewer.Core.ViewModels
{
    public class MappingsOverviewViewModel:MvxViewModel
    {
        private ObservableCollection<Mapping> _mappings;
        private MvxCommand<List<Mapping>> _okCommand;
        private IParseDataService _remoteMappingService;
        private bool _isRefreshing;
        private MvxCommand _refreshMappingsCommand;

        public MappingsOverviewViewModel()
        {

            RefreshRemoteMappings();
        }
        public MappingsOverviewViewModel(MvxCommand<List<Mapping>> okCommand)
        {
            _okCommand = okCommand;
            RefreshRemoteMappings();
        }

        private void RefreshRemoteMappings()
        {
            _remoteMappingService = Mvx.Resolve<IParseDataService>();
            _mappings = new ObservableCollection<Mapping>();

            // ToDo: remove me
            foreach (var dummyMapping in DummyDataGenerator.GetDummyMappings())
            {
                _mappings.Add(dummyMapping);
            }

            FetchRemoteMappings();
        }

        private async Task FetchRemoteMappings()
        {
            var remoteMappings = await _remoteMappingService.GetAllMappings();
            Mvx.Resolve<IMvxMainThreadDispatcher>().RequestMainThreadAction(() =>
            {
                Mappings.Clear();
                foreach (var remoteMapping in remoteMappings)
                {
                    Mappings.Add(remoteMapping);
                }
            });
        }
        public IMvxCommand RefreshMappingsCommand
        {
            get
            {
                _refreshMappingsCommand = _refreshMappingsCommand ?? new MvxCommand(async () =>
                {
                    IsRefreshing = true;
                    Mappings.Clear();

                    await FetchRemoteMappings();

                    IsRefreshing = false;
                });
                return _refreshMappingsCommand;
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
            get
            {
                return _mappings;
            }
            set
            {
                _mappings = value;
                RaisePropertyChanged(() => Mappings);
            }
        }

        public MvxCommand<List<Mapping>> OkCommand
        {
            get { return _okCommand; }
            set { _okCommand = value; }
        }

        public bool All { get; set; }
        public bool None { get; set; }
    }
}