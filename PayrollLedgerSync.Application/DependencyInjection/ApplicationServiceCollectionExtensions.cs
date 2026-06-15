using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PayrollLedgerSync.Application.Common.Behaviors;
using PayrollLedgerSync.Application.Common.Interfaces;
using PayrollLedgerSync.Application.Common.Services;

namespace PayrollLedgerSync.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationServiceCollectionExtensions).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
