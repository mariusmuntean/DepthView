using System;
using MvvmCross.Core.ViewModels;

namespace DepthViewer.ViewModels
{
    public class NavigationMenuViewModel:MvxViewModel
    {
        // Credit to Cheese Baron : https://stackoverflow.com/questions/37033871/how-to-show-mvxdialogfragment-using-showviewmodel
        public Action UpdateParseKeysAction { get; set; }  
        public void UpdateParseKeys()
        {
            //ShowViewModel<ParseKeysViewModel>();
            UpdateParseKeysAction?.Invoke();
        }

        public void UpdateAzureLogin()
        {
            throw new System.NotImplementedException();
        }
    }
}