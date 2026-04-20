using Friday.BuildingBlocks.Application.Errors;
using Friday.BuildingBlocks.Application.Exceptions;
using Friday.Modules.Integration.Application.Models;
using Friday.Modules.Integration.Domain.Repositories;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Http;

namespace Friday.Modules.Integration.Application.Features.NotificationTemplates;

public sealed record GetNotificationTemplateByIdQuery(int Id) : IQuery<NotificationTemplateDto>;

public sealed class GetNotificationTemplateByIdHandler(INotificationTemplateRepository repository)
    : IQueryHandler<GetNotificationTemplateByIdQuery, NotificationTemplateDto>
{
    public async Task<NotificationTemplateDto> HandleAsync(
        GetNotificationTemplateByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        Domain.Aggregates.NotificationTemplateAggregate.NotificationTemplate? item = await repository.GetByIdAsync(
            request.Id,
            cancellationToken
        );
        if (item is null)
        {
            throw new FridayException(
                ErrorCodes.Common.NotFound,
                $"Notification template '{request.Id}' was not found.",
                StatusCodes.Status404NotFound
            );
        }

        return NotificationTemplateMapping.ToDto(item);
    }
}
