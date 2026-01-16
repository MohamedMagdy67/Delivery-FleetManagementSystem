using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.VehicleServices;
using System.Security.Claims;
using SystemDTOS.VehicleDTOS;
using SystemModel.Entities;

namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly VehicleService _vehicleService;
        public VehicleController(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult GetVehicles()
        {
            var Vehicles = _vehicleService.GetVehicles();

            return Ok(Vehicles);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("{VehicleID}/ByVehicleID")]
        public ActionResult GetVehicleByID([FromRoute]int VehicleID)
        {
            var Vehicle = _vehicleService.GetVehicleByID(VehicleID);

            return Ok(Vehicle);
        }

        [Authorize(Roles = "Admin,Driver")]
        [HttpGet("{DriverID}/ByDriverID")]
        public ActionResult GetVehicleByDriverID(int DriverID)
        {
            var Vehicle = _vehicleService.GetVehicleByDriverID(DriverID);

            return Ok(Vehicle);
        }
        
        [Authorize(Roles = "Admin,Driver")]
        [HttpPost]
        public ActionResult CreateVehicle([FromBody]CreateVehicleDTO dto)
        {
            int UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var NewVehicle = _vehicleService.CreateVehicle(dto,UserID);

            return Ok(NewVehicle);
        }

        [Authorize(Roles = "Admin,Driver")]
        [HttpPut("{VehicleID}")]
        public ActionResult UpdateVehicle([FromRoute]int VehicleID, [FromBody]UpdateVehicleDTO dto)
        {
            var UpdatedVehicle = _vehicleService.UpdateVehicle(dto , VehicleID);

            return Ok(UpdatedVehicle);
        }
        
        [Authorize(Roles = "Admin,Driver")]
        [HttpDelete("{VehicleID}")]
        public ActionResult DeleteVehicle([FromRoute]int VehicleID)
        {
            var result = _vehicleService.DeleteVehicle(VehicleID);

            return Ok(result);
        }


    }
}
