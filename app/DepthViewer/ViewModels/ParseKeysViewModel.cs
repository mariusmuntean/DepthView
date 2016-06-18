using System;
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
        private IParseDataService _parseDataService;
        private MvxCommand _cancelCommand;

        public ParseKeysViewModel(IParseDataService parseDataService)
        {
            _parseDataService = parseDataService;
            var parseConfig = _parseDataService.GetCurrentParseConfig();
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

                UpdateParseKeysCommand.RaiseCanExecuteChanged();
            }
        }

        public string ParseNetKey
        {
            get { return _parseNetKey; }
            set
            {
                _parseNetKey = value;
                RaisePropertyChanged(() => ParseNetKey);

                UpdateParseKeysCommand.RaiseCanExecuteChanged();
            }
        }

        public MvxCommand UpdateParseKeysCommand
        {
            get
            {
                _updateParseKeysCommand = _updateParseKeysCommand ?? new MvxCommand(() =>
                {
                    Debug.WriteLine("Updating Parse keys");
                    _parseDataService.UpdateParseApiKeys(ParseAppId, ParseNetKey);

                }, () =>
                {
                    return !(string.IsNullOrWhiteSpace(_parseAppId) || string.IsNullOrWhiteSpace(_parseNetKey));
                });

                return _updateParseKeysCommand;
            }
        }

        public MvxCommand CancelCommand
        {
            get
            {
                _cancelCommand = _cancelCommand ?? new MvxCommand(() =>
                {
                    DismissAction?.Invoke();
                });
                return _cancelCommand;
            }
        }

        public Action DismissAction { get; set; }
    }
}