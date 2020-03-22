namespace MRT.Models
{
    public class RouteStation
    {
        public RouteStation() { }
        public RouteStation(Station station)
        {
            Station = station;
            NextStation = null;
        }

        public Station Station { get; set; }
        public RouteStation NextStation { get; set; }
        public RouteStation PrevStation { get; set; }
    }
}
