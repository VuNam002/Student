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
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> Detail(int id)
        {
            try
            {
                var role = await _roleService.Detail(id);
                if (role == null)
                {
                    return NotFound("Role not found.");
                }
                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role by ID {id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRole dto)
        {
            try
            {
                var role = await _roleService.CreateRole(dto);
                return CreatedAtAction(nameof(Detail), new { id = role.RoleID }, role);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<RoleDto>> Update(int id, [FromBody] CreateRole dto)
        {
            try
            {
                var role = await _roleService.UpdateRole(id, dto);
                if (role == null)
                {
                    return NotFound("Role not found.");
                }
                return Ok(role);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _roleService.DeleteRole(id);
                if (!result)
                {
                    return NotFound("Role not found.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {id}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
