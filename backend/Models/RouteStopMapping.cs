namespace backend.Models;

public class RouteStopMapping
{
    public int Id { get; set; }
    public int RouteId { get; set; }
    public int StopId { get; set; }
    public int StopSequence { get; set; }
    public double EstimatedMinutesFromPrevious { get; set; }

    public Route Route { get; set; } = null!;
    public Stop Stop { get; set; } = null!;
}
