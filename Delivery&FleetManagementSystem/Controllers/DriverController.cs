using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DriverServices;
using System.Security.Claims;
using SystemContext.SystemDbContext;
using SystemDTOS.DriverDTOS;
using SystemModel.Entities;
namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly DriverService _driverService;
        public DriverController(DriverService driverService)
        {
            _driverService = driverService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult GetDrivers()
        {
            var Drivers = _driverService.GetDrivers();

            return Ok(Drivers);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("BusyDrivers")]
        public ActionResult GetBusyDrivers()
        {
            var Drivers = _driverService.GetBusyDrivers();

            return Ok(Drivers);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ActiveDrivers")]
        public ActionResult GetActiveDrivers()
        {
            var Drivers = _driverService.GetActiveDrivers();

            return Ok(Drivers);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("NotActiveDrivers")]
        public ActionResult GetNotActiveDrivers()
        {
            var Drivers = _driverService.GetNotActiveDrivers();

            return Ok(Drivers);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{DriverID}")]
        public ActionResult GetDriverByID([FromRoute] int DriverID)
        {
            var Driver = _driverService.GetDriverByID(DriverID);

            return Ok(Driver);
        }

        [Authorize(Roles = "Admin,Driver")]
        [HttpPost]
        public ActionResult CreateDriver([FromBody] CreateDriverDTO dto)
        {
            var UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var UserRole = User.FindFirst(ClaimTypes.Role).Value;
            if(UserRole == "Driver")
            {
                if (UserID != dto.UserID)
                {
                    throw new Exception("You Can Not Make Driver Account For Another User");
                }
            }
            var NewDriver = _driverService.CreateDriver(dto);

            return Ok(NewDriver);
        }
       
        [Authorize(Roles = "Admin")]
        [HttpPut("{DriverID}/Update")]
        public ActionResult UpdateDriver([FromBody] UpdateDriverDTO dto,[FromRoute]int DriverID)
        {
       
            var UpdatedDriver = _driverService.UpdateDriver(dto, DriverID);

            return Ok(UpdatedDriver);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{DriverID}/Activate")]
        public ActionResult ActivateDriver(int DriverID)
        {
            var Driver = _driverService.ActivateDriver(DriverID);
            
            return Ok(Driver);
        }
       
        [Authorize(Roles = "Admin")]
        [HttpPut("{DriverID}/DeActivate")]
        public ActionResult DeActivateDriver(int DriverID)
        {
            var Driver = _driverService.DeActivateDriver(DriverID);

            return Ok(Driver);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{DriverID}")]
        public ActionResult DeleteDriver(int DriverID)
        {
            var result = _driverService.DeleteDriver(DriverID);

            return Ok(result);
        }


    }
}
