using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ReportsController(AppDbContext db) => _db = db;

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenueReport([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var tickets = await _db.Tickets.Where(t => t.IssuedAt >= from && t.IssuedAt <= to).SumAsync(t => t.FareAmount);
        var passes = await _db.Passes.Where(p => p.PurchasedAt >= from && p.PurchasedAt <= to).SumAsync(p => p.AmountPaid);
        var transactions = await _db.Transactions.Where(t => t.CreatedAt >= from && t.CreatedAt <= to && t.Status == "Success").SumAsync(t => t.Amount);
        return Ok(new { Period = new { from, to }, TicketRevenue = tickets, PassRevenue = passes, TotalTransactions = transactions, TotalRevenue = tickets + passes });
    }

    [HttpGet("route-performance")]
    public async Task<IActionResult> GetRoutePerformance()
    {
        var performance = await _db.DutyAssignments
            .GroupBy(d => d.RouteId)
            .Select(g => new
            {
                RouteId = g.Key,
                TotalTrips = g.Count(),
                Completed = g.Count(d => d.TripStatus == "Completed"),
                Delayed = g.Count(d => d.TripStatus == "Delayed"),
                OnTimePercentage = g.Count() == 0 ? 0 : Math.Round((double)g.Count(d => d.TripStatus == "Completed") / g.Count() * 100, 1)
            }).ToListAsync();
        return Ok(performance);
    }

    [HttpGet("bus-utilization")]
    public async Task<IActionResult> GetBusUtilization()
    {
        var utilization = await _db.DutyAssignments
            .GroupBy(d => d.BusId)
            .Select(g => new
            {
                BusId = g.Key,
                TotalTrips = g.Count(),
                CompletedTrips = g.Count(d => d.TripStatus == "Completed"),
                LastUsed = g.Max(d => d.AssignedDate)
            }).ToListAsync();
        return Ok(utilization);
    }

    [HttpGet("passenger-statistics")]
    public async Task<IActionResult> GetPassengerStatistics()
    {
        var total = await _db.Passengers.CountAsync();
        var byType = await _db.Passengers.GroupBy(p => p.PassengerType).Select(g => new { Type = g.Key, Count = g.Count() }).ToListAsync();
        var totalSos = await _db.SosAlerts.CountAsync();
        var totalFeedback = await _db.Feedbacks.CountAsync();
        return Ok(new { TotalPassengers = total, ByType = byType, TotalSosAlerts = totalSos, TotalFeedbacks = totalFeedback });
    }
}
