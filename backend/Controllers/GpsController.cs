using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/gps")]
public class GpsController : ControllerBase
{
    private readonly AppDbContext _db;
    public GpsController(AppDbContext db) => _db = db;

    [Authorize]
    [HttpPost("location")]
    public async Task<IActionResult> UpdateLocation(BusLocationDto dto)
    {
        if (!await _db.Buses.AnyAsync(b => b.Id == dto.BusId)) return NotFound("Bus not found.");
        _db.BusLocations.Add(new BusLocation { BusId = dto.BusId, Latitude = dto.Latitude, Longitude = dto.Longitude, Speed = dto.Speed });
        await _db.SaveChangesAsync();
        return Ok("Location updated.");
    }

    [HttpGet("location/{busId}")]
    public async Task<IActionResult> GetLiveLocation(int busId)
    {
        var location = await _db.BusLocations
            .Where(l => l.BusId == busId)
            .OrderByDescending(l => l.RecordedAt)
            .FirstOrDefaultAsync();
        if (location == null) return NotFound("No location data found.");
        return Ok(new { location.BusId, location.Latitude, location.Longitude, location.Speed, location.RecordedAt });
    }

    [Authorize]
    [HttpPost("eta")]
    public async Task<IActionResult> SetEta(EtaRequestDto dto)
    {
        if (!await _db.Buses.AnyAsync(b => b.Id == dto.BusId)) return NotFound("Bus not found.");
        if (!await _db.Stops.AnyAsync(s => s.Id == dto.StopId)) return NotFound("Stop not found.");
        _db.EtaRecords.Add(new EtaRecord { BusId = dto.BusId, StopId = dto.StopId, CalculatedEta = dto.CalculatedEta });
        await _db.SaveChangesAsync();
        return Ok("ETA recorded.");
    }

    [HttpGet("eta/{busId}/{stopId}")]
    public async Task<IActionResult> GetEta(int busId, int stopId)
    {
        var eta = await _db.EtaRecords
            .Where(e => e.BusId == busId && e.StopId == stopId)
            .OrderByDescending(e => e.CreatedAt)
            .FirstOrDefaultAsync();
        if (eta == null) return NotFound("No ETA found.");
        return Ok(new { eta.BusId, eta.StopId, eta.CalculatedEta, eta.ActualArrival });
    }

    [HttpGet("delay/{busId}")]
    public async Task<IActionResult> DetectDelay(int busId)
    {
        var duty = await _db.DutyAssignments
            .Where(d => d.BusId == busId && d.TripStatus == "Started")
            .OrderByDescending(d => d.AssignedDate)
            .FirstOrDefaultAsync();
        if (duty == null) return NotFound("No active trip found.");

        var minutesElapsed = (DateTime.UtcNow - duty.AssignedDate).TotalMinutes;
        var isDelayed = minutesElapsed > 10 && duty.TripStatus != "Completed";
        return Ok(new { busId, isDelayed, minutesElapsed = Math.Round(minutesElapsed, 1), duty.TripStatus });
    }

    [HttpGet("stationary/{busId}")]
    public async Task<IActionResult> DetectStationary(int busId)
    {
        var recent = await _db.BusLocations
            .Where(l => l.BusId == busId)
            .OrderByDescending(l => l.RecordedAt)
            .Take(2)
            .ToListAsync();

        if (recent.Count < 2) return Ok(new { busId, isStationary = false });

        var timeDiff = (recent[0].RecordedAt - recent[1].RecordedAt).TotalMinutes;
        var distanceMoved = Math.Abs(recent[0].Latitude - recent[1].Latitude) + Math.Abs(recent[0].Longitude - recent[1].Longitude);
        var isStationary = distanceMoved < 0.0001 && timeDiff >= 15;

        return Ok(new { busId, isStationary, minutesSinceLastMove = Math.Round(timeDiff, 1) });
    }
}
