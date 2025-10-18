using AccountService.Api;
using AccountService.Application;
using AccountService.Common;
using AccountService.Domain;
using AccountService.Infrastructure;
using AccountService.Persistence;
using ArchUnitNET.Domain;
using ArchUnitNET.Loader;


namespace AccountService.ArchTests;

public static class ArchitectureSetup
{
    public static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(ApiAssemblyMarker).Assembly,
            typeof(ApplicationAssemblyMarker).Assembly,
            typeof(DomainAssemblyMarker).Assembly,
            typeof(InfrastructureAssemblyMarker).Assembly,
            typeof(PersistenceAssemblyMarker).Assembly,
            typeof(CommonAssemblyMarker).Assembly
        )
        .Build();
}
