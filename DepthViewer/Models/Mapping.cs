using System;
using System.Collections.Generic;
using System.Linq;

namespace DepthViewer.Models
{
    public class Mapping
    {
        private List<Measurement> _measurements;
        private DateTime _createdAt;
        private string _id;

        public Mapping(string id, List<Measurement> measurements, DateTime createdAt)
        {
            _measurements = measurements;
            _createdAt = createdAt;
            _id = id;
        }

        public string Id
        {
            get { return _id; }
            private set { _id = value; }
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

        public string TeaserPath
        {
            get
            {
                return Measurements.First().ImagePath;
            }
        }
    }
}