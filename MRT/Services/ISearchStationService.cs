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
        string StationCode { get; set; }
        string StationName { get; set; }
    }

    public class SearchStationService: ISearchStationService
    {
        private Dictionary<string, Station> _stations;
        private Dictionary<string, string> _stationCodeMapping;

        private const string FILEPATH = "./Datasets/StationMap.csv";

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
            FindRouteHelper(sourceStationName, destStationName, possibleRoutes, new Route()); 
            return possibleRoutes; 
        }

        public void FindRouteHelper(string sourceStationName, string destStationName,
           IList<Route> possibleRoutes, Route route) {

            if (!Stations.ContainsKey(sourceStationName)) throw new Exception();
            
            Station curStation = Stations[sourceStationName];

            if (curStation.GetStationName() == destStationName)
            {
                IList<Station> stations = route.GetStations(); 
                Station[] routeAr = new Station[stations.Count];
                stations.CopyTo(routeAr,0);
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
            ((List<Route>)possibleRoutes).Sort((Route x,Route y) => x.GetTotalDuration() - y.GetTotalDuration()); 
         
            //format to routeDto
            IList<RouteDTO> routeDTOs = new List<RouteDTO>();

            //Convert data representation routes to natural language

            return routeDTOs; 
        }

        public void ImportDataset(string filepath,DateTime atDate) {

            StationCodeMapping = new Dictionary<string, string>();
            Stations = new Dictionary<string, Station>(); 
            using (StreamReader sr = new StreamReader(filepath)) 
            {
                String line = sr.ReadLine();

                Station prevStation = null; 
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    string stationCode = parts[0];
                    string stationName = parts[1];
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

                        if(prevStation != null) {
                            //TODO: make the weight different
                            StationEdge stationEdge = new StationEdge(station, 5);
                            prevStation.AddConnectedStations(stationEdge);
                        }
                        prevStation = station;
                    }
                }
            }
        }   
    }
}
