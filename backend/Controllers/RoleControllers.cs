using Microsoft.AspNetCore.Mvc;
using Student_management.DTOs.Permission;
using Student_management.DTOs.Role;
using Student_management.Services;

namespace Student_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(
            IPermissionService permissionService,
            IRoleService roleService,
            ILogger<RoleController> logger)
        {
            _permissionService = permissionService;
            _roleService = roleService;
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

        [HttpGet]
        public async Task<ActionResult<List<RoleDto>>> GetAll()
        {
            try
            {
                var roles = await _roleService.GetAll();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
