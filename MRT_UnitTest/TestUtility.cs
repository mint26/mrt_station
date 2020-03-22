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

        public static void PrintRoute(Route given) {
            var current = given.LastStation;
            while(current != null) {
                Console.Write(" -> {0}: {1} ",current.Station.StationCode, current.Station.StationName);
                current = current.PrevStation;
            }

            Console.WriteLine("");
        }
    }
}