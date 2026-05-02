namespace backend.Models;

public class Stop
{
    public int Id { get; set; }
    public string StopName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Landmark { get; set; }
    public string? Zone { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<RouteStopMapping> RouteStopMappings { get; set; } = new List<RouteStopMapping>();
}
