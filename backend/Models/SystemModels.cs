namespace backend.Models;

// NA - Notifications
public class Notification
{
    public int Id { get; set; }
    public string RecipientType { get; set; } = string.Empty; // Passenger, Driver, DepotManager
    public int RecipientId { get; set; }
    public string NotificationType { get; set; } = string.Empty; // Delay, RouteChange, Payment, Emergency
    public string Message { get; set; } = string.Empty;
    public string Channel { get; set; } = "Push"; // Push, SMS, Email
    public string DeliveryStatus { get; set; } = "Pending"; // Pending, Sent, Failed
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
}

// EM - Emergency & Incidents
public class Incident
{
    public int Id { get; set; }
    public int BusId { get; set; }
    public int DriverId { get; set; }
    public int? RouteId { get; set; }
    public string IncidentType { get; set; } = string.Empty; // Breakdown, Accident, Medical, Dispute
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Status { get; set; } = "Reported"; // Reported, InProgress, Resolved
    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;

    public Bus Bus { get; set; } = null!;
    public Driver Driver { get; set; } = null!;
    public ICollection<IncidentResponse> Responses { get; set; } = new List<IncidentResponse>();
}

public class IncidentResponse
{
    public int Id { get; set; }
    public int IncidentId { get; set; }
    public string ActionTaken { get; set; } = string.Empty;
    public string RespondedBy { get; set; } = string.Empty;
    public DateTime RespondedAt { get; set; } = DateTime.UtcNow;

    public Incident Incident { get; set; } = null!;
}

// RBAC
public class Role
{
    public int Id { get; set; }
    public string RoleName { get; set; } = string.Empty; // Admin, Passenger, Driver, Conductor, DepotManager
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

public class UserRole
{
    public int Id { get; set; }
    public int PassengerId { get; set; }
    public int RoleId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public Passenger Passenger { get; set; } = null!;
    public Role Role { get; set; } = null!;
}

// AS - Admin
public class AuditLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
