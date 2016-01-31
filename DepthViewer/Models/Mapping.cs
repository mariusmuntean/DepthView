using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DepthViewer.Models
{
    public class Mapping
    {
        private List<Measurement> _measurements;
        private DateTime _createdAt;

        public Mapping(List<Measurement> measurements, DateTime createdAt)
        {
            _measurements = measurements;
            _createdAt = createdAt;
        }

        public DateTime CreatedAt
        {
            get { return _createdAt; }
            private set { _createdAt = value; }
        }

        public List<Measurement> Measurements
        {
            get { return _measurements; }
            private set { _measurements = value; }
        }
    }
}