using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Class;
using Student_management.Services.Interfaces;

namespace Student_management.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class ClassController : ControllerBase
        {
            private readonly IClassService _classService;
            private readonly ILogger<ClassController> _logger;

            public ClassController(
                IClassService classService,
                ILogger<ClassController> logger)
            {
                _classService = classService;
                _logger = logger;
            }
            [HttpGet("pagination")]
            public async Task<ActionResult<PaginationClass>> GetClassPagination([FromQuery] ClassSearch classSearch)
            {
                try
                {
                    if(classSearch.Page <=0 || classSearch.PageSize <= 0)
                    {
                        return BadRequest("Page and PageSize must be greater than zero.");
                    }
                    var result = await _classService.GetClassPagination(classSearch);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while getting class pagination.");
                    return StatusCode(500, "Internal server error");
                }
            }
        }
}
