using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.LocationServices;

namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverLocationController : ControllerBase
    {
        private readonly LocationService _locationService;
        public DriverLocationController(LocationService locationService)
        {
            _locationService = locationService; 
        }
       
        [Authorize(Roles = "Admin")]
        [HttpGet("DriverLocationByID/{DriverID}")]
        public async Task<ActionResult> GetDriverLocationByIDAsync([FromRoute]int DriverID)
        {
            var DriverLocation = await _locationService.GetDriverLocationByIDAsync(DriverID);

            return Ok(DriverLocation);
        } 
        
        [Authorize(Roles = "Admin")]
        [HttpGet("AvailableDriversLocation")]
        public async Task<ActionResult> GetAvailableDriversLocationAsync()
        {
            var DriversLocation = await _locationService.GetAvailableDriversLocationAsync();

            return Ok(DriversLocation);
        } 
        
        [Authorize(Roles = "Admin")]
        [HttpGet("BusyDriversLocation")]
        public async Task<ActionResult> GetBusyDriversLocationAsync()
        {
            var DriversLocation = await _locationService.GetBusyDriversLocationAsync();

            return Ok(DriversLocation);
        }
    }
}
