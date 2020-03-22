using System;
using NUnit.Framework;
using MRT.Services;

namespace MRT_UnitTest.Services
{
    [TestFixture]
    public class FindKShortestPathTest
    {
        private SearchStationService searchStationService;
        [SetUp]
        public void Setup()
        {
            searchStationService = new SearchStationService();
            var task = searchStationService.ImportDataset("Datasets/StationMap.csv", new DateTime(2022, 1, 1));
            task.Wait();
        }

        #region TestFindKShortestPath

        [Test]
        public void TestFindKShortestPath_SameLineRoute()
        {
            var greenRoute = searchStationService.FindKShortestPath("Joo Koon", "Jurong East", 3);
            var greenLine = new String[] { "Jurong East", "Chinese Garden", "Lakeside", "Boon Lay", "Pioneer", "Joo Koon" };
            Assert.IsTrue(TestUtility.IsSameRoute(greenLine, greenRoute[0]));

            var redRoute = searchStationService.FindKShortestPath("Toa Payoh", "Yishun", 3);
            var redLine = new String[] { "Yishun", "Khatib", "Yio Chu Kang", "Ang Mo Kio", "Bishan", "Braddell", "Toa Payoh" };
            Assert.IsTrue(TestUtility.IsSameRoute(redLine, redRoute[0]));

            var purpleRoute = searchStationService.FindKShortestPath("Sengkang", "Hougang", 3);
            var purpleLine = new String[] { "Hougang", "Buangkok", "Sengkang" };
            Assert.IsTrue(TestUtility.IsSameRoute(purpleLine, purpleRoute[0]));
        }

        [Test]
        public void TestFindKShortestPath_NoRoute()
        {
            Assert.Pass();
        }

        [Test]
        public void TestFindKShortestPath_GreenRedLineRoute()
        {
            var route = searchStationService.FindKShortestPath("Lakeside", "Yew Tee", 3);
            var expected = new String[] { "Yew Tee", "Choa Chu Kang", "Bukit Gombak", "Bukit Batok", "Jurong East", "Chinese Garden", "Lakeside" };
            Assert.IsTrue(TestUtility.IsSameRoute(expected, route[0]));
        }

        [Test]
        public void TestFindKShortestPath_PurpleGreenLineRoute()
        {
            var route = searchStationService.FindKShortestPath("Chinatown", "Commonwealth", 3);
            var expected = new String[] { "Commonwealth", "Queenstown", "Redhill", "Tiong Bahru", "Outram Park", "Chinatown" };
            Assert.IsTrue(TestUtility.IsSameRoute(expected, route[0]));
        }

        [Test]
        public void TestFindKShortestPath_BlueYellowLineRoute()
        {
            var route = searchStationService.FindKShortestPath("Kaki Bukit", "Mountbatten", 3);
            var expected = new String[] { "Mountbatten", "Dakota", "Paya Lebar", "MacPherson", "Ubi", "Kaki Bukit" };
            Assert.IsTrue(TestUtility.IsSameRoute(expected, route[0]));
        }

        [Test]
        public void TestFindKShortestPath_TransitThreeLineRoute_EunosToKovan()
        {
            var route = searchStationService.FindKShortestPath("Eunos", "Kovan", 3);
            var expected = new String[] { "Kovan", "Serangoon", "Bartley", "Tai Seng", "MacPherson", "Paya Lebar", "Eunos" };
            Assert.IsTrue(TestUtility.IsSameRoute(expected, route[0]));
        }

        [Test]
        public void TestFindKShortestPath_TransitThreeLineRoute_MarinaBayToFarrerRoad()
        {
            var route = searchStationService.FindKShortestPath("Marina Bay", "Farrer Road", 3);
            var expected = new String[]{"Farrer Road",  "Botanic Gardens",  "Stevens", "Newton", "Little India",
                                        "Dhoby Ghaut", "City Hall", "Raffles Place", "Marina Bay"};
            TestUtility.PrintRoute(route[0]);
            TestUtility.PrintRoute(route[1]);
            TestUtility.PrintRoute(route[2]);
            Assert.IsTrue(TestUtility.IsSameRoute(expected, route[0]));
        }

        [Test]
        public void TestFindKShortestPath_TransitThreeLineRoute_MarinaBayToNewton()
        {
            var route = searchStationService.FindKShortestPath("Marina Bay", "Newton", 3);
            var expected = new String[] { "Newton", "Little India", "Dhoby Ghaut", "City Hall", "Raffles Place", "Marina Bay" };

            TestUtility.PrintRoute(route[0]);
            TestUtility.PrintRoute(route[1]);
            TestUtility.PrintRoute(route[2]);
            Assert.IsTrue(TestUtility.IsSameRoute(expected, route[0]));
        }

        [Test]
        public void TestFindKShortestPath_TransitThreeLineRoute_MarymountToDhobyGhaut()
        {
            var route = searchStationService.FindKShortestPath("Marymount", "Dhoby Ghaut", 3);
            var expected = new String[] { "Dhoby Ghaut", "Little India", "Newton", "Stevens", "Mount Pleasant", "Caldecott", "Marymount" };
            TestUtility.PrintRoute(route[0]);
            TestUtility.PrintRoute(route[1]);
            TestUtility.PrintRoute(route[2]);
            Assert.IsTrue(TestUtility.IsSameRoute(expected, route[0]));
        }
        #endregion
    }
}
