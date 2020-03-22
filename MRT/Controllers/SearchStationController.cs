using System;
using System.Collections.Generic;
using MRT.Models.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MRT.Services;
using MRT.Constants; 
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
            List<RouteDTO> routes = await SearchStationService.GetRoutesAsync(request.SourceStationCode, request.DestStationCode, request.AtDate);

            SearchRoutesResponse response = new SearchRoutesResponse
            {
                Success = routes != null && routes.Count > 0,
                Routes = routes,
                ErrorMessage = routes != null && routes.Count > 0 ? null : Consts.NO_ROUTE
            };
            return response; 
        }
    }

    #region searchRequest and searchResponse
    public class SearchRoutesRequest
    {
        public string SourceStationCode { get; set; }
        public string DestStationCode { get; set; }
        public DateTime AtDate { get; set; }
    }

    public class SearchRoutesResponse
    {
        public bool Success { get; set; }
        public List<RouteDTO> Routes { get; set; }
        public string ErrorMessage { get; set; }
    }
    #endregion
}
