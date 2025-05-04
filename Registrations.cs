using Autofac;
using REPRPatternApi.Services;

namespace REPRPatternApi;

/// <summary>
/// Registrations class for DI framework.
/// </summary>
public class Registrations : Module
{
    /// <summary>
    /// Load all registrations.
    /// </summary>
    /// <param name="builder"></param>
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        RegisterService(ref builder);
    }

    private static void RegisterService(ref ContainerBuilder builder)
    {
        builder.RegisterType<ProductService>().AsImplementedInterfaces();
        builder.RegisterType<ExternalApiService>().AsImplementedInterfaces();
    }
}