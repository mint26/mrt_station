using NUnit.Framework;
using MRT.Services;
using MRT.Models;
using System.Collections.Generic;
using System;

namespace MRT_UnitTest.Services
{
    [TestFixture]
    public class FormatRouteTest
    {
        private SearchStationService _searchStationService;
        [SetUp]
        public void Setup()
        {
            _searchStationService = new SearchStationService();
            var task = _searchStationService.ImportDataset("Datasets/StationMap.csv", new DateTime(2022, 1, 1));
            task.Wait();
        }

        [Test]
        public void TestFormatRouteToRouteDTO_JooKoonToJurongEastSameLine()
        {
            var greenRoute = _searchStationService.FindKShortestPath("Joo Koon", "Jurong East", 3);
            var instructions = _searchStationService.FormatRouteToRouteDTO(greenRoute[0]);
            List<string> expected = new List<string>{
                "Take EW from Joo Koon to Pioneer",
                "Take EW from Pioneer to Boon Lay",
                "Take EW from Boon Lay to Lakeside",
                "Take EW from Lakeside to Chinese Garden",
                "Take EW from Chinese Garden to Jurong East"
            };
            Assert.AreEqual(instructions.Instructions, expected);
        }

        [Test]
        public void TestFormatRouteToRouteDTO_LakesideToYewTeeOneInterchange()
        {
            var route = _searchStationService.FindKShortestPath("Lakeside", "Yew Tee", 3);
            var instructions = _searchStationService.FormatRouteToRouteDTO(route[0]);
            List<string> expected = new List<string>{
                "Take EW from Lakeside to Chinese Garden",
                "Take EW from Chinese Garden to Jurong East",
                "Change from EW line to NS line",
                "Take NS from Jurong East to Bukit Batok",
                "Take NS from Bukit Batok to Bukit Gombak",
                "Take NS from Bukit Gombak to Choa Chu Kang",
                "Take NS from Choa Chu Kang to Yew Tee"
            };
            Assert.AreEqual(instructions.Instructions, expected);
        }

        [Test]
        public void TestFormatRouteToRouteDTO_EunosToKovanTwoInterchange()
        {
            var route = _searchStationService.FindKShortestPath("Eunos", "Kovan", 3);
            var instructions = _searchStationService.FormatRouteToRouteDTO(route[0]);
            List<string> expected = new List<string>{
                "Take EW from Eunos to Paya Lebar",
                "Change from EW line to CC line",
                "Take CC from Paya Lebar to MacPherson",
                "Take CC from MacPherson to Tai Seng",
                "Take CC from Tai Seng to Bartley",
                "Take CC from Bartley to Serangoon",
                "Change from CC line to NE line",
                "Take NE from Serangoon to Kovan"
            };
            Assert.AreEqual(instructions.Instructions, expected);
        }

        [Test]
        public void TestFormatRouteToRouteDTO_MarinaBayToFarrerRoadThreeInterchange()
        {
            var route = _searchStationService.FindKShortestPath("Marina Bay", "Farrer Road", 3);
            var instructions = _searchStationService.FormatRouteToRouteDTO(route[0]);
            List<string> expected = new List<string>{
                "Take NS from Marina Bay to Raffles Place",
                "Take NS from Raffles Place to City Hall",
                "Take NS from City Hall to Dhoby Ghaut",
                "Change from NS line to NE line",
                "Take NE from Dhoby Ghaut to Little India",
                "Change from NE line to DT line",
                "Take DT from Little India to Newton",
                "Take DT from Newton to Stevens",
                "Take DT from Stevens to Botanic Gardens",
                "Change from DT line to CC line",
                "Take CC from Botanic Gardens to Farrer Road"
            };
            Assert.AreEqual(instructions.Instructions, expected);
        }

        [Test]
        public void TestFormatRouteToRouteDTO_MarinaBayToNewton()
        {
            var route = _searchStationService.FindKShortestPath("Marina Bay", "Newton", 3);
            var instructions = _searchStationService.FormatRouteToRouteDTO(route[0]);
            List<string> expected = new List<string>{
                "Take NS from Marina Bay to Raffles Place",
                "Take NS from Raffles Place to City Hall",
                "Take NS from City Hall to Dhoby Ghaut",
                "Change from NS line to NE line",
                "Take NE from Dhoby Ghaut to Little India",
                "Change from NE line to DT line",
                "Take DT from Little India to Newton"
            };
            Assert.AreEqual(instructions.Instructions, expected);
        }

        [Test]
        public void TestFormatRouteToRouteDTO_MarymountToDhobyGhaut()
        {
            var route = _searchStationService.FindKShortestPath("Marymount", "Dhoby Ghaut", 3);
            var instructions = _searchStationService.FormatRouteToRouteDTO(route[0]);
            List<string> expected = new List<string>{
                "Take CC from Marymount to Caldecott",
                "Change from CC line to TE line",
                "Take TE from Caldecott to Mount Pleasant",
                "Take TE from Mount Pleasant to Stevens",
                "Change from TE line to DT line",
                "Take DT from Stevens to Newton",
                "Take DT from Newton to Little India",
                "Change from DT line to NE line",
                "Take NE from Little India to Dhoby Ghaut"

            };
            Assert.AreEqual(instructions.Instructions, expected);
        }

        [Test]
        public void TestFormatRouteToRouteDTO_TanjongPagarToPromenade()
        {
            var route = _searchStationService.FindKShortestPath("Tanjong Pagar", "Promenade", 3);
            var instructions = _searchStationService.FormatRouteToRouteDTO(route[0]);
            List<string> expected = new List<string>{
                "Take EW from Tanjong Pagar to Raffles Place",
                "Take EW from Raffles Place to City Hall",
                "Take EW from City Hall to Bugis",
                "Change from EW line to DT line",
                "Take DT from Bugis to Promenade"
            };
            Assert.AreEqual(instructions.Instructions, expected);
        }

        [Test]
        public void TestFormatRoutes_NoPossibleRoute()
        {
            var routes = _searchStationService.FormatRoutes(new List<Route>());
            Assert.IsNull(routes);
        }

        [Test]
        public void TestFormatRoutes_HasPossibleRoute()
        {
            var clementi = new Station
            {
                StationCode = "EW23",
                StationName = "Clementi",
                IsInterchange = false,
                AlternativeStationCodes = new List<string> { "EW23" }
            };
            var dover = new Station
            {
                StationCode = "EW22",
                StationName = "Dover",
                IsInterchange = false,
                AlternativeStationCodes = new List<string> { "EW22" }
            };
            var routes = _searchStationService.FormatRoutes(new List<Route> {
                new Route {
                    TotalDuration = 5,
                    LastStation = new RouteStation {
                        Station = clementi,
                        PrevStation = new RouteStation {
                            Station = dover
                        }
                    }
                }

            });
            Assert.IsNotNull(routes);

            Assert.AreEqual(routes[0].Instructions, new List<string> {
                "Take EW from Dover to Clementi"
                });
            Assert.AreEqual(routes[0].RouteStations, new List<string> { "EW22", "EW23" });
            Assert.AreEqual(5, routes[0].TotalDurations);
            Assert.AreEqual(2, routes[0].TotalStations);
        }
    }
}