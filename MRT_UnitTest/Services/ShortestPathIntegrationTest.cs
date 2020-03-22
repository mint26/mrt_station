using NUnit.Framework;
using MRT.Services;
using System;
namespace MRT_UnitTest.Services
{
    [TestFixture]
    public class ShortestPathIntegrationTest
    {
        private SearchStationService _searchStationService;
        [SetUp]
        public void Setup()
        {
            _searchStationService = new SearchStationService();
            _searchStationService.ImportDataset("Datasets/StationMap.csv",new DateTime(2022,1,1));
        }

        [Test]
        public void TestSameLineRoute()
        {
            var greenRoute = _searchStationService.FindKShortestPath("Joo Koon", "Jurong East", 3);
            var greenLine = new String[]{"Jurong East",  "Chinese Garden", "Lakeside", "Boon Lay", "Pioneer", "Joo Koon"};
            Assert.IsTrue(TestUtility.IsSameRoute(greenLine, greenRoute[0]));

            var redRoute = _searchStationService.FindKShortestPath("Toa Payoh", "Yishun", 3);
            var redLine = new String[]{"Yishun",  "Khatib", "Yio Chu Kang", "Ang Mo Kio", "Bishan", "Braddell", "Toa Payoh"};
            Assert.IsTrue(TestUtility.IsSameRoute(redLine, redRoute[0]));

            var purpleRoute = _searchStationService.FindKShortestPath("Sengkang", "Hougang", 3);
            var purpleLine = new String[]{"Hougang",  "Buangkok", "Sengkang"};
            Assert.IsTrue(TestUtility.IsSameRoute(purpleLine, purpleRoute[0]));
        }

        [Test]
        public void TestGreenRedLineRoute()
        {
            var route = _searchStationService.FindKShortestPath("Lakeside", "Yew Tee", 3);
            var expected = new String[]{"Yew Tee",  "Choa Chu Kang", "Bukit Gombak", "Bukit Batok", "Jurong East", "Chinese Garden", "Lakeside"};
            Assert.IsTrue(TestUtility.IsSameRoute(expected, route[0]));
        }

        [Test]
        public void TestPurpleGreenLineRoute()
        {
            var route = _searchStationService.FindKShortestPath("Chinatown", "Commonwealth", 3);
            var expected = new String[]{"Commonwealth",  "Queenstown", "Redhill", "Tiong Bahru", "Outram Park", "Chinatown"};
            Assert.IsTrue(TestUtility.IsSameRoute(expected, route[0]));
        }

        [Test]
        public void TestBlueYellowLineRoute()
        {
            var route = _searchStationService.FindKShortestPath("Kaki Bukit", "Mountbatten", 3);
            var expected = new String[]{"Mountbatten",  "Dakota", "Paya Lebar", "MacPherson", "Ubi", "Kaki Bukit"};
            Assert.IsTrue(TestUtility.IsSameRoute(expected, route[0]));
        }

        [Test]
        public void TestThreeLineRoute()
        {
            var route = _searchStationService.FindKShortestPath("Eunos", "Kovan", 3);
            var expected = new String[]{"Kovan",  "Serangoon",  "Bartley", "Tai Seng", "MacPherson", "Paya Lebar", "Eunos"};
            Assert.IsTrue(TestUtility.IsSameRoute(expected, route[0]));
        }

        [Test]
        public void TestThreeLineRoute2()
        {
            var route = _searchStationService.FindKShortestPath("Marina Bay", "Farrer Road", 3);
            var expected = new String[]{"Farrer Road",  "Botanic Gardens",  "Stevens", "Newton", "Little India", 
                                        "Dhoby Ghaut", "City Hall", "Raffles Place", "Marina Bay"};
            Console.WriteLine("----------------------------------------");
            TestUtility.PrintRoute(route[0]);
            Console.WriteLine("----------------------------------------");
            TestUtility.PrintRoute(route[1]);
            Console.WriteLine("----------------------------------------");
            TestUtility.PrintRoute(route[2]);
            Assert.IsTrue(TestUtility.IsSameRoute(expected, route[0]));
        }

        [Test]
        public void TestThreeLineRoute3()
        {
            var route = _searchStationService.FindKShortestPath("Marina Bay", "Newton", 3);
            var expected = new String[]{"Newton", "Little India", "Dhoby Ghaut", "City Hall", "Raffles Place", "Marina Bay"};
            Console.WriteLine("----------------------------------------");
            TestUtility.PrintRoute(route[0]);
            Console.WriteLine("----------------------------------------");
            TestUtility.PrintRoute(route[1]);
            Console.WriteLine("----------------------------------------");
            TestUtility.PrintRoute(route[2]);
            Assert.IsTrue(TestUtility.IsSameRoute(expected, route[0]));

            // var instructions = _searchStationService.FormatRouteToRouteDTO(route[0]);

        }

        [Test]
        public void TestThreeLineRoute4()
        {
            var route = _searchStationService.FindKShortestPath("Marymount", "Dhoby Ghaut", 3);
            var expected = new String[]{"Dhoby Ghaut", "Little India", "Newton", "Stevens", "Botanic Gardens", "Caldecott", "Marymount"};
            TestUtility.PrintRoute(route[0]);
            Assert.IsTrue(TestUtility.IsSameRoute(expected, route[0]));
        }

        [Test]
        public void TestThreeLineRoute5()
        {
            var route = _searchStationService.FindKShortestPath("Tanjong Pagar", "Promenade", 3);
            TestUtility.PrintRoute(route[0]);

            // var instructions = _searchStationService.FormatRouteToRouteDTO(route[0]);
        }
    }
}