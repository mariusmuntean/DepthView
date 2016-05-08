using System;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Droid.FullFragging.Fragments;
using MvvmCross.Platform.IoC;

namespace DepthViewer.Views.Presenter
{
    public class FragmentTypeLookup : IFragmentTypeLookup
    {
        private readonly Dictionary<string, Type> _vmToFragTypeMapping;

        public FragmentTypeLookup()
        {
            _vmToFragTypeMapping = GetType().Assembly.ExceptionSafeGetTypes()
                .Where(type =>
                        !type.IsAbstract && !type.IsInterface &&
                        typeof(MvxFragment).IsAssignableFrom(type) &&
                        type.Name.EndsWith("View"))
                .Select(type => type)
                .ToDictionary(GetStrippedName);
        }

        public bool TryGetFragmentType(Type viewModelType, out Type fragmentType)
        {
            var strippedName = GetStrippedName(viewModelType);

            if (!_vmToFragTypeMapping.ContainsKey(strippedName))
            {
                fragmentType = null;
                return false;
            }

            fragmentType = _vmToFragTypeMapping[strippedName];
            return true;
        }

        private static string GetStrippedName(Type type)
        {
            return type.Name.TrimEnd("View".ToCharArray()).TrimEnd("ViewModel".ToCharArray());
        }
    }
}