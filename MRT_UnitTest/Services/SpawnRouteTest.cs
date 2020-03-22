using NUnit.Framework;
using MRT.Services;
using System.Collections.Generic;
using MRT.Models;
namespace MRT_UnitTest.Services
{
    [TestFixture]
    public class SpawnRouteTest
    {
        private SearchStationService searchStationService;
        [SetUp]
        public void Setup()
        {
            searchStationService = new SearchStationService();
            
        }

        [Test]
        public void TestSpawnMatchedRoute_OneMatchedRoute() {
            List<Route> possibleRoutes = new List<Route>
            {
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CC1",
                            StationName = "Tampines",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                },
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CD1",
                            StationName = "Lentor",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                }
            };

            List<Route> matchedRoutes = searchStationService.SpawnMatchedRoute(possibleRoutes, "Tampines", new Station() {
                StationCode = "CC2",
                StationName = "Simei",
            }, 5);
            Assert.AreEqual(1, matchedRoutes.Count);
            Assert.AreEqual("CC2", matchedRoutes[0].LastStation.Station.StationCode);
            Assert.AreEqual("Simei", matchedRoutes[0].LastStation.Station.StationName);

            Assert.IsNotNull(matchedRoutes[0].LastStation.PrevStation);
            Assert.AreEqual("CC1", matchedRoutes[0].LastStation.PrevStation.Station.StationCode);
            Assert.AreEqual("Tampines", matchedRoutes[0].LastStation.PrevStation.Station.StationName);

            Assert.IsNull(matchedRoutes[0].LastStation.NextStation);
        }

        [Test]
        public void TestSpawnMatchedRoute_ManyMatchedRoute()
        {
            List<Route> possibleRoutes = new List<Route>
            {
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CC1",
                            StationName = "Tampines",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                },
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CC1",
                            StationName = "Tampines",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                },
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CD1",
                            StationName = "Lentor",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                }
            };

            List<Route> matchedRoutes = searchStationService.SpawnMatchedRoute(possibleRoutes, "Tampines", new Station()
            {
                StationCode = "CC2",
                StationName = "Simei",
            }, 5);
            Assert.AreEqual(2, matchedRoutes.Count);

            for (int i = 0; i < 2; i++) {
                Assert.AreEqual("CC2", matchedRoutes[i].LastStation.Station.StationCode);
                Assert.AreEqual("Simei", matchedRoutes[i].LastStation.Station.StationName);

                Assert.IsNotNull(matchedRoutes[i].LastStation.PrevStation);
                Assert.AreEqual("CC1", matchedRoutes[i].LastStation.PrevStation.Station.StationCode);
                Assert.AreEqual("Tampines", matchedRoutes[i].LastStation.PrevStation.Station.StationName);

                Assert.IsNull(matchedRoutes[i].LastStation.NextStation);
            }
           
        }

        [Test]
        public void TestSpawnMatchedRoute_NoMatchedRoute()
        {
            List<Route> possibleRoutes = new List<Route>
            {
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CC1",
                            StationName = "Tampines",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                }
            };

            List<Route> matchedRoutes = searchStationService.SpawnMatchedRoute(possibleRoutes, "Test", new Station(), 5);
            Assert.AreEqual(0, matchedRoutes.Count); 

        }

        [Test]
        public void TestSpawnMismatchedRoute_OneMismatchedRoute()
        {
            List<Route> possibleRoutes = new List<Route>
            {
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CC1",
                            StationName = "Tampines",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                },
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CC1",
                            StationName = "Tampines",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                },
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CD1",
                            StationName = "Lentor",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                }
            };

            List<Route> mismatchedRoutes = searchStationService.SpawnMismatchedRoute(possibleRoutes, "Tampines");
            Assert.AreEqual(1, mismatchedRoutes.Count);
            Assert.AreEqual("CD1", mismatchedRoutes[0].LastStation.Station.StationCode);
            Assert.AreEqual("Lentor", mismatchedRoutes[0].LastStation.Station.StationName);

            Assert.IsNull(mismatchedRoutes[0].LastStation.PrevStation);
            Assert.IsNull(mismatchedRoutes[0].LastStation.NextStation);
      
        }

        [Test]
        public void TestSpawnMismatchedRoute_ManyMatchedRoute()
        {
            List<Route> possibleRoutes = new List<Route>
            {
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CC1",
                            StationName = "Tampines",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                },
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CC1",
                            StationName = "Tampines",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                },
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CD1",
                            StationName = "Lentor",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                }
            };

            List<Route> mismatchedRoutes = searchStationService.SpawnMismatchedRoute(possibleRoutes, "Lentor");
            Assert.AreEqual(2, mismatchedRoutes.Count);

            for (int i = 0; i < 2; i++) {
                Assert.AreEqual("CC1", mismatchedRoutes[i].LastStation.Station.StationCode);
                Assert.AreEqual("Tampines", mismatchedRoutes[i].LastStation.Station.StationName);
                Assert.IsNull(mismatchedRoutes[i].LastStation.PrevStation);
                Assert.IsNull(mismatchedRoutes[i].LastStation.NextStation);
            }
            
        }

        [Test]
        public void TestSpawnMismatchedRoute_NoMatchedRoute()
        {
            List<Route> possibleRoutes = new List<Route>
            {
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CC1",
                            StationName = "Tampines",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                },
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CC1",
                            StationName = "Tampines",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                }
            };

            List<Route> mismatchedRoutes = searchStationService.SpawnMismatchedRoute(possibleRoutes, "Tampines");
            Assert.AreEqual(0, mismatchedRoutes.Count);
        }

        [Test]
        public void TestSpawnValidRoute_OneMatchedRoute()
        {
            List<Route> possibleRoutes = new List<Route>
            {
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CC1",
                            StationName = "Tampines",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                },
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CD1",
                            StationName = "Lentor",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                }
            };

            List<Route> validRoutes = searchStationService.SpawnValidRoute(possibleRoutes, "Tampines", new Station()
            {
                StationCode = "CC2",
                StationName = "Simei",
            }, 5);
            Assert.AreEqual(1, validRoutes.Count);
            Assert.AreEqual("CC2", validRoutes[0].LastStation.Station.StationCode);
            Assert.AreEqual("Simei", validRoutes[0].LastStation.Station.StationName);

            Assert.IsNotNull(validRoutes[0].LastStation.PrevStation);
            Assert.AreEqual("CC1", validRoutes[0].LastStation.PrevStation.Station.StationCode);
            Assert.AreEqual("Tampines", validRoutes[0].LastStation.PrevStation.Station.StationName);

            Assert.IsNull(validRoutes[0].LastStation.NextStation);

        }

        [Test]
        public void TestSpawnValidRoute_ManyMatchedRoute()
        {
            List<Route> possibleRoutes = new List<Route>
            {
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CC1",
                            StationName = "Tampines",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                },
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CC1",
                            StationName = "Tampines",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                },
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CD1",
                            StationName = "Lentor",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                }
            };

            List<Route> validRoutes = searchStationService.SpawnValidRoute(possibleRoutes, "Tampines", new Station()
            {
                StationCode = "CC2",
                StationName = "Simei",
            }, 5);
            Assert.AreEqual(2, validRoutes.Count);

            for (int i = 0; i < 2; i++)
            {
                Assert.AreEqual("CC2", validRoutes[i].LastStation.Station.StationCode);
                Assert.AreEqual("Simei", validRoutes[i].LastStation.Station.StationName);

                Assert.IsNotNull(validRoutes[i].LastStation.PrevStation);
                Assert.AreEqual("CC1", validRoutes[i].LastStation.PrevStation.Station.StationCode);
                Assert.AreEqual("Tampines", validRoutes[i].LastStation.PrevStation.Station.StationName);

                Assert.IsNull(validRoutes[i].LastStation.NextStation);
            }
        }

        [Test]
        public void TestSpawnValidRoute_NoMatchedRoute()
        {
            List<Route> possibleRoutes = new List<Route>
            {
                new Route(){
                    LastStation = new RouteStation{
                        Station = new Station(){
                            StationCode = "CC1",
                            StationName = "Tampines",
                        },
                        PrevStation = null,
                        NextStation = null
                    },
                    TotalDuration = 5,
                }
            };

            List<Route> validRoutes = searchStationService.SpawnValidRoute(possibleRoutes, "Test", new Station(), 5);
            Assert.AreEqual(0, validRoutes.Count);
        }
    }
}
