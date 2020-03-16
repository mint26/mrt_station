using System;
using System.Collections.Generic;
namespace MRT.Models
{
    public class Station
    {
        public string StationCode { get; set; }
        public string StationName { get; set; }
        public bool IsInterchange { get; set; }
        public DateTime CommencementDate { get; set; }
        public IList<StationEdge> ConnectedStations { get; set; }
        
        public Station(){}

        public Station(string stationCode, string stationName, DateTime commencementDate) {
            this.StationCode = stationCode;
            this.StationName = stationName;
            this.CommencementDate = commencementDate;
            this.IsInterchange = false; 
        }

        public void AddConnectedStations(StationEdge stationEdge) {
            if (this.ConnectedStations == null) {
                this.ConnectedStations = new List<StationEdge>(); 
            }
            this.ConnectedStations.Add(stationEdge);
        }

        public void SetIsInterchange() {
            this.IsInterchange = true; 
        }

        public string GetMrtLine() {
            return this.StationCode.Substring(0, 2); 
        }
    }
}
