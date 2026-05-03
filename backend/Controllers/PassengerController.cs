using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/passenger")]
[Authorize(Roles = "Passenger")]
public class PassengerController : ControllerBase
{
    private readonly AppDbContext _db;
    public PassengerController(AppDbContext db) => _db = db;

    private int GetPassengerId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("saved-routes")]
    public async Task<IActionResult> SaveRoute(SaveRouteDto dto)
    {
        var id = GetPassengerId();
        if (await _db.SavedRoutes.AnyAsync(s => s.PassengerId == id && s.RouteId == dto.RouteId))
            return BadRequest("Route already saved.");
        if (!await _db.Routes.AnyAsync(r => r.Id == dto.RouteId))
            return NotFound("Route not found.");

        _db.SavedRoutes.Add(new SavedRoute { PassengerId = id, RouteId = dto.RouteId });
        await _db.SaveChangesAsync();
        return Ok("Route saved.");
    }

    [HttpGet("saved-routes")]
    public async Task<IActionResult> GetSavedRoutes()
    {
        var id = GetPassengerId();
        var routes = await _db.SavedRoutes
            .Where(s => s.PassengerId == id)
            .Include(s => s.Route)
            .OrderByDescending(s => s.LastUsedAt)
            .Select(s => new { s.Id, s.RouteId, s.Route.RouteName, s.Route.Source, s.Route.Destination, s.UsageFrequency, s.LastUsedAt })
            .ToListAsync();
        return Ok(routes);
    }

    [HttpPost("travel-history")]
    public async Task<IActionResult> AddTravelHistory(TravelHistoryDto dto)
    {
        var id = GetPassengerId();
        if (!await _db.Routes.AnyAsync(r => r.Id == dto.RouteId))
            return NotFound("Route not found.");

        _db.TravelHistories.Add(new TravelHistory { PassengerId = id, RouteId = dto.RouteId, PassOrTicketUsed = dto.PassOrTicketUsed });
        await _db.SaveChangesAsync();
        return Ok("Travel history recorded.");
    }

    [HttpGet("travel-history")]
    public async Task<IActionResult> GetTravelHistory()
    {
        var id = GetPassengerId();
        var history = await _db.TravelHistories
            .Where(t => t.PassengerId == id)
            .Include(t => t.Route)
            .OrderByDescending(t => t.TravelDate)
            .Select(t => new { t.Id, t.RouteId, t.Route.RouteName, t.PassOrTicketUsed, t.TravelDate })
            .ToListAsync();
        return Ok(history);
    }

    [HttpPost("feedback")]
    public async Task<IActionResult> SubmitFeedback(FeedbackDto dto)
    {
        var id = GetPassengerId();
        _db.Feedbacks.Add(new Feedback { PassengerId = id, Category = dto.Category, Description = dto.Description });
        await _db.SaveChangesAsync();
        return Ok("Feedback submitted.");
    }

    [HttpPost("sos")]
    public async Task<IActionResult> TriggerSos(SosAlertDto dto)
    {
        var id = GetPassengerId();
        var sos = new SosAlert { PassengerId = id, Latitude = dto.Latitude, Longitude = dto.Longitude, BusId = dto.BusId };
        _db.SosAlerts.Add(sos);
        await _db.SaveChangesAsync();
        return Ok(new { sos.Id, sos.AlertStatus, sos.TriggeredAt, message = "SOS alert triggered. Authorities notified." });
    }
}
