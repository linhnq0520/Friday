using Friday.BuildingBlocks.Application.Errors;
using Friday.BuildingBlocks.Application.Exceptions;
using Friday.Modules.Integration.Application.Models;
using Friday.Modules.Integration.Domain.Repositories;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Http;

namespace Friday.Modules.Integration.Application.Features.NotificationTemplates;

public sealed record UpdateNotificationTemplateCommand(
    int Id,
    string SubjectTemplate,
    string BodyTemplate,
    bool IsActive
) : ICommand<NotificationTemplateDto>;

public sealed class UpdateNotificationTemplateHandler(INotificationTemplateRepository repository)
    : ICommandHandler<UpdateNotificationTemplateCommand, NotificationTemplateDto>
{
    public async Task<NotificationTemplateDto> HandleAsync(
        UpdateNotificationTemplateCommand request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.SubjectTemplate))
        {
            throw new FridayException(ErrorCodes.Common.BadRequest, "SubjectTemplate is required.");
        }
        if (string.IsNullOrWhiteSpace(request.BodyTemplate))
        {
            throw new FridayException(ErrorCodes.Common.BadRequest, "BodyTemplate is required.");
        }

        Domain.Aggregates.NotificationTemplateAggregate.NotificationTemplate? updated =
            await repository.UpdateAsync(
            request.Id,
            request.SubjectTemplate,
            request.BodyTemplate,
            request.IsActive,
            cancellationToken
        );
        if (updated is null)
        {
            throw new FridayException(
                ErrorCodes.Common.NotFound,
                $"Notification template '{request.Id}' was not found.",
                StatusCodes.Status404NotFound
            );
        }

        return NotificationTemplateMapping.ToDto(updated);
    }
}
