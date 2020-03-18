using System;
using System.Collections.Generic;
using System.IO;
using MRT.Models; 
namespace MRT.Services
{
    public interface ISearchStationService {
        IList<RouteDTO> GetRoutes(string sourceStationCode, string destStationCode, DateTime searchTime);
        void ImportDataset(string filepath, DateTime atDate);
        IList<RouteDTO> FormatRoutes(IList<Route> possibleRoutes);
    }

    public class RouteDTO
    {
        public IList<string> Instructions { get; set; }
        public int TotalDurations { get; set; }
        public RouteDTO() {
            this.Instructions = new List<string>();
            this.TotalDurations = 0; 
        }
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
            IList<Route> possibleRoutes = FindKShortestPath(sourceName, destName, MAX_NUM_RESULT);

            return FormatRoutes(possibleRoutes); 
        }

        public IList<Route> FindKShortestPath(string sourceStationName, string destStationName, int k) {
            if (!Stations.ContainsKey(sourceStationName)) throw new Exception();

            List<Route> kRoutes = new List<Route>();
 
            Queue<Station> stationQueue = new Queue<Station>();
            HashSet<string> visitedStationName = new HashSet<string>();
            Station curStation = Stations[sourceStationName];
            stationQueue.Enqueue(curStation);

            IList<Route> possibleRoutes = new List<Route>();
            Route startRoute = new Route();
            startRoute.AddStationToRoute(curStation);
            possibleRoutes.Add(startRoute);

            bool foundKRoutes = false;
            while (stationQueue.Count > 0 && !foundKRoutes)
            {

                curStation = stationQueue.Dequeue();
                List<Route> updatedRoutes = new List<Route>();
                visitedStationName.Add(curStation.StationName);

                foreach (StationEdge stationEdge in curStation.ConnectedStations)
                {
                    Station connectedStation = stationEdge.Station;
                    
                    if (connectedStation.StationName == destStationName && kRoutes.Count < k)
                    {
                        IList<Route> validRoutes = SpawnValidRoute(possibleRoutes, curStation.StationName, connectedStation, stationEdge.Duration);
                        int availableRouteSlots = k - kRoutes.Count < validRoutes.Count ? k - kRoutes.Count : validRoutes.Count;
                        for (int i = 0; i < availableRouteSlots; i++) {
                            kRoutes.Add(validRoutes[i]);
                            possibleRoutes.Remove(validRoutes[i]); 
                        }

                        if (kRoutes.Count >= k)
                        {
                            foundKRoutes = true;
                            break; 
                        }
                        continue; 
                    }
                    if (!visitedStationName.Contains(connectedStation.StationName))
                    {
                        stationQueue.Enqueue(connectedStation);
                        updatedRoutes.AddRange(SpawnRoute(possibleRoutes, curStation.StationName, connectedStation, stationEdge.Duration));
                    }
                }

                if (updatedRoutes.Count > 0) {
                    possibleRoutes = updatedRoutes;
                }
            }
            return kRoutes; 
        }

        public IList<Route> SpawnRoute(IList<Route> possibleRoutes, string prevStationName, Station stationToAdd, int duration) {
            IList<Route> newRoutes = new List<Route>();

            foreach (Route route in possibleRoutes) {
                if (route.LastStation.Station.StationName == prevStationName)
                {
                    Route updatedRoute = (Route)route.Clone();
                    updatedRoute.AddStationToRoute(stationToAdd);
                    updatedRoute.AddTotalDuration(duration);
                    newRoutes.Add(updatedRoute);
                }
                else {
                    newRoutes.Add(route); 
                }
                
            }

            return newRoutes; 
        }

        public IList<Route> SpawnValidRoute(IList<Route> possibleRoutes, string lastStationName, Station finalStation, int duration)
        {
            IList<Route> validRoutes = new List<Route>();

            foreach (Route route in possibleRoutes) {
                if (route.LastStation.Station.StationName == lastStationName) {
                    ///Route updatedRoute = (Route)route.Clone();
                    route.AddStationToRoute(finalStation);
                    route.AddTotalDuration(duration);
                    validRoutes.Add(route);
                }
            }
            return validRoutes; 
        }

        //public IList<Route> FindRoutes(string sourceStationName, string destStationName) {
        //    IList<Route> possibleRoutes = new List<Route>();
        //    Route route = new Route();
        //    route.AddStation(Stations[sourceStationName]);
        //    FindRouteHelper(sourceStationName, destStationName, possibleRoutes, route); 
        //    return possibleRoutes; 
        //}

        //public void FindRouteHelper(string sourceStationName, string destStationName,
        //   IList<Route> possibleRoutes, Route route) {

        //    if (!Stations.ContainsKey(sourceStationName)) throw new Exception();

        //    Station curStation = Stations[sourceStationName];

        //    if (curStation.GetStationName() == destStationName)
        //    {
        //        //if (lastMaxRoute == null || lastMaxRoute.GetTotalDuration() > route.GetTotalDuration())
        //        //{
        //        IList<Station> stations = route.GetStations();

        //        Station[] routeAr = new Station[stations.Count];
        //        stations.CopyTo(routeAr, 0);

        //        List<Station> newStationList = new List<Station>();
        //        newStationList.AddRange(routeAr);

        //        Route validRoute = new Route();
        //        validRoute.SetStations(newStationList);
        //        validRoute.SetTotalDuration(route.GetTotalDuration());
        //        possibleRoutes.Add(validRoute);
        //        return; 
        //    }

        //    IList<StationEdge> connectedStations = curStation.GetConnectedStations();
        //    if (connectedStations != null)
        //    {
        //        foreach (StationEdge stationEdge in connectedStations)
        //        {
        //            Station nextStation = stationEdge.GetStation();
        //            if (!route.GetStations().Contains(nextStation))
        //            {
        //                route.AddStation(nextStation);
        //                route.AddTotalDuration(stationEdge.GetDuration());
        //                FindRouteHelper(nextStation.GetStationName(), destStationName, possibleRoutes, route);
        //                route.AddTotalDuration(-stationEdge.GetDuration());
        //                route.GetStations().Remove(nextStation);
        //            }
        //        }
        //    }
        //}
            
        
        public IList<RouteDTO> FormatRoutes(IList<Route> possibleRoutes) {
            if (possibleRoutes == null || possibleRoutes.Count == 0) {
                return null; 
            }
            IList<RouteDTO> routeDTOs = new List<RouteDTO>();
            foreach (var route in possibleRoutes)
            {
                RouteDTO routeDto = new RouteDTO();
                routeDto.TotalDurations = route.TotalDuration;

                RouteStation current = route.LastStation;
                Queue<Station> tmpQueue = new Queue<Station>();
                Station lastUpdatedStation = current.Station;
                string curActiveLine = ""; 
                while (true)
                {
                    if (current.PrevStation == null)
                    {
                        string instruction = string.Format("Take {0} line from {1} to {2}", curActiveLine,
                            current.Station.StationName, lastUpdatedStation.StationName);
                        routeDto.Instructions.Add(instruction);
                        break;
                    }
                    else if (!current.PrevStation.Station.IsInterchange)
                    {
                        Station prevStation = current.PrevStation.Station;
                        string curLine = prevStation.StationCode.Substring(0, 2).ToUpper();
                        while (tmpQueue.Count > 0){
                            Station nextStation = tmpQueue.Dequeue(); 
                            string prevLine = lastUpdatedStation.StationCode.Substring(0, 2);
                            string transitInstruction = string.Format("Change from {0} line to {1} line", curLine, prevLine);
                            string forwardInstruction = string.Format("Take {0} line from {1} to {2}", curLine,
                                lastUpdatedStation.StationName, nextStation.StationName);
                            routeDto.Instructions.Add(transitInstruction);
                            routeDto.Instructions.Add(forwardInstruction);
                        }
                        string instruction = string.Format("Take {0} line from {1} to {2}", curLine, prevStation.StationName, current.Station.StationName);
                        routeDto.Instructions.Add(instruction);
                        lastUpdatedStation = prevStation;
                        curActiveLine = curLine; 
                    }
                    else if (current.PrevStation.Station.IsInterchange)
                    { 
                        tmpQueue.Enqueue(current.PrevStation.Station);
                    }
                    current = current.PrevStation;
                }
                routeDTOs.Add(routeDto); 
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
