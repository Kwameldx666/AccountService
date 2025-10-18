using System;
using System.Collections.Generic;
using System.Linq;
using AccountService.Api;
using AccountService.Application;
using AccountService.Common;
using AccountService.Domain;
using AccountService.Infrastructure;
using AccountService.Persistence;
using ArchUnitNET.Domain;
using ArchUnitNET.xUnit;
using FluentValidation;
using MediatR;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace AccountService.ArchTests;

public class ArchitectureRules
{
    private static readonly Architecture Architecture = ArchitectureSetup.Architecture;

    private static string NamespaceOf<T>() => typeof(T).Namespace
        ?? throw new InvalidOperationException($"Namespace for {typeof(T).Name} cannot be resolved.");

    private static readonly string ApiNamespace = NamespaceOf<ApiAssemblyMarker>();
    private static readonly string ApplicationNamespace = NamespaceOf<ApplicationAssemblyMarker>();
    private static readonly string DomainNamespace = NamespaceOf<DomainAssemblyMarker>();
    private static readonly string InfrastructureNamespace = NamespaceOf<InfrastructureAssemblyMarker>();
    private static readonly string PersistenceNamespace = NamespaceOf<PersistenceAssemblyMarker>();
    private static readonly string CommonNamespace = NamespaceOf<CommonAssemblyMarker>();

    private static IEnumerable<IType> TypesInNamespace(string namespacePrefix) =>
        Architecture.Types.Where(t =>
            t.Namespace.FullName.StartsWith(namespacePrefix, StringComparison.Ordinal));

    private static IEnumerable<IType> NamespaceTypes(params string[] namespaces) =>
        namespaces.SelectMany(TypesInNamespace);

    [Fact]
    public void Controllers_Should_Have_Name_Ending_With_Controller()
    {
        var rule = Types()
            .That().ResideInNamespace($"{ApiNamespace}.Controllers", true)
            .Should().HaveNameEndingWith("Controller");

        rule.Check(Architecture);
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Outer_Layers()
    {
        var forbiddenTypes = TypesInNamespace(ApiNamespace)
            .Concat(TypesInNamespace(InfrastructureNamespace))
            .Concat(TypesInNamespace(PersistenceNamespace));

        var rule = Types()
            .That().ResideInNamespace(ApplicationNamespace, true)
            .Should().NotDependOnAny(forbiddenTypes.ToArray());

        rule.Check(Architecture);
    }

    [Fact]
    public void Domain_Should_Remain_Isolated()
    {
        var outerLayerTypes = NamespaceTypes(
            ApiNamespace,
            ApplicationNamespace,
            InfrastructureNamespace,
            PersistenceNamespace);

        var rule = Types()
            .That().ResideInNamespace(DomainNamespace, true)
            .Should().NotDependOnAny(outerLayerTypes.ToArray());

        rule.Check(Architecture);
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Presentation()
    {
        var forbiddenTypes = NamespaceTypes(ApiNamespace, PersistenceNamespace);

        var rule = Types()
            .That().ResideInNamespace(InfrastructureNamespace, true)
            .Should().NotDependOnAny(forbiddenTypes.ToArray());

        rule.Check(Architecture);
    }

    [Fact]
    public void Persistence_Should_Not_Depend_On_Presentation_Or_Application()
    {
        var forbiddenTypes = NamespaceTypes(ApiNamespace, ApplicationNamespace);

        var rule = Types()
            .That().ResideInNamespace(PersistenceNamespace, true)
            .Should().NotDependOnAny(forbiddenTypes.ToArray());

        rule.Check(Architecture);
    }

    [Fact]
    public void Presentation_Should_Not_Depend_On_Domain_Infrastructure_Or_Persistence()
    {
        var forbiddenTypes = NamespaceTypes(
            DomainNamespace,
            InfrastructureNamespace,
            PersistenceNamespace);

        var rule = Types()
            .That().ResideInNamespace(ApiNamespace, true)
            .Should().NotDependOnAny(forbiddenTypes.ToArray());

        rule.Check(Architecture);
    }

    [Fact]
    public void Common_Should_Not_Depend_On_Other_Layers()
    {
        var forbiddenTypes = NamespaceTypes(
            ApiNamespace,
            ApplicationNamespace,
            DomainNamespace,
            InfrastructureNamespace,
            PersistenceNamespace);

        var rule = Types()
            .That().ResideInNamespace(CommonNamespace, true)
            .Should().NotDependOnAny(forbiddenTypes.ToArray());

        rule.Check(Architecture);
    }

    [Fact]
    public void Handlers_Should_Be_Suffixed_With_Handler()
    {
        var rule = Types()
            .That().ResideInNamespace($"{ApplicationNamespace}.Features", true)
            .And().ImplementInterface(typeof(IRequestHandler<,>))
            .Should().HaveNameEndingWith("Handler");

        rule.Check(Architecture);
    }

    [Fact]
    public void Handlers_Should_Implement_IRequestHandler()
    {
        var rule = Types()
            .That().ResideInNamespace($"{ApplicationNamespace}.Features", true)
            .And().HaveNameEndingWith("Handler")
            .Should().ImplementInterface(typeof(IRequestHandler<,>));

        rule.Check(Architecture);
    }

    [Fact]
    public void Commands_Should_Not_Depend_On_Queries()
    {
        var queryTypes = TypesInNamespace($"{ApplicationNamespace}.Features.Queries");

        var rule = Types()
            .That().ResideInNamespace($"{ApplicationNamespace}.Features.Commands", true)
            .Should().NotDependOnAny(queryTypes.ToArray());

        rule.Check(Architecture);
    }

    [Fact]
    public void Commands_Should_Implement_IRequest()
    {
        var rule = Types()
            .That().ResideInNamespace($"{ApplicationNamespace}.Features.Commands", true)
            .And().DoNotHaveNameEndingWith("Handler")
            .Should().ImplementInterface(typeof(IRequest<>));

        rule.Check(Architecture);
    }

    [Fact]
    public void Queries_Should_Not_Depend_On_Commands()
    {
        var commandTypes = TypesInNamespace($"{ApplicationNamespace}.Features.Commands");

        var rule = Types()
            .That().ResideInNamespace($"{ApplicationNamespace}.Features.Queries", true)
            .Should().NotDependOnAny(commandTypes.ToArray());

        rule.Check(Architecture);
    }

    [Fact]
    public void Queries_Should_Implement_IRequest()
    {
        var rule = Types()
            .That().ResideInNamespace($"{ApplicationNamespace}.Features.Queries", true)
            .And().DoNotHaveNameEndingWith("Handler")
            .Should().ImplementInterface(typeof(IRequest<>));

        rule.Check(Architecture);
    }

    [Fact]
    public void Commands_Should_Be_Suffixed_With_Command()
    {
        var rule = Types()
            .That().ResideInNamespace($"{ApplicationNamespace}.Features.Commands", true)
            .And().DoNotHaveNameEndingWith("Handler")
            .Should().HaveNameEndingWith("Command");

        rule.Check(Architecture);
    }

    [Fact]
    public void Queries_Should_Be_Suffixed_With_Query()
    {
        var rule = Types()
            .That().ResideInNamespace($"{ApplicationNamespace}.Features.Queries", true)
            .And().DoNotHaveNameEndingWith("Handler")
            .Should().HaveNameEndingWith("Query");

        rule.Check(Architecture);
    }

    [Fact]
    public void Behaviors_Should_Implement_Pipeline_Interface()
    {
        var rule = Types()
            .That().ResideInNamespace($"{ApplicationNamespace}.Behaviors", true)
            .Should().ImplementInterface(typeof(IPipelineBehavior<,>));

        rule.Check(Architecture);
    }

    [Fact]
    public void Validators_Should_Be_Suffixed_With_Validator()
    {
        var rule = Types()
            .That().ResideInNamespace($"{ApplicationNamespace}.Features.Validators", true)
            .Should().HaveNameEndingWith("Validator");

        rule.Check(Architecture);
    }

    [Fact]
    public void Validators_Should_Inherit_From_AbstractValidator()
    {
        var rule = Types()
            .That().ResideInNamespace($"{ApplicationNamespace}.Features.Validators", true)
            .Should().BeAssignableTo(typeof(AbstractValidator<>));

        rule.Check(Architecture);
    }

    [Fact]
    public void Assembly_Markers_Should_Be_Sealed()
    {
        var rule = Classes()
            .That().HaveNameEndingWith("AssemblyMarker")
            .Should().BeSealed();

        rule.Check(Architecture);
    }
}
