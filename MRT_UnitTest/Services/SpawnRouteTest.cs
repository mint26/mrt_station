using NUnit.Framework;
using MRT.Services;
using System;
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
            var task = searchStationService.ImportDataset("Datasets/StationMap.csv", new DateTime(2022, 1, 1));
            task.Wait();
        }

        [Test]
        public void TestSpawnMatchedRoute_OneMatchedRoute() {

        }

        [Test]
        public void TestSpawnMatchedRoute_ManyMatchedRoute()
        {

        }

        [Test]
        public void TestSpawnMatchedRoute_NoMatchedRoute()
        {

        }

        [Test]
        public void TestSpawnMismatchedRoute_OneMatchedRoute()
        {

        }

        [Test]
        public void TestSpawnMismatchedRoute_ManyMatchedRoute()
        {

        }

        [Test]
        public void TestSpawnMismatchedRoute_NoMatchedRoute()
        {

        }

        [Test]
        public void TestSpawnValidRoute_OneMatchedRoute()
        {

        }

        [Test]
        public void TestSpawnValidRoute_ManyMatchedRoute()
        {

        }

        [Test]
        public void TestSpawnValidRoute_NoMatchedRoute()
        {

        }
    }
}
