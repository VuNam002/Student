using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Teacher;
using Student_management.Services.Interfaces;

namespace Student_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(ITeacherService teacherService, ILogger<TeacherController> logger)
        {
            _teacherService = teacherService;
            _logger = logger;
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<PaginationTeacher>> GetTeacherPagination([FromQuery] TeacherSearch teacherSearch)
        {
            try
            {
                if (teacherSearch.Page <= 0 || teacherSearch.PageSize <= 0)
                {
                    return BadRequest("Page and PageSize must be greater than zero.");
                }

                var result = await _teacherService.GetTeacherPagination(teacherSearch);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching paginated teacher.");
                return StatusCode(500, $"Internal Error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacher request)
        {
            try
            {
                if(request == null)
                {
                    return BadRequest("Request body cannot be null.");
                }
                var result = await _teacherService.CreateTeacher(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a teacher.");
                return StatusCode(500, $"Internal Error: {ex.Message}");
            }
        }
    }
}
