using MRT.Models; 
using System;
namespace MRT_UnitTest.Services
{
    public static class TestUtility
    {
        public static bool IsSameRoute(string[] expected, Route given) {
            var current = given.LastStation;
            var isValid = true;
            foreach(var stationName in expected) {
                if(current == null || stationName != current.Station.StationName) {
                    return false;
                }  
                current = current.PrevStation;
            }

            return isValid;
        }

        public static void PrintRoute(Route route) {
            var current = route.LastStation;
            while(current != null) {
                if (current.PrevStation != null)
                {
                    Console.Write(" -> {0} ", current.Station.StationName);
                }
                else
                {
                    Console.Write("{0}", current.Station.StationName);
                }
                current = current.PrevStation;
            }

            Console.WriteLine("");
        }
    }
}