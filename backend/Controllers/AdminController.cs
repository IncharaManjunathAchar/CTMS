using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminController(AppDbContext db) => _db = db;

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _db.Passengers
            .Select(p => new { p.Id, p.Name, p.Email, p.MobileNumber, p.PassengerType, p.RegisteredAt })
            .ToListAsync();
        return Ok(users);
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _db.Passengers.FindAsync(id);
        if (user == null) return NotFound();
        _db.Passengers.Remove(user);
        _db.AuditLogs.Add(new Models.AuditLog { UserId = id, ActionType = "Delete", Module = "AS", EntityId = id.ToString() });
        await _db.SaveChangesAsync();
        return Ok("User deleted.");
    }

    [HttpGet("fare-rules")]
    public async Task<IActionResult> GetFareRules() => Ok(await _db.FareRules.ToListAsync());

    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs([FromQuery] int page = 1, [FromQuery] int size = 50)
    {
        var logs = await _db.AuditLogs
            .OrderByDescending(l => l.Timestamp)
            .Skip((page - 1) * size).Take(size)
            .ToListAsync();
        return Ok(logs);
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var totalBuses = await _db.Buses.CountAsync(b => b.IsActive);
        var totalRoutes = await _db.Routes.CountAsync(r => r.IsActive);
        var totalPassengers = await _db.Passengers.CountAsync();
        var activeIncidents = await _db.Incidents.CountAsync(i => i.Status != "Resolved");
        var todayRevenue = await _db.Tickets.Where(t => t.IssuedAt.Date == DateTime.UtcNow.Date).SumAsync(t => t.FareAmount);
        var runningBuses = await _db.BusStatuses
            .Where(s => s.Status == "Running")
            .Select(s => s.BusId).Distinct().CountAsync();

        return Ok(new
        {
            TotalActiveBuses = totalBuses,
            RunningBuses = runningBuses,
            TotalActiveRoutes = totalRoutes,
            TotalPassengers = totalPassengers,
            ActiveIncidents = activeIncidents,
            TodayRevenue = todayRevenue
        });
    }
}
