namespace backend.Models;

public class Driver
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime LicenseExpiry { get; set; }
    public string ContactNumber { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = "Direct"; // Direct, Contracted
    public string Status { get; set; } = "Active"; // Active, OnLeave, Suspended
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public ICollection<DutyAssignment> DutyAssignments { get; set; } = new List<DutyAssignment>();
}

public class Conductor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string EmploymentType { get; set; } = "Direct"; // Direct, Contracted
    public string Status { get; set; } = "Active"; // Active, OnLeave, Suspended
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public ICollection<DutyAssignment> DutyAssignments { get; set; } = new List<DutyAssignment>();
}

public class DutyAssignment
{
    public int Id { get; set; }
    public int DriverId { get; set; }
    public int ConductorId { get; set; }
    public int BusId { get; set; }
    public int RouteId { get; set; }
    public string Shift { get; set; } = "Morning"; // Morning, Afternoon, Night
    public DateTime AssignedDate { get; set; }
    public string TripStatus { get; set; } = "Scheduled"; // Scheduled, Started, Delayed, Completed
    public string? DelayReason { get; set; }

    public Driver Driver { get; set; } = null!;
    public Conductor Conductor { get; set; } = null!;
    public Bus Bus { get; set; } = null!;
    public Route Route { get; set; } = null!;
}

public class Attendance
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public string StaffType { get; set; } = string.Empty; // Driver, Conductor
    public int DutyAssignmentId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public double TotalHours { get; set; }

    public DutyAssignment DutyAssignment { get; set; } = null!;
}
