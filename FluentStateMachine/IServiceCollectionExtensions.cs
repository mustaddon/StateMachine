using FluentStateMachine;
using FluentStateMachine._internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;


public static class FsmServiceCollectionExtensions
{
    public static IServiceCollection AddFsm(this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        return services.AddFsm(cfg => cfg.RegisterServicesFromAssembly(assembliesToScan));
    }

    public static IServiceCollection AddFsm(this IServiceCollection services, Action<FsmServiceConfiguration> configuration)
    {
        var config = new FsmServiceConfiguration();
        configuration(config);
        return services.AddFsm(config);
    }

    public static IServiceCollection AddFsm(this IServiceCollection services, FsmServiceConfiguration configuration)
    {
        services.AddSingleton(x => new FsmFactoryRegister(services));
        services.AddTransient(typeof(IFsmFactory<>), typeof(FsmFactoryRequestAdapter<>));
        services.AddFactories(configuration);
        return services;
    }

    static void AddFactories(this IServiceCollection services, FsmServiceConfiguration configuration)
    {
        var facOpenType = typeof(IFsmFactory<>);
        var serviceDescriptors = configuration.AssembliesToRegister
            .SelectMany(x => x.GetTypes()
                .Where(x => !x.IsAbstract && !x.IsGenericTypeDefinition && configuration.TypeEvaluator(x)))
            .SelectMany(x => x.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == facOpenType)
                .Select(i => new ServiceDescriptor(i, x, configuration.Lifetime)));

        foreach (var item in serviceDescriptors)
            services.Add(item);
    }
}


public class FsmServiceConfiguration
{
    internal readonly List<Assembly> AssembliesToRegister = [];
    internal ServiceLifetime Lifetime = ServiceLifetime.Transient;
    internal Func<Type, bool> TypeEvaluator = t => true;

    public FsmServiceConfiguration RegisterServicesFromAssembly(params Assembly[] assemblies)
    {
        AssembliesToRegister.AddRange(assemblies);
        return this;
    }

    public FsmServiceConfiguration SetLifetime(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
        return this;
    }

    public FsmServiceConfiguration SetTypeEvaluator(Func<Type, bool> typeEvaluator)
    {
        TypeEvaluator = typeEvaluator ?? throw new ArgumentNullException(nameof(typeEvaluator));
        return this;
    }
}