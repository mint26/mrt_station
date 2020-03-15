using System;
namespace MRT.Models
{
    public class StationEdge
    {
        private Station Station { get; set; }
        private int Duration { get; set; }

        public StationEdge()
        {
        }

        public StationEdge(Station station, int duration) {
            this.Station = station;
            this.Duration = duration; 
        }

        public Station GetStation() {
            return this.Station; 
        }

        public int GetDuration() {
            return this.Duration; 
        }
    }
}
