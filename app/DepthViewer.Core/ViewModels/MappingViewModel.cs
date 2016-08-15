using System;
using System.Diagnostics;
using DepthViewer.Core.Contracts;
using DepthViewer.Shared.Models;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace DepthViewer.Core.ViewModels
{
    public class MappingViewModel : MvxViewModel
    {
        private Mapping _currentMapping;
        private readonly ILocalMappingServices _mappingServices;

        public MappingViewModel(ILocalMappingServices localMappingServices)
        {
            _mappingServices = localMappingServices;
        }


        protected override async void InitFromBundle(IMvxBundle parameters)
        {
            base.InitFromBundle(parameters);

            try
            {
                if (parameters.Data.ContainsKey("Id"))
                {
                    var id = "";
                    if (parameters.Data.TryGetValue("Id", out id))
                    {
                        _currentMapping = _mappingServices.GetMapping(id).Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in MappingViewModel.InitFromBundle(): "+ex);
                //Close(this);
            }
        }

        public override void Start()
        {
            base.Start();
            Mvx.Resolve<IDataExchangeService>().Payload.Remove("CurrentMapping");
            Mvx.Resolve<IDataExchangeService>().Payload.Add("CurrentMapping", CurrentMapping);
        }





        #region Properties

        public Mapping CurrentMapping
        {
            get
            {
                return _currentMapping;
            }
            set
            {
                _currentMapping = value;
                RaisePropertyChanged(() => CurrentMapping);
            }
        }

        #endregion Properties
    }
}