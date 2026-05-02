namespace backend.Models;

public class Depot
{
    public int Id { get; set; }
    public string DepotName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Bus> Buses { get; set; } = new List<Bus>();
    public ICollection<BusAssignment> BusAssignments { get; set; } = new List<BusAssignment>();
}

public class BusAssignment
{
    public int Id { get; set; }
    public int DepotId { get; set; }
    public int BusId { get; set; }
    public int RouteId { get; set; }
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ValidUntil { get; set; }
    public bool IsActive { get; set; } = true;

    public Depot Depot { get; set; } = null!;
    public Bus Bus { get; set; } = null!;
    public Route Route { get; set; } = null!;
}
