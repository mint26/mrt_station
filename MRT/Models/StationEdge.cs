using System;
namespace MRT.Models
{
    public class StationEdge
    {
        public Station Station { get; set; }
        public int Duration { get; set; }

        public StationEdge()
        {
        }

        public StationEdge(Station station, int duration) {
            this.Station = station;
            this.Duration = duration; 
        }
    }
}
