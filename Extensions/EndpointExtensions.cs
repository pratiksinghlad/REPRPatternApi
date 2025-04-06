using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using REPRPatternApi.Endpoints;

namespace REPRPatternApi.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        ServiceDescriptor[] serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static void MapEndpoints(this IEndpointRouteBuilder endpoints, RouteGroupBuilder group)
    {
        var endpointTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in endpointTypes)
        {
            var endpointInstance = (IEndpoint)Activator.CreateInstance(type)!;
            endpointInstance.MapEndpoint(group);
        }
    }
}
