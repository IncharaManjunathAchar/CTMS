using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/emergency")]
[Authorize]
public class EmergencyController : ControllerBase
{
    private readonly AppDbContext _db;
    public EmergencyController(AppDbContext db) => _db = db;

    [HttpPost("incidents")]
    public async Task<IActionResult> ReportIncident(IncidentDto dto)
    {
        if (!await _db.Buses.AnyAsync(b => b.Id == dto.BusId)) return NotFound("Bus not found.");
        if (!await _db.Drivers.AnyAsync(d => d.Id == dto.DriverId)) return NotFound("Driver not found.");

        var incident = new Incident
        {
            BusId = dto.BusId, DriverId = dto.DriverId, RouteId = dto.RouteId,
            IncidentType = dto.IncidentType, Description = dto.Description,
            Latitude = dto.Latitude, Longitude = dto.Longitude
        };
        _db.Incidents.Add(incident);
        await _db.SaveChangesAsync();
        return Ok(new { incident.Id, incident.IncidentType, incident.Status, incident.ReportedAt });
    }

    [HttpGet("incidents/{id}")]
    public async Task<IActionResult> GetIncident(int id)
    {
        var incident = await _db.Incidents
            .Include(i => i.Responses)
            .Include(i => i.Bus)
            .Include(i => i.Driver)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (incident == null) return NotFound();
        return Ok(incident);
    }

    [HttpPut("incidents/{id}/status")]
    public async Task<IActionResult> UpdateIncidentStatus(int id, UpdateIncidentStatusDto dto)
    {
        var incident = await _db.Incidents.FindAsync(id);
        if (incident == null) return NotFound();
        incident.Status = dto.Status;
        await _db.SaveChangesAsync();
        return Ok("Incident status updated.");
    }

    [HttpPost("incidents/{id}/respond")]
    public async Task<IActionResult> AddResponse(int id, IncidentResponseDto dto)
    {
        if (!await _db.Incidents.AnyAsync(i => i.Id == id)) return NotFound("Incident not found.");
        _db.IncidentResponses.Add(new IncidentResponse { IncidentId = id, ActionTaken = dto.ActionTaken, RespondedBy = dto.RespondedBy });
        await _db.SaveChangesAsync();
        return Ok("Response recorded.");
    }
}
