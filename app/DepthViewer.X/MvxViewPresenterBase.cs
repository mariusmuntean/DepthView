using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.Presenter.Core;
using MvvmCross.Platform;
using Xamarin.Forms;

namespace DepthViewer.X
{
    public abstract class SenovoMvxViewPresenterBase : MvxFormsPagePresenter
    {
        protected SenovoMvxViewPresenterBase(Application mvxFormsApp)
        {
            MvxFormsApp = mvxFormsApp;
        }

        protected SenovoMvxViewPresenterBase()
        {
        }

        private List<Page> _viewStack = new List<Page>();

        public override async void Show(MvxViewModelRequest request)
        {
            var page = MvxPresenterHelpers.CreatePage(request);
            if (page == null)
                return;

            var viewModel = MvxPresenterHelpers.LoadViewModel(request);

            var mainPage = Application.Current.MainPage as NavigationPage;
            page.BindingContext = viewModel;

            if (mainPage == null)
            {
                Application.Current.MainPage = new NavigationPage(page);
                mainPage = Application.Current.MainPage as NavigationPage;
                _viewStack.Add(page);

                // disable navigation bar
                NavigationPage.SetHasNavigationBar(mainPage, false);
                NavigationPage.SetHasNavigationBar(page, false);

                CustomPlatformInitialization(mainPage);
            }
            else
            {
                try
                {
#if DEBUG
                    //if (page.GetType() != typeof (HomeView))
                    NavigationPage.SetHasNavigationBar(page, false);
#else

                    NavigationPage.SetHasNavigationBar(page, false);
#endif
                    await mainPage.PushAsync(page);
                    _viewStack.Add(page);
                }
                catch (Exception e)
                {
                    Mvx.Error("Exception pushing {0}: {1}\n{2}", page.GetType(), e.Message, e.StackTrace);
                }
            }
        }

        public override async void ChangePresentation(MvxPresentationHint hint)
        {
            if (HandlePresentationChange(hint)) return;

            if (hint is MvxClosePresentationHint)
            {
                var mainPage = Application.Current.MainPage as NavigationPage;

                if (mainPage == null)
                {
                    Mvx.TaggedTrace("MvxFormsPresenter:ChangePresentation()", "Shit, son! Don't know what to do");
                }
                else
                {

                    try
                    {
                        if (mainPage.CurrentPage.BindingContext is IKillable)
                        {
                            ((IKillable)mainPage.CurrentPage.BindingContext).Kill();
                        }

                        if (mainPage.CurrentPage is IKillable)
                        {
                            ((IKillable)mainPage.CurrentPage).Kill();
                        }

                        //do not switch statements.Will break lifecycle
                        if (_viewStack.Any())
                        {
                            _viewStack.Remove(_viewStack.Last());
                        }
                        await mainPage.PopAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }

                }
            }
        }

        protected virtual void CustomPlatformInitialization(NavigationPage mainPage)
        { }

    }
}