﻿using FluentValidation;
using MediatR;
using Shared.Bases;

namespace Shared.Behavoir
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())  // if behavoir run the fluentValidator and find errors so it will prevent the command to reach the handler
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    var responseType = typeof(TResponse); //reflection
                    if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Response<>))
                    {
                        var responseInstance = (TResponse)Activator.CreateInstance(responseType);//reflection
                        var responseProperty = responseType.GetProperty("Errors");//reflection
                        responseProperty?.SetValue(responseInstance, failures.Select(e => e.ErrorMessage).ToList());
                        return responseInstance;
                    }

                    throw new ValidationException(failures);
                }
            }

            return await next(); // ok this validation is alright the got to handler 
        }
    }
}
