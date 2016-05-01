using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using DepthViewer.Contracts;
using DepthViewer.Shared.Models;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Core;

namespace DepthViewer.ViewModels
{
    public class FirstViewModel : MvxViewModel
    {
        private ObservableCollection<Mapping> _mappings = new ObservableCollection<Mapping>();
        private MvxCommand<Mapping> _mappingTappedCommand;
        private bool _isRefreshing;
        private IMvxCommand _reloadLocalMappingsCommand;
        private MappingsOverviewViewModel _sub;
        private MvxCommand<int> _mappingLongClickCommand;

        private IMvxCommand _deleteCommand;
        private int _longPressedMappingIndex;

        public FirstViewModel()
        {
            IsRefreshing = true;
            ReadLocalMappings();
            IsRefreshing = false;

            _sub = new MappingsOverviewViewModel(new MvxCommand<List<Mapping>>(OkCommand));
        }

        private async void OkCommand(List<Mapping> mappings)
        {
            IsRefreshing = true;

            var localMappingService = Mvx.Resolve<ILocalMappingServices>();
            foreach (var mapping in mappings)
            {
                await localMappingService.PersistMapping(mapping);
                await ReadLocalMappings();
            }

            IsRefreshing = false;
        }

        private async Task ReadLocalMappings()
        {
            var localMappingService = Mvx.Resolve<ILocalMappingServices>();
            var localMappings = await localMappingService.GetAllLocalMappings();

            RepopulateMappings(localMappings);
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

                Mvx.Resolve<IMvxMainThreadDispatcher>().RequestMainThreadAction(() =>
                {
                    RaisePropertyChanged(() => IsRefreshing);
                    RaisePropertyChanged(() => CanAddMoreMappings);
                });
            }
        }

        public bool CanAddMoreMappings
        {
            get { return !IsRefreshing; }
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

        public IMvxCommand ReloadLocalMappingsCommand
        {
            get
            {
                _reloadLocalMappingsCommand = _reloadLocalMappingsCommand ?? new MvxCommand(async () =>
                {
                    IsRefreshing = true;

                    var localMappingService = Mvx.Resolve<ILocalMappingServices>();
                    var localMappings = await localMappingService.GetAllLocalMappings();
                    RepopulateMappings(localMappings);

                    IsRefreshing = false;
                });
                return _reloadLocalMappingsCommand;
            }

        }

        public IMvxCommand DeleteCommand    
        {
            get
            {
                _deleteCommand = _deleteCommand ?? new MvxCommand(async () =>
                {
                    if (LongPressedMapping != null)
                    {
                        var localMappingService = Mvx.Resolve<ILocalMappingServices>();
                        await localMappingService.DeleteLocalMapping(LongPressedMapping);

                        IsRefreshing = true;
                        await ReadLocalMappings();
                        IsRefreshing = false;

                        LongPressedMappingIndex = -1;
                    }
                });
                return _deleteCommand;
            }
        }

        public Mapping LongPressedMapping
        {
            get
            {
                return LongPressedMappingIndex == -1 ? null: Mappings?.ElementAt(LongPressedMappingIndex);
            }
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