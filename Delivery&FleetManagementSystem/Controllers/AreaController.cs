using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.AreaServices;
using SystemDTOS.AreaDTOS;

namespace Delivery_FleetManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        public readonly AreaService _areaService;
        public AreaController(AreaService areaService)
        {
            _areaService = areaService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateArea([FromBody]CreateAreaDTO dto)
        {
            var NewArea = _areaService.CreateArea(dto);

            return Ok(NewArea);
        }
        
        [Authorize(Roles = "RestaurantOwner,Admin")]
        [HttpGet]
        public ActionResult GetAreas()
        {
            var Areas = _areaService.GetAreas();

            return Ok(Areas);
        }

        [Authorize(Roles = "RestaurantOwner,Admin")]
        [HttpGet("{AreaID}")]
        public ActionResult GetAreaByID([FromRoute] int AreaID)
        {
            var Area = _areaService.GetAreaByID(AreaID);

            return Ok(Area);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{AreaID}")]
        public ActionResult UpdateArea([FromBody] UpdateAreaDTO dto,[FromRoute] int AreaID)
        {
            var UpdatedArea = _areaService.UpdateArea(AreaID, dto);

            return Ok(UpdatedArea);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{AreaID}")]
        public ActionResult DeleteArea([FromRoute]int AreaID)
        {
            var result = _areaService.DeleteArea(AreaID);
            
            return Ok(result);
        }
    }
}
