using System.Collections.Generic;
using System.Linq;
using DepthViewer.Contracts;
using MvvmCross.Platform.Droid.Platform;
using Xamarin.Auth;

namespace DepthViewer.Services
{
    public class SecureDataStore : ISecureDataStore
    {
        readonly string _serviceId = "depthViewerServiceId";
        private AccountStore _accountStore;

        public SecureDataStore(IMvxAndroidCurrentTopActivity currentTopActivity)
        {
            _accountStore = AccountStore.Create(currentTopActivity.Activity);
        }

        public string GetValue(string key)
        {
            var storedAccount = _accountStore.FindAccountsForService(_serviceId).First(account => account.Username.Equals(key));
            return storedAccount?.Properties[key];
        }

        public void SetValue(string key, string value)
        {
            _accountStore.Save(new Account(key, new Dictionary<string, string> { { key, value } }), _serviceId);
        }

        public Dictionary<string, string> GetProperties(string key)
        {
            var storedAccount = _accountStore.FindAccountsForService(_serviceId)?.FirstOrDefault(account => account.Username.Equals(key));
            return storedAccount?.Properties;
        }

        public void SetProperties(string key, Dictionary<string, string> properties)
        {
            _accountStore.Save(new Account(key, properties), _serviceId);
        }

        public void RemoveValue(string key)
        {
            var storedAccount =
                _accountStore.FindAccountsForService(_serviceId).FirstOrDefault(account => account.Username.Equals(key));
            if (storedAccount == null)
            {
                return;
            }

            if (storedAccount.Properties.ContainsKey(key))
            {
                storedAccount.Properties.Remove(key);
            }

            _accountStore.Save(new Account(key, storedAccount.Properties), _serviceId);
        }

        public void RemoveProperties(string key)
        {
            _accountStore.Delete(new Account(key), _serviceId);
        }
    }
}