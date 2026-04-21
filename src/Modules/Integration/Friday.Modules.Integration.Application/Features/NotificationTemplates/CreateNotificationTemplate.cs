using Friday.BuildingBlocks.Application.Errors;
using Friday.BuildingBlocks.Application.Exceptions;
using Friday.Modules.Integration.Application.Models;
using Friday.Modules.Integration.Domain.Repositories;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Http;

namespace Friday.Modules.Integration.Application.Features.NotificationTemplates;

public sealed record CreateNotificationTemplateCommand(
    string Channel,
    string TemplateCode,
    string Language,
    string SubjectTemplate,
    string BodyTemplate,
    bool IsActive,
    int? Version
) : ICommand<NotificationTemplateDto>;

public sealed class CreateNotificationTemplateHandler(INotificationTemplateRepository repository)
    : ICommandHandler<CreateNotificationTemplateCommand, NotificationTemplateDto>
{
    public async Task<NotificationTemplateDto> HandleAsync(
        CreateNotificationTemplateCommand request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Channel))
        {
            throw new FridayException(ErrorCodes.Common.BadRequest, "Channel is required.");
        }
        if (string.IsNullOrWhiteSpace(request.TemplateCode))
        {
            throw new FridayException(ErrorCodes.Common.BadRequest, "TemplateCode is required.");
        }
        if (string.IsNullOrWhiteSpace(request.Language))
        {
            throw new FridayException(ErrorCodes.Common.BadRequest, "Language is required.");
        }
        if (string.IsNullOrWhiteSpace(request.SubjectTemplate))
        {
            throw new FridayException(ErrorCodes.Common.BadRequest, "SubjectTemplate is required.");
        }
        if (string.IsNullOrWhiteSpace(request.BodyTemplate))
        {
            throw new FridayException(ErrorCodes.Common.BadRequest, "BodyTemplate is required.");
        }
        if (request.Version is <= 0)
        {
            throw new FridayException(
                ErrorCodes.Common.BadRequest,
                "Version must be greater than zero.",
                StatusCodes.Status400BadRequest
            );
        }

        Domain.Aggregates.NotificationTemplateAggregate.NotificationTemplate entity =
            await repository.CreateAsync(
                request.Channel,
                request.TemplateCode,
                request.Language,
                request.SubjectTemplate,
                request.BodyTemplate,
                request.IsActive,
                request.Version,
                cancellationToken
            );
        return NotificationTemplateMapping.ToDto(entity);
    }
}
