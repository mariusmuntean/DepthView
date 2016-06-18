using System.Diagnostics;
using DepthViewer.Contracts;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace DepthViewer.ViewModels
{
    public class ParseKeysViewModel : MvxViewModel
    {
        private MvxCommand _updateParseKeysCommand;
        private string _parseAppId;
        private string _parseNetKey;

        public ParseKeysViewModel()
        {
            // Init keys if possible
            var parseConfig = Mvx.Resolve<IParseConfig>();
            _parseNetKey = parseConfig.DotNetKey;
            _parseAppId = parseConfig.ApplicationId;
        }

        public string ParseAppId
        {
            get { return _parseAppId; }
            set
            {
                _parseAppId = value;
                RaisePropertyChanged(() => ParseAppId);
            }
        }

        public string ParseNetKey
        {
            get { return _parseNetKey; }
            set
            {
                _parseNetKey = value;
                RaisePropertyChanged(() => ParseNetKey);
            }
        }

        public MvxCommand UpdateParseKeysCommandCommand
        {
            get
            {
                _updateParseKeysCommand = _updateParseKeysCommand ?? new MvxCommand(() =>
                {
                    Debug.WriteLine("Updating Pare keys");
                });

                return _updateParseKeysCommand;
            }

        }
    }
}