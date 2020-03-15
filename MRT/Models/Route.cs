using System;
using System.Collections.Generic;

namespace MRT.Models
{
    public class Route
    {
        private IList<Station> Stations;
        private int TotalDuration; 
        public Route()
        {
            this.TotalDuration = 0;
            this.Stations = new List<Station>(); 
        }

        public void AddStation(Station station) {
            this.Stations.Add(station); 
        }

        public IList<Station> GetStations()
        {
            return this.Stations; 
        }

        public void SetStations(IList<Station> stations) {
            this.Stations = stations; 
        }

        public void AddTotalDuration(int duration) {
            this.TotalDuration += duration; 
        }

        public int GetTotalDuration() {
            return this.TotalDuration; 
        }

        public void SetTotalDuration(int totalDuration)
        {
            this.TotalDuration = totalDuration;
        }
    }
}
