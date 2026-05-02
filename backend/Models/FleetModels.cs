namespace backend.Models;

public class Bus
{
    public int Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string BusType { get; set; } = string.Empty; // AC, NonAC, Electric, CNG
    public int SeatingCapacity { get; set; }
    public int StandingCapacity { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    public int? DepotId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Depot? Depot { get; set; }
    public ICollection<BusStatus> BusStatuses { get; set; } = new List<BusStatus>();
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    public ICollection<FuelRecord> FuelRecords { get; set; } = new List<FuelRecord>();
}

public class BusStatus
{
    public int Id { get; set; }
    public int BusId { get; set; }
    public string Status { get; set; } = "Idle"; // Running, UnderMaintenance, Idle, Breakdown
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Bus Bus { get; set; } = null!;
}

public class MaintenanceRecord
{
    public int Id { get; set; }
    public int BusId { get; set; }
    public DateTime ServiceDate { get; set; }
    public string IssueDescription { get; set; } = string.Empty;
    public string? PartsReplaced { get; set; }
    public decimal Cost { get; set; }
    public DateTime? NextDueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Bus Bus { get; set; } = null!;
}

public class FuelRecord
{
    public int Id { get; set; }
    public int BusId { get; set; }
    public double FuelFilled { get; set; } // litres
    public decimal Cost { get; set; }
    public string? Location { get; set; }
    public DateTime FilledAt { get; set; } = DateTime.UtcNow;

    public Bus Bus { get; set; } = null!;
}
