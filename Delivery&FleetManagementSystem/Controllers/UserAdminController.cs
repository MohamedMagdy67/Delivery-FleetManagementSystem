using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.UserServices;
using SystemModel.Entities;
using SystemDTOS.UserDTOS;
namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAdminController : ControllerBase
    {
        public readonly UserService _userService;
        public UserAdminController(UserService userService)
        {
            _userService = userService;
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult GetUsers()
        {
            var users = _userService.GetUsers();
           
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{UserID}")]
        public ActionResult GetUserByID([FromRoute]int UserID)
        {
            var user = _userService.GetUserByID(UserID);
           
            return Ok(user);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("change-role")]
        public ActionResult ChangeUserRole([FromBody]ChangeRoleDTO dto)
        {

            var user = _userService.ChangeUserRole(dto);

            return Ok(user);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPut("{UserID}/activate")]
        public ActionResult ActivateUser([FromRoute] int UserID)
        {
            var user = _userService.ActivateUser(UserID);

            return Ok(user);

        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{UserID}/deactivate")]
        public ActionResult DeActivateUser([FromRoute] int UserID)
        {
            var user = _userService.DeActivateUser(UserID);

            return Ok(user);

        }
    }
}
