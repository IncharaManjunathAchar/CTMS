namespace backend.DTOs;

// ── AUTH ──────────────────────────────────────────────
public record RegisterDto(string Name, string Email, string MobileNumber, string Password, string PassengerType);
public record LoginDto(string Email, string Password);
public record UpdateProfileDto(string Name, string MobileNumber, string PassengerType);
public record AuthResponseDto(string Token, string Name, string Email, string PassengerType);

// ── ROUTE ─────────────────────────────────────────────
public record RouteDto(string RouteName, string Source, string Destination, double TotalDistance);
public record RouteResponseDto(int Id, string RouteName, string Source, string Destination, double TotalDistance, bool IsActive);

// ── STOP ──────────────────────────────────────────────
public record StopDto(string StopName, double Latitude, double Longitude, string? Landmark, string? Zone);
public record LinkStopDto(int StopId, int StopSequence, double EstimatedMinutesFromPrevious);
public record StopResponseDto(int Id, string StopName, double Latitude, double Longitude, string? Landmark, string? Zone, int StopSequence, double EstimatedMinutesFromPrevious);

// ── PM ────────────────────────────────────────────────
public record SaveRouteDto(int RouteId);
public record TravelHistoryDto(int RouteId, string? PassOrTicketUsed);
public record FeedbackDto(string Category, string Description);
public record SosAlertDto(double Latitude, double Longitude, int? BusId);

// ── FM ────────────────────────────────────────────────
public record BusDto(string RegistrationNumber, string BusType, int SeatingCapacity, int StandingCapacity, string OperatorName, int? DepotId);
public record UpdateBusDto(string BusType, int SeatingCapacity, int StandingCapacity, string OperatorName, int? DepotId);
public record BusStatusDto(string Status);
public record MaintenanceRecordDto(DateTime ServiceDate, string IssueDescription, string? PartsReplaced, decimal Cost, DateTime? NextDueDate);
public record FuelRecordDto(double FuelFilled, decimal Cost, string? Location);

// ── DM ────────────────────────────────────────────────
public record DepotDto(string DepotName, string Location, int MaxCapacity);
public record AssignBusToDepotDto(int BusId);
public record AssignBusToRouteDto(int BusId, int RouteId, DateTime? ValidUntil);
public record ReassignBusDto(int NewBusId, int RouteId);

// ── DC ────────────────────────────────────────────────
public record DriverDto(string Name, string LicenseNumber, DateTime LicenseExpiry, string ContactNumber, string EmploymentType);
public record ConductorDto(string Name, string EmployeeNumber, string ContactNumber, string EmploymentType);
public record DutyAssignmentDto(int DriverId, int ConductorId, int BusId, int RouteId, string Shift, DateTime AssignedDate);
public record UpdateTripStatusDto(string TripStatus, string? DelayReason);
public record AttendanceDto(int StaffId, string StaffType, int DutyAssignmentId, DateTime CheckIn);

// ── GT ────────────────────────────────────────────────
public record BusLocationDto(int BusId, double Latitude, double Longitude, double Speed);
public record EtaRequestDto(int BusId, int StopId, DateTime CalculatedEta);

// ── FP ────────────────────────────────────────────────
public record FareRuleDto(string PassengerCategory, decimal BaseFare, decimal PerKmRate, decimal ConcessionPercentage, DateTime ValidFrom, DateTime? ValidTo);
public record FareCalculateDto(int BoardingStopId, int AlightingStopId, string PassengerCategory);
public record TicketDto(int BusId, int? ConductorId, int BoardingStopId, int AlightingStopId, string PassengerCategory, string PaymentMode);
public record PassDto(int PassengerId, string PassType, DateTime ValidFrom, DateTime ValidTo, decimal AmountPaid);
public record TransactionDto(int PassengerId, decimal Amount, string PaymentMethod, string? GatewayReference);
public record ValidatePassDto(int PassId, int BusId);

// ── NA ────────────────────────────────────────────────
public record NotificationDto(string RecipientType, int RecipientId, string NotificationType, string Message, string Channel);

// ── EM ────────────────────────────────────────────────
public record IncidentDto(int BusId, int DriverId, int? RouteId, string IncidentType, string Description, double Latitude, double Longitude);
public record IncidentResponseDto(string ActionTaken, string RespondedBy);
public record UpdateIncidentStatusDto(string Status);
