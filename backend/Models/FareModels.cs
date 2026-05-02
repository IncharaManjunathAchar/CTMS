namespace backend.Models;

public class FareRule
{
    public int Id { get; set; }
    public string PassengerCategory { get; set; } = "General"; // General, Student, SeniorCitizen
    public decimal BaseFare { get; set; }
    public decimal PerKmRate { get; set; }
    public decimal ConcessionPercentage { get; set; } = 0;
    public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
    public DateTime? ValidTo { get; set; }
}

public class Ticket
{
    public int Id { get; set; }
    public int BusId { get; set; }
    public int? ConductorId { get; set; }
    public int BoardingStopId { get; set; }
    public int AlightingStopId { get; set; }
    public string PassengerCategory { get; set; } = "General";
    public decimal FareAmount { get; set; }
    public string PaymentMode { get; set; } = "Cash";
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    public Bus Bus { get; set; } = null!;
    public Stop BoardingStop { get; set; } = null!;
    public Stop AlightingStop { get; set; } = null!;
}

public class Pass
{
    public int Id { get; set; }
    public int PassengerId { get; set; }
    public string PassType { get; set; } = string.Empty; // Monthly, Weekly, Student, SeniorCitizen
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public Passenger Passenger { get; set; } = null!;
}

public class Transaction
{
    public int Id { get; set; }
    public int PassengerId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // UPI, Card, Wallet, Cash
    public string? GatewayReference { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Success, Failed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Passenger Passenger { get; set; } = null!;
}
