using NUnit.Framework;
using MRT.Services;
using System.Collections.Generic;
using System.Linq;
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
            List<string> prevStationCodes = new List<string> { "CC1", "TE2", "EW3"};
            List<string> nextStationCodes = new List<string> { "NS1", "TE4"};
            HashSet<string> matchingLines = searchStationService.GetMatchingLines(prevStationCodes, nextStationCodes);
            Assert.AreEqual(1, matchingLines.Count);
            Assert.AreEqual("TE", matchingLines.First());
        }

        [Test]
        public void TestGetMatchingLines_ManyMatchingLine()
        {
            List<string> prevStationCodes = new List<string> { "CC1", "TE2", "EW3" };
            List<string> nextStationCodes = new List<string> { "EW1", "TE4" };
            HashSet<string> matchingLines = searchStationService.GetMatchingLines(prevStationCodes, nextStationCodes);
            Assert.AreEqual(2, matchingLines.Count);
        }

        [Test]
        public void TestGetMatchingLines_NoMatchingLine()
        {
            List<string> prevStationCodes = new List<string> { "CC1", "TE2", "EW3" };
            List<string> nextStationCodes = new List<string> { "PP1", "AA4" };
            HashSet<string> matchingLines = searchStationService.GetMatchingLines(prevStationCodes, nextStationCodes);
            Assert.AreEqual(0, matchingLines.Count);
        }
    }
}
