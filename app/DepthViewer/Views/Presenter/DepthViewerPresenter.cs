using System;
using Android.App;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.FullFragging.Fragments;
using MvvmCross.Droid.Views;

namespace DepthViewer.Views.Presenter
{
    public class DepthViewerPresenter : MvxAndroidViewPresenter
    {

        IFragmentTypeLookup _fragmentTypeLookup;
        IMvxViewModelLoader _viewModelLoader;
        FragmentManager _fragmentManager;

        public DepthViewerPresenter(IMvxViewModelLoader viewModelLoader, IFragmentTypeLookup fragmentTypeLookup)
        {
            _fragmentTypeLookup = fragmentTypeLookup;
            _viewModelLoader = viewModelLoader;
        }

        public void RegisterFragmentManager(FragmentManager fragmentManager, MvxFragment initialFragment)
        {
            _fragmentManager = fragmentManager;
            showFragment(initialFragment, false);
        }

        public override void Show(MvxViewModelRequest request)
        {
            Type fragmentType;

            if (_fragmentManager == null ||
                !_fragmentTypeLookup.TryGetFragmentType(request.ViewModelType, out fragmentType))
            {
                base.Show(request);
                return;
            }

            var fragment = (MvxFragment)Activator.CreateInstance(fragmentType);
            fragment.ViewModel = _viewModelLoader.LoadViewModel(request, null);

            showFragment(fragment, true);
        }

        public override void Close(IMvxViewModel viewModel)
        {
            var currentFrag = _fragmentManager.FindFragmentById(Resource.Id.contentFrame) as MvxFragment;

            if (currentFrag != null && currentFrag.ViewModel == viewModel)
            {
                _fragmentManager.PopBackStackImmediate();
                return;
            }

            base.Close(viewModel);
        }

        private void showFragment(MvxFragment fragment, bool addToBackstack)
        {
            var transaction = _fragmentManager.BeginTransaction();

            if (addToBackstack)
            {
                transaction.AddToBackStack(fragment.GetType().Name);
            }

            transaction.Replace(Resource.Id.contentFrame, fragment).Commit();
        }
    }
}