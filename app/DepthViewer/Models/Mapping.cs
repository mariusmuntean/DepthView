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
        private bool _isSavedLocally;

        public Mapping(string id, List<Measurement> measurements, DateTime createdAt, DateTime? downloadedAt = null)
        {
            _measurements = measurements;
            _createdAt = createdAt;
            _id = id;
            _isSavedLocally = false;
        }

        public bool IsSavedLocally
        {
            get { return _isSavedLocally; }
            set { _isSavedLocally = value; }
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
                return Measurements.First().ImageUrl;
            }
        }
    }
}