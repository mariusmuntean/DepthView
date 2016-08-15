using System;

namespace DepthViewer.X.Utils
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModalAttribute : Attribute
    {
        public bool IsModal { get; set; }
    }
}
