using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DepthViewer.Contracts;
using DepthViewer.Models;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Core;

namespace DepthViewer.ViewModels
{
    public class MappingsOverviewViewModel:MvxViewModel
    {
        private ObservableCollection<Mapping> _mappings;
        private MvxCommand<List<Mapping>> _okCommand;
        private IParseDataService _remoteMappingService;
        private bool _isRefreshing;
        private MvxCommand _refreshMappingsCommand;

        public MappingsOverviewViewModel(MvxCommand<List<Mapping>> okCommand)
        {
            _okCommand = okCommand;
            _remoteMappingService = Mvx.Resolve<IParseDataService>();
            _mappings = new ObservableCollection<Mapping>();

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
            get { return _mappings; }
            set
            {
                _mappings = value;
                RaisePropertyChanged(() => Mappings);
            }
        }

        public MvxCommand<List<Mapping>> OkCommand
        {
            get { return _okCommand; }
        }

        public bool All { get; set; }
        public bool None { get; set; }
    }

    public class FirstViewModel : MvxViewModel
    {
        private ObservableCollection<Mapping> _mappings = new ObservableCollection<Mapping>();
        private MvxCommand<Mapping> _mappingTappedCommand;
        private bool _isRefreshing;
        private IMvxCommand _refreshMappingsCommand;
        private MappingsOverviewViewModel _sub;
        private MvxCommand<int> _mappingLongClickCommand;

        private IMvxCommand _deleteCommand;
        private int _longPressedMappingIndex;

        public FirstViewModel()
        {
            ReadLocalMappings();

            _sub = new MappingsOverviewViewModel(new MvxCommand<List<Mapping>>(mapping =>
            {
                
            }));
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

        public MappingsOverviewViewModel Sub
        {
            get { return _sub; }
            set
            {
                _sub = value;
                RaisePropertyChanged(() => Sub);
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

        public MvxCommand<int> MappingLongClickCommand
        {
            get
            {
                _mappingLongClickCommand = _mappingLongClickCommand ?? new MvxCommand<int>(idx =>
                {
                    LongPressedMappingIndex = idx;
                });
                return _mappingLongClickCommand;
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

        public IMvxCommand DeleteCommand    
        {
            get
            {
                _deleteCommand = _deleteCommand ?? new MvxCommand(() =>
                {
                    if (LongPressedMapping != null)
                    {
                        
                    }
                });
                return _deleteCommand;
            }
        }

        public Mapping LongPressedMapping
        {
            get { return Mappings?.ElementAt(LongPressedMappingIndex); }
        }

        public int LongPressedMappingIndex  
        {
            get { return _longPressedMappingIndex; }
            set
            {
                _longPressedMappingIndex = value;
                RaisePropertyChanged(() => LongPressedMappingIndex);
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