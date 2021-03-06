﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MRT.Models;
using MRT.Models.DTOs;
using MRT.Constants;
using System.Threading.Tasks;

namespace MRT.Services
{
    public interface ISearchStationService
    {
        Task<List<RouteDTO>> GetRoutesAsync(string sourceStationCode, string destStationCode, DateTime searchTime);
    }

    public class SearchStationService : ISearchStationService
    {
        public Dictionary<string, Station> Stations { get; set; }
        public Dictionary<string, string> StationCodeMapping { get; set; }

        public async Task<List<RouteDTO>> GetRoutesAsync(string sourceStationCode, string destStationCode, DateTime searchDate)
        {

            await ImportDataset(Consts.FILEPATH, searchDate);

            if (!StationCodeMapping.ContainsKey(sourceStationCode) || !StationCodeMapping.ContainsKey(destStationCode)) {
                return null;
            }
            string sourceName = StationCodeMapping[sourceStationCode];
            string destName = StationCodeMapping[destStationCode];
            List<Route> possibleRoutes = FindKShortestPath(sourceName, destName, Consts.MAX_NUM_RESULT);

            return FormatRoutes(possibleRoutes);
        }

        public List<Route> FindKShortestPath(string sourceStationName, string destStationName, int k)
        {

            List<Route> kRoutes = new List<Route>();

            Queue<Station> stationQueue = new Queue<Station>();
            HashSet<string> visitedStationName = new HashSet<string>();
            Station curStation = Stations[sourceStationName];
            stationQueue.Enqueue(curStation);

            List<Route> possibleRoutes = new List<Route>();
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

                    //valid route found. 
                    if (connectedStation.StationName == destStationName && kRoutes.Count < k)
                    {
                        List<Route> validRoutes = SpawnValidRoute(possibleRoutes, curStation.StationName, connectedStation, stationEdge.Duration);
                        int availableRouteSlots = k - kRoutes.Count < validRoutes.Count ? k - kRoutes.Count : validRoutes.Count;
                        for (int i = 0; i < availableRouteSlots; i++)
                        {
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

                if (updatedRoutes.Count > 0)
                {
                    updatedRoutes.AddRange(SpawnMismatchedRoute(possibleRoutes, curStation.StationName));
                    possibleRoutes = updatedRoutes;
                }
            }

            kRoutes = kRoutes.OrderBy(route => route.TotalDuration).ToList();
            return kRoutes;
        }

        // Find the route with matching last station name and extend with new station name. 
        public List<Route> SpawnMatchedRoute(List<Route> possibleRoutes, string prevStationName, Station stationToAdd, int duration)
        {
            List<Route> newRoutes = new List<Route>();

            foreach (Route route in possibleRoutes)
            {
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

        // Find the route with no matching last station name
        public List<Route> SpawnMismatchedRoute(List<Route> possibleRoutes, string prevStationName)
        {
            List<Route> newRoutes = new List<Route>();

            foreach (Route route in possibleRoutes)
            {
                if (route.LastStation.Station.StationName != prevStationName)
                {
                    newRoutes.Add(route);
                }
            }

            return newRoutes;
        }

        //Find the final route with matching last station name
        public List<Route> SpawnValidRoute(List<Route> possibleRoutes, string lastStationName, Station finalStation, int duration)
        {
            List<Route> validRoutes = new List<Route>();

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

        public List<RouteDTO> FormatRoutes(List<Route> possibleRoutes)
        {
            if (possibleRoutes == null || possibleRoutes.Count == 0)
            {
                return null;
            }
            List<RouteDTO> routeDTOs = new List<RouteDTO>();
            foreach (Route route in possibleRoutes)
            {
                routeDTOs.Add(FormatRouteToRouteDTO(route));
            }

            return routeDTOs;
        }

        public RouteDTO FormatRouteToRouteDTO(Route route)
        {
            RouteDTO routeDTO = new RouteDTO();
            RouteStation cur = route.LastStation;
            routeDTO.DestStationName = cur.Station.StationName; 
            string prevLine = null;
            Stack<string> instructionStack = new Stack<string>();
            Stack<string> routeStationCodes = new Stack<string>();
            int numStations = 0;

            while (cur != null)
            {
                RouteStation prevStation = cur.PrevStation;
                if (prevStation != null)
                {
                    HashSet<string> matchingLines = GetMatchingLines(cur.Station.AlternativeStationCodes, prevStation.Station.AlternativeStationCodes);
                    //mrt line for this station can be determined
                    if (matchingLines.Count == 1)
                    {
                        string matchedLine = matchingLines.First();
                        //if mrt line for the determined station is not the same as prev mrt line
                        if (prevLine != null && prevLine != matchedLine)
                        {
                            instructionStack.Push(string.Format(Consts.CHANGE_LINE_FORMAT, matchedLine, prevLine));
                        }
                        instructionStack.Push(string.Format(Consts.GO_TO_NEXT_STATION_FORMAT, matchedLine, prevStation.Station.StationName,
                        cur.Station.StationName));

                        prevLine = matchedLine;
                    }
                    else
                    {
                        //if there is more than 1 possible mrt line
                        foreach (string line in matchingLines)
                        {
                            if (prevLine == null) {
                                prevLine = cur.Station.AlternativeStationCodes.First().Substring(0, 2); 
                            }
                            if (line == prevLine)
                            {
                                instructionStack.Push(string.Format(Consts.GO_TO_NEXT_STATION_FORMAT, prevLine, prevStation.Station.StationName,
                                cur.Station.StationName));
                            }
                        }
                    }
                }
                routeStationCodes.Push(cur.Station.GetStationCodeByMrtLine(prevLine));
                
                if (cur.PrevStation == null) {
                    routeDTO.SourceStationName = cur.Station.StationName;
                }

                cur = cur.PrevStation;
                numStations++;

            }


            List<string> instructions = new List<string>();
            while (instructionStack.Count > 0)
            {
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

        //Get the matching mrt lines(eg: CC, EW, etc) between two list of station codes. 
        public HashSet<string> GetMatchingLines(List<string> prevStationCodes, List<string> nextStationCodes)
        {
            HashSet<string> commonStationLines = new HashSet<string>(prevStationCodes.Select(x=> x.Substring(0,2)));
            commonStationLines.IntersectWith(nextStationCodes.Select(x=> x.Substring(0,2)));
            return commonStationLines;
        }

        public async Task ImportDataset(string filepath, DateTime atDate)
        {

            StationCodeMapping = new Dictionary<string, string>();
            Stations = new Dictionary<string, Station>();


            using (StreamReader sr = new StreamReader(filepath))
            {
                string line = await sr.ReadLineAsync();

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
                        if (!StationCodeMapping.ContainsKey(stationCode))
                        {
                            StationCodeMapping.Add(stationCode, stationName);
                        }

                        Station station;
                        if (!Stations.ContainsKey(stationName))
                        {
                            station = new Station(stationCode, stationName, commencementDate);
                            Stations.Add(stationName, station);
                            station.AlternativeStationCodes.Add(stationCode);
                        }
                        else
                        {
                            station = Stations[stationName];
                            station.IsInterchange = true;
                            station.AlternativeStationCodes.Add(stationCode);
                        }

                        if (prevStation != null && (prevStationLine == "" || prevStationLine == stationLine))
                        {
                            //TODO: make the weight different
                            StationEdge prevStationEdge = new StationEdge(station, Consts.WEIGHT);
                            prevStation.ConnectedStations.Add(prevStationEdge);

                            StationEdge curStationEdge = new StationEdge(prevStation, Consts.WEIGHT);
                            station.ConnectedStations.Add(curStationEdge);
                        }

                        if (prevStation != null && prevStationLine != "" && prevStationLine != stationLine)
                        {
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

