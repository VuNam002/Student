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
                if (classSearch.Page <= 0 || classSearch.PageSize <= 0)
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
        [HttpGet("{id}")]
        public async Task<ActionResult<ClassDto>> GetClassById(int id)
        {
            try
            {
                var classSutdent = await _classService.GetClassById(id);
                if(classSutdent == null)
                {
                    return NotFound("Class not found");
                }
                return Ok(classSutdent);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting class by ID.");
                return StatusCode(500, "Internal service error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateClass([FromBody] CreateClass dto)
        {
            try
            {
                if(dto == null)
                {
                    return BadRequest("Class data is invalid");
                }
                var result = await _classService.CreateClass(dto);
                return Ok(result);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while creating class");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult<ClassDto>> EditClass(int id, [FromBody] CreateClass dto)
        {
            try
            {
                var classStudent = await _classService.EditClass(id, dto);
                if (classStudent == null)
                {
                    return NotFound("Class not found");
                }
                return Ok(classStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing class.");
                return StatusCode(500, "Internal service error");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            try
            {
                if(id <=0)
                {
                    return BadRequest(new {success = false, message = "Invalid class ID" });
                }
                var result = await _classService.DeleteClass(id);
                if(!result)
                {
                    return NotFound(new { success = false, message = "Class not found" });
                }
                return Ok(new { success = true, message = "Class deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting class.");
                return StatusCode(500, "Internal service error");
            }
        }
    } 
}
