using System;
using System.Windows.Input;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Util;

namespace DepthViewer.Views.CustomControls
{
    /**
    ** LLAP to Cheesebaron
    ** Class source: https://gist.github.com/Cheesebaron/8130a835a0f0d0747c10
    */

    public class BindableSwipeRefreshLayout : SwipeRefreshLayout
    {
        private ICommand _refresh;

        protected BindableSwipeRefreshLayout(IntPtr javaReference, JniHandleOwnership transfer): base(javaReference, transfer)
        { }
        public BindableSwipeRefreshLayout(Context p0): this(p0, null)
        { }

        public BindableSwipeRefreshLayout(Context p0, IAttributeSet p1): base(p0, p1)
        { }

        public new bool Refreshing
        {
            get { return base.Refreshing; }
            set { base.Refreshing = value; }
        }

        public new ICommand Refresh
        {
            get { return _refresh; }
            set
            {
                _refresh = value;
                if (_refresh != null)
                    EnsureOnRefreshOverloaded();
            }
        }

        private bool _refreshOverloaded;
        private void EnsureOnRefreshOverloaded()
        {
            if (_refreshOverloaded)
                return;

            _refreshOverloaded = true;
            base.Refresh += (sender, args) => ExecuteCommandOnRefresh(Refresh);
        }

        protected virtual void ExecuteCommandOnRefresh(ICommand command)
        {
            if (command == null)
                return;

            if (!command.CanExecute(null))
                return;

            command.Execute(null);
        }
    }
}