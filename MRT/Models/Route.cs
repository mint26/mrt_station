using System;
using System.Collections.Generic;

namespace MRT.Models
{
    public class Route: ICloneable
    {
        public Route()
        {
            this.TotalDuration = 0;
            this.LastStation = null;
        }

        public RouteStation LastStation { get; set; }
        public int TotalDuration { get; set; }

        public void AddStationToRoute(Station station) {

            RouteStation newRouteStation = new RouteStation(station);

            if (this.LastStation == null) {
                this.LastStation = newRouteStation;
                return; 
            }
            if (this.LastStation.Station != null) {
                this.LastStation.NextStation = newRouteStation;
                newRouteStation.PrevStation = this.LastStation; 
                this.LastStation = newRouteStation; 
            } 
        }

        public void AddTotalDuration(int duration) {
            this.TotalDuration += duration; 
        }

        public virtual object Clone()
        {
            return new Route()
            {
                LastStation = new RouteStation {
                    NextStation = this.LastStation.NextStation,
                    PrevStation = this.LastStation.PrevStation,
                    Station = this.LastStation.Station, 
                },
                TotalDuration = this.TotalDuration,
                
            };
        }

    }

    public class RouteStation {
        public RouteStation() { }
        public RouteStation(Station station) {
            this.Station = station;
            this.NextStation = null; 
        }

        public Station Station { get; set; }
        public RouteStation NextStation { get; set; }
        public RouteStation PrevStation { get; set; }
    }
}
