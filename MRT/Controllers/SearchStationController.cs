using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MRT.Services; 
namespace MRT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchStationController : ControllerBase
    {
        private ISearchStationService SearchStationService;

        public SearchStationController(ISearchStationService searchStationService)
        {
            SearchStationService = searchStationService;
        }

        // POST api/
        [HttpPost]
        public async Task<ActionResult<SearchRoutesResponse>> PostSearchStation(SearchRoutesRequest request)
        {
            IList<RouteDTO> routes = await Task.Run(() =>
            {
                return this.SearchStationService.GetRoutes(request.SourceStationCode, request.DestStationCode, request.AtDate);
            });

            SearchRoutesResponse response = new SearchRoutesResponse
            {
                Success = routes != null && routes.Count > 0,
                Routes = routes,
                ErrorMessage = routes != null && routes.Count > 0 ? "" : "No possible route at this moment"
            };
            return response; 
        }
    }

    public class SearchRoutesRequest {
        public string SourceStationCode { get; set; }
        public string DestStationCode { get; set; }
        public DateTime AtDate { get; set; }
    }
    public class SearchRoutesResponse
    {
        public bool Success { get; set; }
        public IList<RouteDTO> Routes{ get;set; }
        public string ErrorMessage { get; set; }
    }
}
