namespace backend.Models;

public class Route
{
    public int Id { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public double TotalDistance { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<RouteStopMapping> RouteStopMappings { get; set; } = new List<RouteStopMapping>();
}
