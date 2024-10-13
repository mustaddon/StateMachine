using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FluentStateMachine._internal;

internal class FsmFactoryRegister(IServiceCollection services)
{
    readonly Lazy<HashSet<Type>> _registred = new(() => new(GetFactoryTypes(services)));
    readonly ConcurrentDictionary<Type, Type> _cache = new();

    public Type FindFactoryTypeForRequest(Type requestType)
    {
        return _cache.GetOrAdd(requestType, CreateCacheValue);
    }

    Type CreateCacheValue(Type type)
    {
        var findType = typeof(IFsmFactory<>).MakeGenericType(type);

        if (_registred.Value.Contains(findType))
            return findType;

        return _registred.Value.FirstOrDefault(findType.IsAssignableFrom);
    }

    static IEnumerable<Type> GetFactoryTypes(IServiceCollection services)
    {
        var facOpenType = typeof(IFsmFactory<>);
        foreach (var type in services.Select(x => x.ServiceType))
            if (type.IsInterface && type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == facOpenType)
                yield return type;
    }
}
