using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Permission;
using Student_management.Models;
using Student_management.Services.Interfaces; // ✅ Dùng namespace Interfaces

namespace Student_management.Controllers
{
    [ApiController]
    [Route("api/permissions")]
    //[Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService; // ✅ Dùng interface
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(
            IPermissionService permissionService,  // ✅ Dùng interface
            ILogger<PermissionController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllPermissionsGrouped([FromQuery] string? module = null)
        {
            try
            {
                var permision = await _permissionService.GetAllPermissionsGrouped(module);
                return Ok(permision);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all permissions.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermissions([FromBody] CreatePermission request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Permission data is invalid.");
                }

                var result = await _permissionService.CreatePermissions(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating permission.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermissions(int id)
        {
            try
            {
                var result = await _permissionService.DeletePermission(id);
                if (!result)
                {
                    return NotFound("Permission not found.");
                }

                return Ok(new { message = "Permission deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting permission.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}