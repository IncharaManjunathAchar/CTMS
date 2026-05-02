namespace backend.Models;

public class Passenger
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PassengerType { get; set; } = "Regular"; // Student, SeniorCitizen, DifferentlyAbled, Regular
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
