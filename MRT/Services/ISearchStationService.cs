using System;
using System.Collections.Generic;
using System.IO;
using MRT.Models; 
namespace MRT.Services
{
    public interface ISearchStationService {
        IList<RouteDTO> GetRoutes(string sourceStationCode, string destStationCode, DateTime searchTime);
        void ImportDataset(string filepath, DateTime atDate);
        IList<Route> FindRoutes(string sourceStationName, string destStationName);
        IList<RouteDTO> FormatRoutes(IList<Route> possibleRoutes);
    }

    public class RouteDTO
    {
        IList<string> Instructions;
        int TotalDurations; 
    }

    public class SearchStationService: ISearchStationService
    {
        private Dictionary<string, Station> _stations;
        private Dictionary<string, string> _stationCodeMapping;

        private const string FILEPATH = "./Datasets/StationMap.csv";
        private const int MAX_NUM_RESULT = 3;
        private Route lastMaxRoute = null; 

        public SearchStationService()
        {
        }

        public Dictionary<string, Station> Stations {
            get => _stations;
            set => _stations = value; 
        }

        public Dictionary<string, string> StationCodeMapping
        {
            get => _stationCodeMapping;
            set => _stationCodeMapping = value;
        }

        public IList<RouteDTO> GetRoutes(string sourceStationCode, string destStationCode, DateTime searchDate) {

            ImportDataset(FILEPATH, searchDate);

            string sourceName = StationCodeMapping[sourceStationCode];
            string destName = StationCodeMapping[destStationCode];
            IList<Route> possibleRoutes = FindRoutes(sourceName, destName);

            return FormatRoutes(possibleRoutes); 
        }


        public IList<Route> FindRoutes(string sourceStationName, string destStationName) {
            IList<Route> possibleRoutes = new List<Route>();
            Route route = new Route();
            route.AddStation(Stations[sourceStationName]);
            FindRouteHelper(sourceStationName, destStationName, possibleRoutes, route); 
            return possibleRoutes; 
        }

        public void FindRouteHelper(string sourceStationName, string destStationName,
           IList<Route> possibleRoutes, Route route) {

            if (!Stations.ContainsKey(sourceStationName)) throw new Exception();

            Station curStation = Stations[sourceStationName];

            if (curStation.GetStationName() == destStationName)
            {
                //if (lastMaxRoute == null || lastMaxRoute.GetTotalDuration() > route.GetTotalDuration())
                //{
                IList<Station> stations = route.GetStations();

                Station[] routeAr = new Station[stations.Count];
                stations.CopyTo(routeAr, 0);

                List<Station> newStationList = new List<Station>();
                newStationList.AddRange(routeAr);

                Route validRoute = new Route();
                validRoute.SetStations(newStationList);
                validRoute.SetTotalDuration(route.GetTotalDuration());
                possibleRoutes.Add(validRoute);
                return; 
            }

            IList<StationEdge> connectedStations = curStation.GetConnectedStations();
            if (connectedStations != null)
            {
                foreach (StationEdge stationEdge in connectedStations)
                {
                    Station nextStation = stationEdge.GetStation();
                    if (!route.GetStations().Contains(nextStation))
                    {
                        route.AddStation(nextStation);
                        route.AddTotalDuration(stationEdge.GetDuration());
                        FindRouteHelper(nextStation.GetStationName(), destStationName, possibleRoutes, route);
                        route.AddTotalDuration(-stationEdge.GetDuration());
                        route.GetStations().Remove(nextStation);
                    }
                }
            }
        }
            
        
        public IList<RouteDTO> FormatRoutes(IList<Route> possibleRoutes) {
            if (possibleRoutes == null || possibleRoutes.Count == 0) {
                return null; 
            }
            //sort the route with shortest time
            ((List<Route>)possibleRoutes).Sort((Route x, Route y) => x.GetTotalDuration() - y.GetTotalDuration());


            IList<RouteDTO> routeDTOs = new List<RouteDTO>();
            //format to routeDto
            for (int i = 0; i < 3; i++)
            {
                routeDTOs.Add(FormatRouteToRouteDTO(possibleRoutes[i]));
            }

            return routeDTOs; 
        }

        public RouteDTO FormatRouteToRouteDTO(Route route) {
            RouteDTO routeDTO = new RouteDTO();


            return routeDTO; 
        }

        public void ImportDataset(string filepath,DateTime atDate) {

            StationCodeMapping = new Dictionary<string, string>();
            Stations = new Dictionary<string, Station>(); 
            using (StreamReader sr = new StreamReader(filepath)) 
            {
                String line = sr.ReadLine();

                Station prevStation = null;
                string prevStationLine = ""; 
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    string stationCode = parts[0];
                    string stationName = parts[1];
                    string stationLine = stationCode.Substring(0, 2);
                    DateTime commencementDate = DateTime.Parse(parts[2]);
                    if (commencementDate <= atDate)
                    {
                        if (!StationCodeMapping.ContainsKey(stationCode)) {
                            StationCodeMapping.Add(stationCode, stationName);
                        }

                        Station station;
                        if (!Stations.ContainsKey(stationName))
                        {
                            station = new Station(stationCode, stationName, commencementDate);
                            Stations.Add(stationName, station);
                        }
                        else {
                            station = Stations[stationName];
                            station.SetIsInterchange();
                        }

                        if (prevStation != null && (prevStationLine == "" || prevStationLine == stationLine)) {
                            //TODO: make the weight different
                            StationEdge prevStationEdge = new StationEdge(station, 5);
                            prevStation.AddConnectedStations(prevStationEdge);

                            StationEdge curStationEdge = new StationEdge(prevStation, 5);
                            station.AddConnectedStations(curStationEdge);
                        }

                        if (prevStation != null && prevStationLine != "" && prevStationLine != stationLine) {
                            prevStation = null;
                            prevStationLine = "";
                            continue; 
                        }

                        prevStation = station;
                        prevStationLine = stationLine; 

                    }
                }
            }
        }   
    }
}
