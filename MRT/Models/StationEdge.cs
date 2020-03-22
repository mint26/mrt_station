namespace MRT.Models
{
    public class StationEdge
    {
        public Station Station { get; set; }
        public int Duration { get; set; }

        public StationEdge(Station station, int duration) {
            Station = station;
            Duration = duration; 
        }
    }
}
