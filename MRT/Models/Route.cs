using System;

namespace MRT.Models
{
    public class Route: ICloneable
    {
        public Route()
        {
            TotalDuration = 0;
            LastStation = null;
        }

        public RouteStation LastStation { get; set; }
        public int TotalDuration { get; set; }

        public void AddStationToRoute(Station station) {

            RouteStation newRouteStation = new RouteStation(station);

            if (LastStation == null) {
                LastStation = newRouteStation;
                return; 
            }
            if (LastStation.Station != null) {
                LastStation.NextStation = newRouteStation;
                newRouteStation.PrevStation = LastStation; 
                LastStation = newRouteStation; 
            } 
        }

        public void AddTotalDuration(int duration) {
            TotalDuration += duration; 
        }

        public virtual object Clone()
        {
            return new Route()
            {
                LastStation = new RouteStation {
                    NextStation = LastStation.NextStation,
                    PrevStation = LastStation.PrevStation,
                    Station = LastStation.Station, 
                },
                TotalDuration = TotalDuration,
            };
        }

    }
}
