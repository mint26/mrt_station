using NUnit.Framework;
using System.Collections.Generic;
using System;
using MRT.Models;
namespace MRT_UnitTest.Models
{
    [TestFixture]
    public class StationTest
    {
        private Station station; 
        [SetUp]
        public void Setup()
        {
            station = new Station {
                StationName ="Test",
                StationCode = "T1",
                AlternativeStationCodes = new List<string> { "TT1", "CC1"},

            };
        }

        [Test]
        public void TestGetStationCodeByMrtLine_NoLine() {
            var mrtLine = station.GetStationCodeByMrtLine(null);
            Assert.AreEqual("TT1", mrtLine); 
        }

        [Test]
        public void TestGetStationCodeByMrtLine_MatchedLine()
        {
            var mrtLine = station.GetStationCodeByMrtLine("CC");
            Assert.AreEqual("CC1", mrtLine);
        }
    }
}
