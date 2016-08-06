using System.Diagnostics;
using MvvmCross.Core.ViewModels;

namespace DepthViewer.X
{
    public class BaseViewModel : MvxViewModel, IKillable
    {
        public BaseViewModel()
        {
        }


        public void Kill()
        {
            Debug.WriteLine("Killed VM: {0}", GetType().Name);
        }
    }
}

