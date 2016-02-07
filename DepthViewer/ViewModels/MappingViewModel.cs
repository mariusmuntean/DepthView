using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using DepthViewer.Contracts;
using DepthViewer.Models;

namespace DepthViewer.ViewModels
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

            if (parameters.Data.ContainsKey("Id"))
            {
                var id = "";
                if (parameters.Data.TryGetValue("Id", out id))
                {
                    _currentMapping = await _mappingServices.GetMapping(id);
                }
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