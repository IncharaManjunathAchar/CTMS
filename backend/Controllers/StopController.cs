using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/stops")]
public class StopController : ControllerBase
{
    private readonly AppDbContext _db;
    public StopController(AppDbContext db) => _db = db;

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddStop(StopDto dto)
    {
        var stop = new Stop
        {
            StopName = dto.StopName,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Landmark = dto.Landmark,
            Zone = dto.Zone
        };

        _db.Stops.Add(stop);
        await _db.SaveChangesAsync();
        return Ok(new { stop.Id, stop.StopName, stop.Latitude, stop.Longitude });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{routeId}/stops")]
    public async Task<IActionResult> LinkStopToRoute(int routeId, LinkStopDto dto)
    {
        if (!await _db.Routes.AnyAsync(r => r.Id == routeId))
            return NotFound("Route not found.");

        if (!await _db.Stops.AnyAsync(s => s.Id == dto.StopId))
            return NotFound("Stop not found.");

        if (await _db.RouteStopMappings.AnyAsync(m => m.RouteId == routeId && m.StopId == dto.StopId))
            return BadRequest("Stop already linked to this route.");

        if (await _db.RouteStopMappings.AnyAsync(m => m.RouteId == routeId && m.StopSequence == dto.StopSequence))
            return BadRequest("A stop with this sequence already exists on the route.");

        var mapping = new RouteStopMapping
        {
            RouteId = routeId,
            StopId = dto.StopId,
            StopSequence = dto.StopSequence,
            EstimatedMinutesFromPrevious = dto.EstimatedMinutesFromPrevious
        };

        _db.RouteStopMappings.Add(mapping);
        await _db.SaveChangesAsync();
        return Ok("Stop linked to route successfully.");
    }

    [HttpGet("{routeId}/stops")]
    public async Task<IActionResult> GetStopsByRoute(int routeId)
    {
        if (!await _db.Routes.AnyAsync(r => r.Id == routeId))
            return NotFound("Route not found.");

        var stops = await _db.RouteStopMappings
            .Where(m => m.RouteId == routeId)
            .OrderBy(m => m.StopSequence)
            .Select(m => new StopResponseDto(
                m.Stop.Id, m.Stop.StopName, m.Stop.Latitude, m.Stop.Longitude,
                m.Stop.Landmark, m.Stop.Zone, m.StopSequence, m.EstimatedMinutesFromPrevious))
            .ToListAsync();

        return Ok(stops);
    }
}
