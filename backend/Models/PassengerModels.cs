namespace backend.Models;

public class SavedRoute
{
    public int Id { get; set; }
    public int PassengerId { get; set; }
    public int RouteId { get; set; }
    public int UsageFrequency { get; set; } = 0;
    public DateTime LastUsedAt { get; set; } = DateTime.UtcNow;
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;

    public Passenger Passenger { get; set; } = null!;
    public Route Route { get; set; } = null!;
}

public class TravelHistory
{
    public int Id { get; set; }
    public int PassengerId { get; set; }
    public int RouteId { get; set; }
    public string? PassOrTicketUsed { get; set; }
    public DateTime TravelDate { get; set; } = DateTime.UtcNow;

    public Passenger Passenger { get; set; } = null!;
    public Route Route { get; set; } = null!;
}

public class Feedback
{
    public int Id { get; set; }
    public int PassengerId { get; set; }
    public string Category { get; set; } = string.Empty; // Complaint, Suggestion, LostFound
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Open"; // Open, InProgress, Resolved
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    public Passenger Passenger { get; set; } = null!;
}

public class SosAlert
{
    public int Id { get; set; }
    public int PassengerId { get; set; }
    public int? BusId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string AlertStatus { get; set; } = "Triggered"; // Triggered, Acknowledged, Resolved
    public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    public Passenger Passenger { get; set; } = null!;
}
