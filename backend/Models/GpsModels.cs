namespace backend.Models;

public class BusLocation
{
    public int Id { get; set; }
    public int BusId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Speed { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

    public Bus Bus { get; set; } = null!;
}

public class EtaRecord
{
    public int Id { get; set; }
    public int BusId { get; set; }
    public int StopId { get; set; }
    public DateTime CalculatedEta { get; set; }
    public DateTime? ActualArrival { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Bus Bus { get; set; } = null!;
    public Stop Stop { get; set; } = null!;
}
