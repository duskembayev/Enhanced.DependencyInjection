using System.Reflection;
using Enhanced.DependencyInjection.Modules;

namespace Enhanced.DependencyInjection.CodeGeneration.Tests;

[TestFixture]
public class SimpleAppTests
{
    private Assembly _assembly;

    [SetUp]
    public void Setup()
    {
        _assembly = typeof(SimpleAppRef).Assembly;
    }

    [Test]
    public void ShouldContainModule()
    {
        var containerType = _assembly.GetType("SimpleApp.Enhanced.DependencyInjection.ContainerModule");

        Assert.NotNull(containerType, "containerType != null");

        Assert.True(containerType.IsAssignableTo(typeof(IContainerModule)),
            "containerType.IsAssignableTo(typeof(IContainerModule))");

        Assert.True(containerType.IsPublic, "containerType.IsPublic");
    }

    [Test]
    public void ShouldContainAssemblyAttribute()
    {
        var attribute = _assembly.GetCustomAttribute<ContainerModuleAttribute>();

        var expectedType = _assembly.GetType("SimpleApp.Enhanced.DependencyInjection.ContainerModule");

        var actualType = typeof(ContainerModuleAttribute)
            .GetProperty("ModuleType", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(attribute) as Type;

        Assert.NotNull(attribute, "attribute != null");
        Assert.NotNull(actualType, "actualType != null");
        Assert.AreSame(expectedType, actualType);
    }

    [Test]
    public void ShouldContainExtensionMethod()
    {
        var extMethodClass =
            _assembly.GetType("SimpleApp.Enhanced.DependencyInjection.EnhancedDependencyInjectionExtensions");

        Assert.NotNull(extMethodClass, "extMethodClass != null");
        Assert.True(extMethodClass.IsNotPublic, "extMethodClass.IsNotPublic");
        Assert.True(extMethodClass.IsSealed & extMethodClass.IsAbstract, "extMethodClass.IsStatic");
    }
}