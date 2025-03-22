
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MizanGraduationProject.Data;
using MizanGraduationProject.Data.Classes;

namespace MizanGraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTypeController : ControllerBase
    {
        private AppDbContext _AppDbContext;
        public UserTypeController(AppDbContext AppDbContext)
        {
            _AppDbContext = AppDbContext;
        }

        [HttpPost("fill-user-types")]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> FillUserTypesAsync()
        {
            try
            {
                if (await _AppDbContext.UserTypeModels.CountAsync() > 0)
                {
                    return StatusCode(403, new
                    {
                        Message = "types already exists",
                        Success = false,
                        StatusCode = 403
                    });
                }
                foreach (var type in UserType.types)
                {
                    await _AppDbContext.UserTypeModels.AddAsync(new Data.Models.UserTypeModel
                    {
                        Name = type,
                        NormalizedName = type.ToUpper(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                    await _AppDbContext.SaveChangesAsync();
                }
                return StatusCode(201, new
                {
                    Message = "types added successfully",
                    Success = true,
                    StatusCode = 201
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = 500
                });
            }
        }

        [HttpGet("get-usertypes")]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetLocationsAsync()
        {
            try
            {
                if (await _AppDbContext.UserTypeModels.CountAsync() > 0)
                {
                    var locations = await _AppDbContext.UserTypeModels
                    .Select(x => new
                    {
                        x.NormalizedName
                    })
                    .ToListAsync();
                    return StatusCode(200, new
                    {
                        Message = "types found successfully",
                        Success = true,
                        StatusCode = 200,
                        locations = locations
                    });
                }
                return StatusCode(404, new
                {
                    Message = "types not found",
                    Success = false,
                    StatusCode = 404
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = 500
                });
            }
        }
    }
}
