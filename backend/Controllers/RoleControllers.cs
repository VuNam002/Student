using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Permission;
using Student_management.Services;

namespace Student_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleControllers : ControllerBase
    {
        private readonly PermissionService _permissionService;
        private readonly ILogger<RoleControllers> _logger;

        public RoleControllers(PermissionService permissionService, ILogger<RoleControllers> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpPost("{roleId}/permissions")]
        public async Task<IActionResult> AssignPermissionsToRole(int roleId, [FromBody] AssignPermissionToRoleDto dto)
        {
            if (dto == null || dto.PermissionIds == null)
            {
                return BadRequest("Permission IDs are required.");
            }

            try
            {
                // Giả sử hàm này nằm trong PermissionService
                var result = await _permissionService.AssignPermissionToRoleDto(roleId, dto.PermissionIds);

                if (!result)
                {
                    return NotFound($"Role with ID {roleId} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permissions to role {roleId}", roleId);
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}