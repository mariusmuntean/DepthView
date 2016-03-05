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
    public class Measurement
    {
        private double _panAngle;
        private double _tiltAngle;
        private double _distanceCm;
        private string _imageUrl;

        public Measurement(double panAngle, double tiltAngle, double distanceCm, string imageUrl)
        {
            _panAngle = panAngle;
            _tiltAngle = tiltAngle;
            _distanceCm = distanceCm;
            _imageUrl = imageUrl;
        }

        public double PanAngle
        {
            get { return _panAngle; }
            private set { _panAngle = value; }
        }

        public double TiltAngle
        {
            get { return _tiltAngle; }
            private set { _tiltAngle = value; }
        }

        public double DistanceCm
        {
            get { return _distanceCm; }
            private set { _distanceCm = value; }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            private set { _imageUrl = value; }
        }
    }
}