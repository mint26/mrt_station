using System;
using System.Collections.Generic;
using System.Linq;
namespace MRT.Models
{
    public class Station
    {
        public string StationCode { get; set; }
        public string StationName { get; set; }
        public bool IsInterchange { get; set; }
        public DateTime CommencementDate { get; set; }
        public List<StationEdge> ConnectedStations { get; set; }
        public HashSet<string> AlternativeStationCodes { get; set; }
        
        public Station(){}

        public Station(string stationCode, string stationName, DateTime commencementDate) {
            StationCode = stationCode;
            StationName = stationName;
            CommencementDate = commencementDate;
            IsInterchange = false;
            AlternativeStationCodes = new HashSet<string>(); 
        }

        public void AddConnectedStations(StationEdge stationEdge) {
            if (ConnectedStations == null) {
                ConnectedStations = new List<StationEdge>(); 
            }
            ConnectedStations.Add(stationEdge);
        }

        public void SetIsInterchange() {
            IsInterchange = true; 
        }

        public string GetMrtLine() {
            return StationCode.Substring(0, 2); 
        }

        public string GetStationCodeByMrtLine(string mrtLine)
        {
            if (mrtLine == null) return AlternativeStationCodes.First();
            foreach (string s in AlternativeStationCodes)
            {
                if (s.Contains(mrtLine))
                {
                    return s;
                }
            }
            return null;
        }
    }
}
