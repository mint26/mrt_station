using System;
using System.Collections.Generic;
namespace MRT.Models
{
    public class Station
    {
        private string StationCode { get; set; }
        private string StationName { get; set; }
        private bool IsInterchange { get; set; }
        private DateTime CommencementDate { get; set; }
        private IList<StationEdge> ConnectedStations { get; set; }
        
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

        public bool GetIsInterchange()
        {
            return this.IsInterchange;
        }

        public IList<StationEdge> GetConnectedStations() {
            return this.ConnectedStations?? null; 
        }

        public string GetStationName() {
            return this.StationName;
        }

        public string GetMrtLine() {
            return this.StationCode.Substring(0, 2); 
        }
    }
}
