using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MizanGraduationProject.Data;
using MizanGraduationProject.Data.Classes;
using System.Linq;

namespace MizanGraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {

        private AppDbContext _AppDbContext;
        public LocationsController(AppDbContext AppDbContext)
        {
            _AppDbContext = AppDbContext;
        }

        [HttpPost("fill-locations")]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> FillLocationsAsync()
        {
            try
            {
                if(await _AppDbContext.Locations.CountAsync() > 0)
                {
                    return StatusCode(403, new
                    {
                        Message = "Locations already exists",
                        Success = false,
                        StatusCode = 403
                    });
                }
                foreach (var location in Locations.locations)
                {
                    await _AppDbContext.Locations.AddAsync(new Data.Models.Location
                    {
                        Name = location,
                        NormalizedName = location.ToUpper(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                    await _AppDbContext.SaveChangesAsync();
                }
                return StatusCode(201, new
                {
                    Message = "Locations added successfully",
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

        [HttpGet("get-locations")]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetLocationsAsync()
        {
            try
            {
                if (await _AppDbContext.Locations.CountAsync() > 0)
                {
                    var locations = await _AppDbContext.Locations
                    .Select(x => new
                    {
                        x.NormalizedName
                    })
                    .ToListAsync();
                    return StatusCode(200, new
                    {
                        Message = "Locations found successfully",
                        Success = true,
                        StatusCode = 200,
                        locations = locations
                    });
                }
                return StatusCode(404, new
                {
                    Message = "Locations not found",
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
