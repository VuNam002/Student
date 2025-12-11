using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Student;
using Student_management.Services.Interfaces;

namespace Student_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentController> _logger;

        public StudentController(IStudentService studentService, ILogger<StudentController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<PaginationStudent>> GetStudentPagination([FromQuery] StudentSearch studentSearch)
        {
            try
            {
                if (studentSearch.Page <= 0 || studentSearch.PageSize <= 0)
                {
                    return BadRequest("Page and PageSize must be greater than zero.");
                }

                var result = await _studentService.GetStudentPagination(studentSearch);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching paginated students.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] CreateStudent request)
        {
            try
            {
                if(request == null)
                {
                    return BadRequest("Student data is invalid");
                }
                var result = await _studentService.CreateStudent(request);
                return Ok(result);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating student");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPatch("{id}")]
        public async Task<ActionResult<StudentDto>> EditStudent(int id, [FromBody] CreateStudent dto)
        {
            try
            {
                var student = await _studentService.EditStudent(id, dto);
                if(student == null)
                {
                    return NotFound("Student not found");
                }
                return Ok(student);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student {id}", id);
                return StatusCode(500, "Inter server error");
            }
        }
    }
}
