namespace Enhanced.DependencyInjection.CodeGeneration;

internal record DiRegistration(ServiceLifetime Lifetime, ClassDeclarationSyntax ImplType, ImmutableArray<ITypeSymbol> Interfaces);