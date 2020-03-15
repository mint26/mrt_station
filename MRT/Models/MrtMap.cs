using System;
using System.Collections.Generic;
namespace MRT.Models
{
    public class MrtMap
    {
        private Dictionary<string, IList<StationEdge>> Map { get; set; }
        public MrtMap()
        {
            this.Map = new Dictionary<string, IList<StationEdge>>();
        }
    }
}
