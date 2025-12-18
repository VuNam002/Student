using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Department;
using Student_management.Services.Implementations;
using Student_management.Services.Interfaces;

namespace Student_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDeapartmentService _departmentService;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(IDeapartmentService departmentService,ILogger<DepartmentController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<PaginationDepartment>> GetDepartmentPagination(
            [FromQuery] DepartmentSearch departmentSearch)
        {
            if (departmentSearch.Page <= 0 || departmentSearch.PageSize <= 0)
            {
                return BadRequest("Page and PageSize must be greater than zero.");
            }

            var result = await _departmentService.GetDepartmentPagination(departmentSearch);
            return Ok(result);
        }
    }
}
