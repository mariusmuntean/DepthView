using System;

namespace DepthViewer.Views.Presenter
{
    public interface IFragmentTypeLookup
    {
        bool TryGetFragmentType(Type viewModelType, out Type fragmentType);
    }
}