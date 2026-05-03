using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/staff")]
[Authorize(Roles = "Admin")]
public class DriverConductorController : ControllerBase
{
    private readonly AppDbContext _db;
    public DriverConductorController(AppDbContext db) => _db = db;

    [HttpPost("drivers")]
    public async Task<IActionResult> AddDriver(DriverDto dto)
    {
        var driver = new Driver { Name = dto.Name, LicenseNumber = dto.LicenseNumber, LicenseExpiry = dto.LicenseExpiry, ContactNumber = dto.ContactNumber, EmploymentType = dto.EmploymentType };
        _db.Drivers.Add(driver);
        await _db.SaveChangesAsync();
        return Ok(new { driver.Id, driver.Name, driver.LicenseNumber });
    }

    [HttpPost("conductors")]
    public async Task<IActionResult> AddConductor(ConductorDto dto)
    {
        var conductor = new Conductor { Name = dto.Name, EmployeeNumber = dto.EmployeeNumber, ContactNumber = dto.ContactNumber, EmploymentType = dto.EmploymentType };
        _db.Conductors.Add(conductor);
        await _db.SaveChangesAsync();
        return Ok(new { conductor.Id, conductor.Name, conductor.EmployeeNumber });
    }

    [HttpPost("duty")]
    public async Task<IActionResult> AssignDuty(DutyAssignmentDto dto)
    {
        if (!await _db.Drivers.AnyAsync(d => d.Id == dto.DriverId)) return NotFound("Driver not found.");
        if (!await _db.Conductors.AnyAsync(c => c.Id == dto.ConductorId)) return NotFound("Conductor not found.");
        if (!await _db.Buses.AnyAsync(b => b.Id == dto.BusId)) return NotFound("Bus not found.");
        if (!await _db.Routes.AnyAsync(r => r.Id == dto.RouteId)) return NotFound("Route not found.");

        var duty = new DutyAssignment
        {
            DriverId = dto.DriverId, ConductorId = dto.ConductorId,
            BusId = dto.BusId, RouteId = dto.RouteId,
            Shift = dto.Shift, AssignedDate = dto.AssignedDate
        };
        _db.DutyAssignments.Add(duty);
        await _db.SaveChangesAsync();
        return Ok(new { duty.Id, duty.Shift, duty.AssignedDate });
    }

    [HttpGet("duty/{driverId}")]
    public async Task<IActionResult> GetDutySchedule(int driverId)
    {
        var duties = await _db.DutyAssignments
            .Where(d => d.DriverId == driverId)
            .Include(d => d.Bus).Include(d => d.Route).Include(d => d.Conductor)
            .OrderByDescending(d => d.AssignedDate)
            .Select(d => new { d.Id, d.Shift, d.AssignedDate, d.TripStatus, Bus = d.Bus.RegistrationNumber, Route = d.Route.RouteName, Conductor = d.Conductor.Name })
            .ToListAsync();
        return Ok(duties);
    }

    [HttpPut("duty/{dutyId}/status")]
    public async Task<IActionResult> UpdateTripStatus(int dutyId, UpdateTripStatusDto dto)
    {
        var duty = await _db.DutyAssignments.FindAsync(dutyId);
        if (duty == null) return NotFound();
        duty.TripStatus = dto.TripStatus;
        duty.DelayReason = dto.DelayReason;
        await _db.SaveChangesAsync();
        return Ok("Trip status updated.");
    }

    [HttpPost("attendance")]
    public async Task<IActionResult> CheckIn(AttendanceDto dto)
    {
        if (!await _db.DutyAssignments.AnyAsync(d => d.Id == dto.DutyAssignmentId)) return NotFound("Duty not found.");
        _db.Attendances.Add(new Attendance { StaffId = dto.StaffId, StaffType = dto.StaffType, DutyAssignmentId = dto.DutyAssignmentId, CheckIn = dto.CheckIn });
        await _db.SaveChangesAsync();
        return Ok("Attendance checked in.");
    }

    [HttpPut("attendance/{id}/checkout")]
    public async Task<IActionResult> CheckOut(int id)
    {
        var att = await _db.Attendances.FindAsync(id);
        if (att == null) return NotFound();
        att.CheckOut = DateTime.UtcNow;
        att.TotalHours = (att.CheckOut.Value - att.CheckIn).TotalHours;
        await _db.SaveChangesAsync();
        return Ok(new { att.TotalHours, att.CheckOut });
    }
}
