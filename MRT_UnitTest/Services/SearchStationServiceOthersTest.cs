using NUnit.Framework;
using MRT.Services;
using System;
namespace MRT_UnitTest.Services
{
    [TestFixture]
    public class SearchStationServiceOthersTest
    {
        private SearchStationService searchStationService;
        [SetUp]
        public void Setup()
        {
            searchStationService = new SearchStationService();
        }

        [Test]
        public void TestGetMatchingLines_OneMatchingLine()
        {

        }

        [Test]
        public void TestGetMatchingLines_ManyMatchingLine()
        {
        }

        [Test]
        public void TestGetMatchingLines_NoMatchingLine()
        {
        }
    }
}
