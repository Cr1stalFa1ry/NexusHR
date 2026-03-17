namespace Employees.Contracts.Events;

public record HiredEmployee(Guid EmployeeId, string FirstName, string LastName, string Email, string? Contacts);

