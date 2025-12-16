using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Class;
using Student_management.Services.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

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

            // ← SET LICENSE CHO EPPLUS 7.5.0
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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
                if (classSutdent == null)
                {
                    return NotFound("Class not found");
                }
                return Ok(classSutdent);
            }
            catch (Exception ex)
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
                if (dto == null)
                {
                    return BadRequest("Class data is invalid");
                }
                var result = await _classService.CreateClass(dto);
                return Ok(result);
            }
            catch (Exception ex)
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
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "Invalid class ID" });
                }
                var result = await _classService.DeleteClass(id);
                if (!result)
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

        [HttpGet("{id}/export-students")]
        public async Task<IActionResult> ExportStudents(int id)
        {
            try
            {
                var students = await _classService.GetStudentsByClassId(id);

                if (students == null || !students.Any())
                {
                    return NotFound("Lớp học này chưa có sinh viên nào.");
                }

                var stream = new MemoryStream();

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Danh Sách Sinh Viên");

                    worksheet.Cells[1, 1].Value = "Mã SV";
                    worksheet.Cells[1, 2].Value = "Họ và Tên";
                    worksheet.Cells[1, 3].Value = "Email";
                    worksheet.Cells[1, 4].Value = "Số điện thoại";
                    worksheet.Cells[1, 5].Value = "Ngày sinh";
                    worksheet.Cells[1, 6].Value = "Địa chỉ";

                    using (var range = worksheet.Cells[1, 1, 1, 6])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    // --- Data ---
                    int row = 2;
                    foreach (var st in students)
                    {
                        worksheet.Cells[row, 1].Value = st.StudentID;
                        worksheet.Cells[row, 2].Value = st.Person?.FullName;
                        worksheet.Cells[row, 3].Value = st.Person?.Email;
                        worksheet.Cells[row, 4].Value = st.Person?.PhoneNumber;

                        worksheet.Cells[row, 5].Value = st.Person?.DateOfBirth;
                        worksheet.Cells[row, 5].Style.Numberformat.Format = "dd/MM/yyyy";

                        worksheet.Cells[row, 6].Value = st.Person?.Address;
                        row++;
                    }

                    worksheet.Cells.AutoFitColumns();

                    await package.SaveAsAsync(stream);
                }
                stream.Position = 0;

                string fileName = $"DanhSachSV_Lop_{id}_{DateTime.Now:yyyyMMdd}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while exporting students for class ID {ClassId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}