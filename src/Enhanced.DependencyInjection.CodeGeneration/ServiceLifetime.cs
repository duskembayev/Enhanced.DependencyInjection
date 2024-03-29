﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// ReSharper disable All
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Specifies the lifetime of a service in an IServiceCollection.
/// </summary>
internal enum ServiceLifetime
{
    /// <summary>
    /// Specifies that a single instance of the service will be created.
    /// </summary>
    Singleton,
    /// <summary>
    /// Specifies that a new instance of the service will be created for each scope.
    /// </summary>
    /// <remarks>
    /// In ASP.NET Core applications a scope is created around each server request.
    /// </remarks>
    Scoped,
    /// <summary>
    /// Specifies that a new instance of the service will be created every time it is requested.
    /// </summary>
    Transient
}