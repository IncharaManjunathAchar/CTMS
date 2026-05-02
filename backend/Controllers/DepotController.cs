using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/depots")]
[Authorize]
public class DepotController : ControllerBase
{
    private readonly AppDbContext _db;
    public DepotController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> AddDepot(DepotDto dto)
    {
        var depot = new Depot { DepotName = dto.DepotName, Location = dto.Location, MaxCapacity = dto.MaxCapacity };
        _db.Depots.Add(depot);
        await _db.SaveChangesAsync();
        return Ok(new { depot.Id, depot.DepotName, depot.Location });
    }

    [HttpPost("{depotId}/assign-bus")]
    public async Task<IActionResult> AssignBusToDepot(int depotId, AssignBusToDepotDto dto)
    {
        if (!await _db.Depots.AnyAsync(d => d.Id == depotId)) return NotFound("Depot not found.");
        var bus = await _db.Buses.FindAsync(dto.BusId);
        if (bus == null) return NotFound("Bus not found.");
        bus.DepotId = depotId;
        await _db.SaveChangesAsync();
        return Ok("Bus assigned to depot.");
    }

    [HttpPost("{depotId}/assign-route")]
    public async Task<IActionResult> AssignBusToRoute(int depotId, AssignBusToRouteDto dto)
    {
        if (!await _db.Depots.AnyAsync(d => d.Id == depotId)) return NotFound("Depot not found.");
        if (!await _db.Buses.AnyAsync(b => b.Id == dto.BusId && b.DepotId == depotId)) return BadRequest("Bus not in this depot.");
        if (!await _db.Routes.AnyAsync(r => r.Id == dto.RouteId)) return NotFound("Route not found.");

        // conflict check — same bus, overlapping active assignment
        if (await _db.BusAssignments.AnyAsync(a => a.BusId == dto.BusId && a.RouteId == dto.RouteId && a.IsActive))
            return BadRequest("Bus already assigned to this route.");

        _db.BusAssignments.Add(new BusAssignment
        {
            DepotId = depotId, BusId = dto.BusId, RouteId = dto.RouteId, ValidUntil = dto.ValidUntil
        });
        await _db.SaveChangesAsync();
        return Ok("Bus assigned to route.");
    }

    [HttpPut("{depotId}/reassign-bus")]
    public async Task<IActionResult> ReassignBus(int depotId, ReassignBusDto dto)
    {
        var existing = await _db.BusAssignments.FirstOrDefaultAsync(a => a.DepotId == depotId && a.RouteId == dto.RouteId && a.IsActive);
        if (existing == null) return NotFound("No active assignment found.");
        existing.IsActive = false;

        _db.BusAssignments.Add(new BusAssignment { DepotId = depotId, BusId = dto.NewBusId, RouteId = dto.RouteId });
        await _db.SaveChangesAsync();
        return Ok("Bus reassigned.");
    }

    [HttpGet("{depotId}/dashboard")]
    public async Task<IActionResult> GetDepotDashboard(int depotId)
    {
        var depot = await _db.Depots.Include(d => d.Buses).FirstOrDefaultAsync(d => d.Id == depotId);
        if (depot == null) return NotFound();

        var totalBuses = depot.Buses.Count;
        var assignments = await _db.BusAssignments.Where(a => a.DepotId == depotId && a.IsActive).CountAsync();
        var statuses = await _db.BusStatuses
            .Where(s => _db.Buses.Where(b => b.DepotId == depotId).Select(b => b.Id).Contains(s.BusId))
            .GroupBy(s => s.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        return Ok(new { depot.DepotName, depot.Location, TotalBuses = totalBuses, ActiveAssignments = assignments, StatusBreakdown = statuses });
    }
}
