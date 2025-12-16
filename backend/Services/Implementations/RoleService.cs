using Microsoft.EntityFrameworkCore;
using Student_management.Data;
using Student_management.DTOs.Role;
using Student_management.Services.Interfaces;
using AutoMapper;

namespace Student_management.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RoleService> _logger;
        private readonly IMapper _mapper;

        public RoleService(AppDbContext context, ILogger<RoleService> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<RoleDto>> GetAll()
        {
            try
            {
                var roles = await _context.Roles
                    .Include(r => r.RolePermissions)
                    .Where(r => r.IsDeleted == false)
                    .ToListAsync();

                return _mapper.Map<List<RoleDto>>(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all roles.");
                throw;
            }
        }

        public async Task<RoleDto?> Detail(int id)
        {
            try
            {
                var role = await _context.Roles
                    .Where(r => r.RoleID == id && r.IsDeleted == false)
                    .FirstOrDefaultAsync();

                return _mapper.Map<RoleDto>(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving role details for RoleID: {RoleID}", id);
                throw;
            }
        }

        public async Task<RoleDto> CreateRole(CreateRole dto)
        {
            try
            {
                var isExist = await _context.Roles
                    .AnyAsync(r => r.RoleCode == dto.RoleCode && !r.IsDeleted);
                if (isExist)
                {
                    throw new InvalidOperationException($"Role with code '{dto.RoleCode}' already exists.");
                }
                //AutoMapper
                var newRole = _mapper.Map<Role>(dto);

                await _context.Roles.AddAsync(newRole);
                await _context.SaveChangesAsync();

                return _mapper.Map<RoleDto>(newRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating role with code: {RoleCode}", dto.RoleCode);
                throw;
            }
        }

        public async Task<RoleDto?> UpdateRole(int id, CreateRole dto)
        {
            try
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleID == id && !r.IsDeleted);
                if (role == null)
                {
                    return null;
                }
                var isDuplicate = await _context.Roles
                    .AnyAsync(r => r.RoleCode == dto.RoleCode && r.RoleID != id && !r.IsDeleted);

                if (isDuplicate)
                {
                    throw new InvalidOperationException($"Role with code '{dto.RoleCode}' already exists.");
                }
                // Update fields
                role.RoleCode = dto.RoleCode;
                role.RoleName = dto.RoleName;
                role.Description = dto.Description;

                await _context.SaveChangesAsync();
                return _mapper.Map<RoleDto>(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating role with ID: {RoleID}", id);
                throw;
            }
        }

        public async Task<bool> DeleteRole(int id)
        {
            try
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleID == id && !r.IsDeleted);

                if (role == null)
                {
                    return false;
                }
                role.IsDeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting role with ID: {RoleID}", id);
                throw;
            }
        }
    }
}