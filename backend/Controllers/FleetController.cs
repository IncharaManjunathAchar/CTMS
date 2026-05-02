using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/fleet")]
[Authorize]
public class FleetController : ControllerBase
{
    private readonly AppDbContext _db;
    public FleetController(AppDbContext db) => _db = db;

    [HttpPost("buses")]
    public async Task<IActionResult> AddBus(BusDto dto)
    {
        if (await _db.Buses.AnyAsync(b => b.RegistrationNumber == dto.RegistrationNumber))
            return BadRequest("Bus with this registration number already exists.");

        var bus = new Bus
        {
            RegistrationNumber = dto.RegistrationNumber,
            BusType = dto.BusType,
            SeatingCapacity = dto.SeatingCapacity,
            StandingCapacity = dto.StandingCapacity,
            OperatorName = dto.OperatorName,
            DepotId = dto.DepotId
        };
        _db.Buses.Add(bus);
        _db.BusStatuses.Add(new BusStatus { Bus = bus, Status = "Idle" });
        await _db.SaveChangesAsync();
        return Ok(new { bus.Id, bus.RegistrationNumber, bus.BusType });
    }

    [HttpPut("buses/{id}")]
    public async Task<IActionResult> UpdateBus(int id, UpdateBusDto dto)
    {
        var bus = await _db.Buses.FindAsync(id);
        if (bus == null) return NotFound();
        bus.BusType = dto.BusType;
        bus.SeatingCapacity = dto.SeatingCapacity;
        bus.StandingCapacity = dto.StandingCapacity;
        bus.OperatorName = dto.OperatorName;
        bus.DepotId = dto.DepotId;
        await _db.SaveChangesAsync();
        return Ok("Bus updated.");
    }

    [HttpPut("buses/{id}/status")]
    public async Task<IActionResult> UpdateBusStatus(int id, BusStatusDto dto)
    {
        if (!await _db.Buses.AnyAsync(b => b.Id == id)) return NotFound("Bus not found.");
        _db.BusStatuses.Add(new BusStatus { BusId = id, Status = dto.Status });
        await _db.SaveChangesAsync();
        return Ok(new { BusId = id, dto.Status, UpdatedAt = DateTime.UtcNow });
    }

    [HttpGet("buses/route/{routeId}")]
    public async Task<IActionResult> GetBusesByRoute(int routeId)
    {
        var buses = await _db.BusAssignments
            .Where(a => a.RouteId == routeId && a.IsActive)
            .Include(a => a.Bus)
            .Select(a => new { a.Bus.Id, a.Bus.RegistrationNumber, a.Bus.BusType, a.Bus.SeatingCapacity })
            .ToListAsync();
        return Ok(buses);
    }

    [HttpPost("buses/{id}/maintenance")]
    public async Task<IActionResult> AddMaintenanceRecord(int id, MaintenanceRecordDto dto)
    {
        if (!await _db.Buses.AnyAsync(b => b.Id == id)) return NotFound("Bus not found.");
        _db.MaintenanceRecords.Add(new MaintenanceRecord
        {
            BusId = id,
            ServiceDate = dto.ServiceDate,
            IssueDescription = dto.IssueDescription,
            PartsReplaced = dto.PartsReplaced,
            Cost = dto.Cost,
            NextDueDate = dto.NextDueDate
        });
        await _db.SaveChangesAsync();
        return Ok("Maintenance record added.");
    }

    [HttpPost("buses/{id}/fuel")]
    public async Task<IActionResult> AddFuelRecord(int id, FuelRecordDto dto)
    {
        if (!await _db.Buses.AnyAsync(b => b.Id == id)) return NotFound("Bus not found.");
        _db.FuelRecords.Add(new FuelRecord { BusId = id, FuelFilled = dto.FuelFilled, Cost = dto.Cost, Location = dto.Location });
        await _db.SaveChangesAsync();
        return Ok("Fuel record added.");
    }

    [HttpGet("buses/{id}")]
    public async Task<IActionResult> GetBus(int id)
    {
        var bus = await _db.Buses
            .Include(b => b.BusStatuses.OrderByDescending(s => s.UpdatedAt).Take(1))
            .Include(b => b.Depot)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (bus == null) return NotFound();
        return Ok(bus);
    }
}
