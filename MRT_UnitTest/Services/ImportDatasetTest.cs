using NUnit.Framework;
using MRT.Services;
using System;
namespace MRT_UnitTest.Services
{
    [TestFixture]
    public class ImportDatasetTest
    {
        private SearchStationService searchStationService;
        [SetUp]
        public void Setup()
        {
            searchStationService = new SearchStationService();
        }

        [Test]
        public void TestImportDataset_AllStationOpened()
        {
            //all stations already opened
            searchStationService.ImportDataset("Datasets/StationMap.csv", new DateTime(2022, 1, 1)).Wait();
            Assert.NotNull(searchStationService.Stations);
            Assert.AreEqual(136, searchStationService.Stations.Count);
            Assert.NotNull(searchStationService.StationCodeMapping);
            Assert.AreEqual(166, searchStationService.StationCodeMapping.Count);
        }

        [Test]
        public void TestImportDataset_PartialStationOpened()
        {
            //some stations opened
            searchStationService.ImportDataset("Datasets/StationMap.csv", new DateTime(2019, 12, 31)).Wait();
            Assert.NotNull(searchStationService.Stations);
            Assert.AreEqual(122, searchStationService.Stations.Count);
            Assert.NotNull(searchStationService.StationCodeMapping);
            Assert.AreEqual(147, searchStationService.StationCodeMapping.Count);

        }

        [Test]
        public void TestImportDataset_NoStationOpened()
        {

            //no station opened.
            searchStationService.ImportDataset("Datasets/StationMap.csv", new DateTime(1980, 12, 31)).Wait();
            Assert.NotNull(searchStationService.Stations);
            Assert.AreEqual(0, searchStationService.Stations.Count);
            Assert.NotNull(searchStationService.StationCodeMapping);
            Assert.AreEqual(0, searchStationService.StationCodeMapping.Count);

        }
    }
}