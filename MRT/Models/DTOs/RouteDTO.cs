using System.Collections.Generic;
namespace MRT.Models.DTOs
{
    public class RouteDTO
    {
        public List<string> Instructions { get; set; }
        public List<string> RouteStations { get; set; }
        public int TotalDurations { get; set; }
        public int TotalStations { get; set; }
        public string SourceStationName { get; set; }
        public string DestStationName { get; set; }
        public RouteDTO()
        {
            Instructions = new List<string>();
            TotalDurations = 0;
            TotalStations = 0;
            RouteStations = new List<string>();
        }
    }
}
