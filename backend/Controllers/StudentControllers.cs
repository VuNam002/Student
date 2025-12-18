using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Student;
using Student_management.Enum;
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
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStudentStatus(int id, [FromBody] byte status)
        {
            try
            {
                var updateStatusStudent = await _studentService.UpdateStudentStatus(id, (StudentStatus)status);
                return Ok(updateStatusStudent);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating student status");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> Detail(int id)
        {
            try
            {
                var student = await _studentService.Detail(id);
                if (student == null)
                {
                    return NotFound("Student not found");
                }
                return Ok(student);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting student by ID.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("by-class/{classId}")]
        public async Task<ActionResult<StudentListResponse>> GetStudentsByClass(int classId)
        {
            try
            {
                var result = await _studentService.GetStudentsByClass(classId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting students by class.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost("classes/{classId}/students")]
        public async Task<IActionResult> AddStudentsToClass(int classId, [FromBody] List<int> studentIds)
        {
            var result = await _studentService.AddStudentsToClass(classId, studentIds);
            return Ok(result);
        }

        [HttpDelete("classes/{classId}/students/{studentId}")]
        public async Task<IActionResult> RemoveStudentFromClass(int classId, int studentId)
        {
            var result = await _studentService.RemoveStudentFromClass(classId, studentId);
            return Ok(result);
        }
        [HttpPut("{studentId}/transfer-class/{newClassId}")]
        public async Task<IActionResult> TransferStudentToClass(int studentId, int newClassId)
        {
            try
            {
                var result = await _studentService.TransferStudentToClass(studentId, newClassId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi chuyển lớp" });
            }
        }
    }
}
