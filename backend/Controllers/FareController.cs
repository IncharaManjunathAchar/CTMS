using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/fare")]
public class FareController : ControllerBase
{
    private readonly AppDbContext _db;
    public FareController(AppDbContext db) => _db = db;

    [Authorize(Roles = "Admin")]
    [HttpPost("rules")]
    public async Task<IActionResult> AddFareRule(FareRuleDto dto)
    {
        _db.FareRules.Add(new FareRule
        {
            PassengerCategory = dto.PassengerCategory, BaseFare = dto.BaseFare,
            PerKmRate = dto.PerKmRate, ConcessionPercentage = dto.ConcessionPercentage,
            ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo
        });
        await _db.SaveChangesAsync();
        return Ok("Fare rule added.");
    }

    [HttpGet("rules")]
    public async Task<IActionResult> GetFareRules() => Ok(await _db.FareRules.ToListAsync());

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateFare(FareCalculateDto dto)
    {
        var boarding = await _db.Stops.FindAsync(dto.BoardingStopId);
        var alighting = await _db.Stops.FindAsync(dto.AlightingStopId);
        if (boarding == null || alighting == null) return NotFound("Stop not found.");

        var rule = await _db.FareRules.FirstOrDefaultAsync(f => f.PassengerCategory == dto.PassengerCategory && (f.ValidTo == null || f.ValidTo >= DateTime.UtcNow));
        if (rule == null) return NotFound("No fare rule found for this category.");

        // Haversine distance approximation
        var latDiff = Math.Abs(boarding.Latitude - alighting.Latitude) * 111;
        var lngDiff = Math.Abs(boarding.Longitude - alighting.Longitude) * 111;
        var distanceKm = Math.Sqrt(latDiff * latDiff + lngDiff * lngDiff);

        var fare = rule.BaseFare + (decimal)distanceKm * rule.PerKmRate;
        fare = fare * (1 - rule.ConcessionPercentage / 100);

        return Ok(new { BoardingStop = boarding.StopName, AlightingStop = alighting.StopName, DistanceKm = Math.Round(distanceKm, 2), Fare = Math.Round(fare, 2), dto.PassengerCategory });
    }

    [Authorize(Roles = "Conductor,Admin")]
    [HttpPost("tickets")]
    public async Task<IActionResult> GenerateTicket(TicketDto dto)
    {
        if (!await _db.Buses.AnyAsync(b => b.Id == dto.BusId)) return NotFound("Bus not found.");
        if (!await _db.Stops.AnyAsync(s => s.Id == dto.BoardingStopId)) return NotFound("Boarding stop not found.");
        if (!await _db.Stops.AnyAsync(s => s.Id == dto.AlightingStopId)) return NotFound("Alighting stop not found.");

        var rule = await _db.FareRules.FirstOrDefaultAsync(f => f.PassengerCategory == dto.PassengerCategory && (f.ValidTo == null || f.ValidTo >= DateTime.UtcNow));
        if (rule == null) return NotFound("No fare rule found.");

        var boarding = await _db.Stops.FindAsync(dto.BoardingStopId);
        var alighting = await _db.Stops.FindAsync(dto.AlightingStopId);
        var latDiff = Math.Abs(boarding!.Latitude - alighting!.Latitude) * 111;
        var lngDiff = Math.Abs(boarding.Longitude - alighting.Longitude) * 111;
        var distanceKm = Math.Sqrt(latDiff * latDiff + lngDiff * lngDiff);
        var fare = (rule.BaseFare + (decimal)distanceKm * rule.PerKmRate) * (1 - rule.ConcessionPercentage / 100);

        var ticket = new Ticket
        {
            BusId = dto.BusId, ConductorId = dto.ConductorId,
            BoardingStopId = dto.BoardingStopId, AlightingStopId = dto.AlightingStopId,
            PassengerCategory = dto.PassengerCategory, FareAmount = Math.Round(fare, 2),
            PaymentMode = dto.PaymentMode
        };
        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();
        return Ok(new { ticket.Id, ticket.FareAmount, ticket.IssuedAt, ticket.PaymentMode });
    }

    [Authorize(Roles = "Passenger,Admin")]
    [HttpPost("passes")]
    public async Task<IActionResult> PurchasePass(PassDto dto)
    {
        if (!await _db.Passengers.AnyAsync(p => p.Id == dto.PassengerId)) return NotFound("Passenger not found.");
        var pass = new Pass { PassengerId = dto.PassengerId, PassType = dto.PassType, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo, AmountPaid = dto.AmountPaid };
        _db.Passes.Add(pass);
        await _db.SaveChangesAsync();
        return Ok(new { pass.Id, pass.PassType, pass.ValidFrom, pass.ValidTo });
    }

    [HttpPost("passes/validate")]
    public async Task<IActionResult> ValidatePass(ValidatePassDto dto)
    {
        var pass = await _db.Passes.FindAsync(dto.PassId);
        if (pass == null) return NotFound("Pass not found.");
        if (!pass.IsActive || pass.ValidTo < DateTime.UtcNow)
            return BadRequest("Pass is expired or inactive.");
        return Ok(new { pass.Id, pass.PassType, pass.ValidTo, IsValid = true });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("transactions")]
    public async Task<IActionResult> StoreTransaction(TransactionDto dto)
    {
        var txn = new Transaction { PassengerId = dto.PassengerId, Amount = dto.Amount, PaymentMethod = dto.PaymentMethod, GatewayReference = dto.GatewayReference, Status = "Success" };
        _db.Transactions.Add(txn);
        await _db.SaveChangesAsync();
        return Ok(new { txn.Id, txn.Amount, txn.Status, txn.CreatedAt });
    }
}
