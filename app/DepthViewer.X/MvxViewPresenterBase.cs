using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DepthViewer.X.Utils;
using DepthViewer.X.Views;
using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.Presenter.Core;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using Xamarin.Forms;

namespace DepthViewer.X
{
    public abstract class DepthViewerViewPresenterBase : MvxFormsPagePresenter
    {
        protected DepthViewerViewPresenterBase(Application mvxFormsApp)
        {
            MvxFormsApp = mvxFormsApp;
        }

        protected DepthViewerViewPresenterBase()
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
                    NavigationPage.SetHasNavigationBar(page, false);

                    if (IsModal(page))
                    {
                        await mainPage.Navigation.PushModalAsync(page, true);
                    }
                    else
                    {
                        await mainPage.PushAsync(page);
                    }
                    
                    _viewStack.Add(page);
                }
                catch (Exception e)
                {
                    Mvx.Error("Exception pushing {0}: {1}\n{2}", page.GetType(), e.Message, e.StackTrace);
                }
            }
        }

        private bool IsModal(Page page)
        {
            return IsModal(page.GetType());
        }

        private bool IsModal(MvxClosePresentationHint hint)
        {
            var pageName = hint.ViewModelToClose.GetType().Name.Replace("ViewModel", "Page");
            var pageType = typeof(LocalMappingsPage).GetTypeInfo().Assembly.CreatableTypes().FirstOrDefault(t => t.Name.Equals(pageName));
            return IsModal(pageType);
        }

        private bool IsModal(Type page)
        {
            var attr = page.GetCustomAttributes(typeof(ModalAttribute), false).FirstOrDefault() as ModalAttribute;
            return attr != null && attr.IsModal;
        }

        /*
 * Create the page for the modal ViewModel. 
 * Create class attribute(yay) for the Page class and evaluate it here, pushing the new page with PushModal
 * 
 * var detailPage = new DetailPage ();
...
await Navigation.PushModalAsync (detailPage);
 */

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

                        // Pop differently if modal
                        if (IsModal(hint as MvxClosePresentationHint))
                        {
                            await mainPage.Navigation.PopModalAsync(true);
                        }
                        else
                        {
                            await mainPage.PopAsync();
                        }
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