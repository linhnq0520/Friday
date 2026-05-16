using FluentValidation;
using FluentValidation.Results;
using Friday.BuildingBlocks.Application.Errors;
using Friday.BuildingBlocks.Application.Exceptions;
using LinKit.Core.Cqrs;

namespace Friday.BuildingBlocks.Application.Behaviors;

[CqrsBehavior(typeof(ICommand), -100)]
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!_validators.Any())
        {
            return await next();
        }

        ValidationContext<TRequest> context = new(request);
        ValidationFailure[] failures = (
            await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(context, cancellationToken))
            )
        )
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToArray();

        if (failures.Length > 0)
        {
            string message = string.Join("; ", failures.Select(failure => failure.ErrorMessage));
            throw new FridayException(ErrorCodes.Common.BadRequest, message);
        }

        return await next();
    }
}
