using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/routes")]
public class RouteController : ControllerBase
{
    private readonly AppDbContext _db;
    public RouteController(AppDbContext db) => _db = db;

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddRoute(RouteDto dto)
    {
        if (await _db.Routes.AnyAsync(r => r.RouteName == dto.RouteName))
            return BadRequest("Route name already exists.");

        var route = new Models.Route
        {
            RouteName = dto.RouteName,
            Source = dto.Source,
            Destination = dto.Destination,
            TotalDistance = dto.TotalDistance
        };

        _db.Routes.Add(route);
        await _db.SaveChangesAsync();
        return Ok(new RouteResponseDto(route.Id, route.RouteName, route.Source, route.Destination, route.TotalDistance, route.IsActive));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoutes()
    {
        var routes = await _db.Routes
            .Where(r => r.IsActive)
            .Select(r => new RouteResponseDto(r.Id, r.RouteName, r.Source, r.Destination, r.TotalDistance, r.IsActive))
            .ToListAsync();
        return Ok(routes);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchRoutes([FromQuery] string source, [FromQuery] string destination)
    {
        var routes = await _db.Routes
            .Where(r => r.IsActive &&
                        r.Source.ToLower().Contains(source.ToLower()) &&
                        r.Destination.ToLower().Contains(destination.ToLower()))
            .Select(r => new RouteResponseDto(r.Id, r.RouteName, r.Source, r.Destination, r.TotalDistance, r.IsActive))
            .ToListAsync();

        if (!routes.Any()) return NotFound("No routes found for the given source and destination.");
        return Ok(routes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRouteDetails(int id)
    {
        var route = await _db.Routes
            .Include(r => r.RouteStopMappings)
                .ThenInclude(m => m.Stop)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (route == null) return NotFound();

        var stops = route.RouteStopMappings
            .OrderBy(m => m.StopSequence)
            .Select(m => new StopResponseDto(
                m.Stop.Id, m.Stop.StopName, m.Stop.Latitude, m.Stop.Longitude,
                m.Stop.Landmark, m.Stop.Zone, m.StopSequence, m.EstimatedMinutesFromPrevious))
            .ToList();

        return Ok(new { route.Id, route.RouteName, route.Source, route.Destination, route.TotalDistance, Stops = stops });
    }
}
