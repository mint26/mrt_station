using NUnit.Framework;
using MRT.Services;
using MRT.Models; 
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
namespace MRT_UnitTest.Services
{
    [TestFixture]
    public class SearchStationServiceTest
    {
        private ISearchStationService _searchStationService;
        [SetUp]
        public void Setup()
        {
            this._searchStationService = new SearchStationService();
        }

        [Test]
        public void TestImportDataset_AllStationOpened()
        {
            //all stations already opened
            SearchStationService searchStationService = (SearchStationService)_searchStationService;
            searchStationService.ImportDataset("Datasets/StationMap.csv",new DateTime(2022,1,1));
            Assert.NotNull(searchStationService.Stations);
            Assert.AreEqual(136, searchStationService.Stations.Count);
            Assert.NotNull(searchStationService.StationCodeMapping);
            Assert.AreEqual(166, searchStationService.StationCodeMapping.Count);
        }

        [Test]
        public void TestImportDataset_PartialStationOpened() {
            //some stations opened
            SearchStationService searchStationService = (SearchStationService)_searchStationService;
            searchStationService.ImportDataset("Datasets/StationMap.csv", new DateTime(2019, 12, 31));
            Assert.NotNull(searchStationService.Stations);
            Assert.AreEqual(122, searchStationService.Stations.Count);
            Assert.NotNull(searchStationService.StationCodeMapping);
            Assert.AreEqual(147, searchStationService.StationCodeMapping.Count);

        }

        [Test]
        public void TestImportDataset_NoStationOpened() {

            //no station opened.
            SearchStationService searchStationService = (SearchStationService)_searchStationService;
            searchStationService.ImportDataset("Datasets/StationMap.csv", new DateTime(1980, 12, 31));
            Assert.NotNull(searchStationService.Stations);
            Assert.AreEqual(0, searchStationService.Stations.Count);
            Assert.NotNull(searchStationService.StationCodeMapping);
            Assert.AreEqual(0, searchStationService.StationCodeMapping.Count);

        }

        [Test]
        public void TestFormatRoutes()
        {
            Assert.Pass();
        }

        [Test]
        public void TestFindRoutes_OnlyOneRoute()
        {
            SearchStationService searchStationService = DatasetGenerator.InitDataset("Datasets/TestDataset_1.json");
            IList<Route> routes = searchStationService.FindRoutes("Jurong East", "Choa Chu Kang");
            Assert.IsNotNull(routes);
            Assert.AreEqual(1, routes.Count);
        }

        [Test]
        public void TestFindRoutes_NoRoute()
        {
            Assert.Pass();
        }
    }
}

// region
static class DatasetGenerator
{  
    public static SearchStationService InitDataset(string filename)
    {
        SearchStationService searchStationService = new SearchStationService();
        using (StreamReader r = new StreamReader(filename))
        {
            var dataset = JsonConvert.DeserializeObject<Dataset>(r.ReadToEnd());
            searchStationService.StationCodeMapping = dataset.StationCodeMapping;
            searchStationService.Stations = dataset.Stations;
        }
        return searchStationService;
    }
}

public class Dataset {
    public Dictionary<string, Station> Stations { get; set; }
    public Dictionary<string, string> StationCodeMapping { get; set; }

}