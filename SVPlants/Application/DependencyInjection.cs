using System;
using System.Reflection;
using Application.Pipelines;
using Application.Plant.Commands;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Add MediatR
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
        services.AddTransient(typeof(IRequestPreProcessor<>), typeof(RequestValidationBehavior<>));

        services.AddMediatR(config => config.AsScoped(), typeof(WaterPlantCommand.Handler).GetTypeInfo().Assembly);

        var validators = AssemblyScanner.FindValidatorsInAssemblyContaining<WaterPlantCommand.Validator>();
        validators.ForEach(validator => services.AddTransient(validator.InterfaceType, validator.ValidatorType));

        return services;
    }
}