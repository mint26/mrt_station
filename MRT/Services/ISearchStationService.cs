using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MRT.Models; 
namespace MRT.Services
{
    public interface ISearchStationService {
        IList<RouteDTO> GetRoutes(string sourceStationCode, string destStationCode, DateTime searchTime);
        void ImportDataset(string filepath, DateTime atDate);
        IList<RouteDTO> FormatRoutes(IList<Route> possibleRoutes, string destStationCode);
    }

    public class RouteDTO
    {
        public List<string> Instructions { get; set; }
        public List<string> RouteStations { get; set; }
        public int TotalDurations { get; set; }
        public int TotalStations { get; set; }
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

        private const string CHANGE_LINE_FORMAT = "Change from {0} line to {1} line";
        private const string GO_TO_NEXT_STATION_FORMAT = "Take {0} from {1} to {2}";

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

            return FormatRoutes(possibleRoutes, destStationCode); 
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
            visitedStationName.Add(curStation.StationName);
            bool foundKRoutes = false;
            while (stationQueue.Count > 0 && !foundKRoutes)
            {

                curStation = stationQueue.Dequeue();
                List<Route> updatedRoutes = new List<Route>();
                

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
                        visitedStationName.Add(curStation.StationName);
                        stationQueue.Enqueue(connectedStation);
                        updatedRoutes.AddRange(SpawnMatchedRoute(possibleRoutes, curStation.StationName, connectedStation, stationEdge.Duration));
                    }
                }

                if (updatedRoutes.Count > 0) {
                    updatedRoutes.AddRange(SpawnMismatchedRoute(possibleRoutes, curStation.StationName));
                    possibleRoutes = updatedRoutes;
                }
            }
            return kRoutes; 
        }

        public IList<Route> SpawnMatchedRoute(IList<Route> possibleRoutes, string prevStationName, Station stationToAdd, int duration) {
            IList<Route> newRoutes = new List<Route>();

            foreach (Route route in possibleRoutes) {
                if (route.LastStation.Station.StationName == prevStationName)
                {
                    Route updatedRoute = (Route)route.Clone();
                    updatedRoute.AddStationToRoute(stationToAdd);
                    updatedRoute.AddTotalDuration(duration);
                    newRoutes.Add(updatedRoute);
                }
            }

            return newRoutes; 
        }

        public IList<Route> SpawnMismatchedRoute(IList<Route> possibleRoutes, string prevStationName)
        {
            IList<Route> newRoutes = new List<Route>();

            foreach (Route route in possibleRoutes)
            {
                if (route.LastStation.Station.StationName != prevStationName)
                {
                    newRoutes.Add(route);
                }
            }

            return newRoutes;
        }

        public IList<Route> SpawnValidRoute(IList<Route> possibleRoutes, string lastStationName, Station finalStation, int duration)
        {
            IList<Route> validRoutes = new List<Route>();

            foreach (Route route in possibleRoutes)
            {
                if (route.LastStation.Station.StationName == lastStationName)
                {
                    route.AddStationToRoute(finalStation);
                    route.AddTotalDuration(duration);
                    validRoutes.Add(route);
                }
            }
            return validRoutes;
        }
            
        public IList<RouteDTO> FormatRoutes(IList<Route> possibleRoutes, string destStationCode) {
            if (possibleRoutes == null || possibleRoutes.Count == 0) {
                return null; 
            }
            IList<RouteDTO> routeDTOs = new List<RouteDTO>();
            foreach (Route route in possibleRoutes)
            {
                routeDTOs.Add(FormatRouteToRouteDTO(route, destStationCode)); 
            }

            return routeDTOs; 
        }

        public RouteDTO FormatRouteToRouteDTO(Route route, string destStationCode) {
            RouteDTO routeDTO = new RouteDTO();
            RouteStation cur = route.LastStation;
            string prevLine = destStationCode.Substring(0,2);
            Stack<string> instructionStack = new Stack<string>();
            Stack<string> routeStationCodes = new Stack<string>();
            int numStations = 0; 

            while (cur != null) {
                RouteStation prevStation = cur.PrevStation;
                if (prevStation != null) {
                    List<string> matchingLines = GetMatchingLines(cur.Station.AlternativeStationCodes, prevStation.Station.AlternativeStationCodes);
                    if (matchingLines.Count == 1)
                    {
                        if (prevLine != null && prevLine != matchingLines[0])
                        {
                            instructionStack.Push(string.Format(CHANGE_LINE_FORMAT, matchingLines[0], prevLine));
                        }
                        instructionStack.Push(string.Format(GO_TO_NEXT_STATION_FORMAT, matchingLines[0], prevStation.Station.StationName,
                        cur.Station.StationName));

                        prevLine = matchingLines[0];
                    }
                    else
                    {
                        foreach (string line in matchingLines)
                        {
                            if (line == prevLine)
                            {
                                instructionStack.Push(string.Format(GO_TO_NEXT_STATION_FORMAT, prevLine, prevStation.Station.StationName,
                                cur.Station.StationName));
                            }
                        }
                    }
                }
                routeStationCodes.Push(cur.Station.GetStationCodeByMrtLine(prevLine)); 
                cur = cur.PrevStation;
                numStations++; 
            }


            List<string> instructions = new List<string>();
            while (instructionStack.Count > 0) {
                instructions.Add(instructionStack.Pop()); 
            }

            List<string> routeStations = new List<string>();
            while (routeStationCodes.Count > 0)
            {
                routeStations.Add(routeStationCodes.Pop());
            }
            routeDTO.Instructions = instructions;
            routeDTO.TotalDurations = route.TotalDuration;
            routeDTO.TotalStations = numStations;
            routeDTO.RouteStations = routeStations;

            return routeDTO; 
        }

        public List<string> GetMatchingLines(IList<string> prevStationLines, IList<string> nextStationLines) {
            List<string> matchingLines = new List<string>(); 
            for (int i = 0; i < prevStationLines.Count; i++) {
                string prevLine = prevStationLines[i].Substring(0, 2);
                for (int j = 0; j < nextStationLines.Count; j++) {
                    string nextLine = nextStationLines[j].Substring(0, 2); 
                    if ( prevLine == nextLine) {
                        matchingLines.Add(nextLine); 
                    }
                }
            }
            return matchingLines; 
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
                            station.AlternativeStationCodes.Add(stationCode); 
                        }
                        else {
                            station = Stations[stationName];
                            station.SetIsInterchange();
                            station.AlternativeStationCodes.Add(stationCode);
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
