using System.Collections.Generic;

namespace DepthViewer.Core.Contracts
{
    public interface ISecureDataStore
    {
        string GetValue(string key);
        void SetValue(string key, string value);

        Dictionary<string, string> GetProperties(string key);
        void SetProperties(string key, Dictionary<string, string> properties);

        void RemoveValue(string key);
        void RemoveProperties(string key);
    }
}