using Employees.Contracts.Events;

namespace Notifications.Api.Handlers;

public class BonusNotificationHandler
{
    public async static Task Handle(BonusAwarded message, ILogger<BonusNotificationHandler> logger)
    {
        logger.LogInformation(
            "ОТПРАВКА EMAIL: Уважаемый сотрудник {Id}, вам начислен бонус {Amount} за {Reason}!", 
            message.EmployeeId, message.Amount, message.Reason);
    }
}
