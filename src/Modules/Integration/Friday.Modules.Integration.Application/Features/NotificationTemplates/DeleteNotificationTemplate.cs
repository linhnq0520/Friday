using Friday.Modules.Integration.Domain.Repositories;
using LinKit.Core.Cqrs;

namespace Friday.Modules.Integration.Application.Features.NotificationTemplates;

public sealed record DeleteNotificationTemplateCommand(int Id) : ICommand<bool>;

public sealed class DeleteNotificationTemplateHandler(INotificationTemplateRepository repository)
    : ICommandHandler<DeleteNotificationTemplateCommand, bool>
{
    public async Task<bool> HandleAsync(
        DeleteNotificationTemplateCommand request,
        CancellationToken cancellationToken
    )
    {
        return await repository.DeleteAsync(request.Id, cancellationToken);
    }
}
