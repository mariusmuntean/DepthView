using Android.App;
using DepthViewer.Core.Contracts;

namespace DepthViewer.Android.Services
{
    public class PathProvider : IPathProvider
    {
        public string BaseDirPath => Application.Context.FilesDir.Path;
    }
}