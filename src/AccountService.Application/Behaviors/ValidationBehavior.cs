using FluentValidation;
using MediatR;

namespace AccountService.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }
        var context = new ValidationContext<TRequest>(request);
        var validationResults = (await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken))));

        var failures = validationResults.Where(r => !r.IsValid)
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            var errors = string.Join("; ", failures.Select(f => f.ErrorMessage));
            var resultType = typeof(TResponse);

            // Пытаемся вызвать Result<T>.Failure(string)
            var failureMethod = resultType.GetMethod("Failure", new[] { typeof(string) });
            if (failureMethod != null)
            {
                return (TResponse)failureMethod.Invoke(null, new object[] { errors });
            }

            throw new ValidationException(failures);

        }


        return await next();
    }
}
