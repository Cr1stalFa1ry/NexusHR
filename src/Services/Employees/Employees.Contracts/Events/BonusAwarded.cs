namespace Employees.Contracts.Events;

public record BonusAwarded(Guid EmployeeId, decimal Amount, string Reason);
