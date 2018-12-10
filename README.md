Hangfire.Lamar
=====================

[![Windows Build Status](https://img.shields.io/appveyor/ci/cocowalla/hangfire-lamar.svg?label=Windows%20Build)](https://ci.appveyor.com/project/ionx-solutions/Hangfire.Lamar)
[![Linux Build status](https://img.shields.io/travis/cocowalla/Hangfire.Lamar.svg?label=Linux%20Build)](https://travis-ci.org/cocowalla/Hangfire.Lamar)
[![NuGet](https://img.shields.io/nuget/v/Hangfire.Lamar.svg)](https://www.nuget.org/packages/Hangfire.Lamar)

This package provides [Lamar](http://Lamar.github.io/) support for [Hangfire](http://hangfire.io), allowing nested Lamar containers to resolve job type instances and their dependencies, and to manage the lifetime of resolved instances.

Getting started
---------------

Install the [Hangfire.Lamar](https://www.nuget.org/packages/Hangfire.Lamar) package from NuGet:

```powershell
Install-Package Hangfire.Lamar
```

To configure Hangfire to use Lamar, configure your container and call the `IGlobalConfiguration` extension method, `UseLamarActivator`:

```csharp
var container = new Container();
// container.Configure...

GlobalConfiguration.Configuration.UseLamarActivator(container);
```

After configuration, when jobs are started a Lamar-based implementation of the `JobActivator` class is used to resolve job type instances and all of their dependencies.

Object Lifecycles
-----------------

*Hangfire.Lamar* doesn't rely on a specific object lifecycle - you can configure your dependencies as `Singleton`, `Scoped` or `Transient` as normal.

*Hangfire.Lamar* creates a [*nested container*](http://Lamar.github.io/the-container/nested-containers/) for each job execution, so using `Scoped` will scope dependency lifetimes to that of the job.

```csharp
var container = new Container(c =>
  c.For<IRepository>().Use<GenericRepository>().Scoped();
);
```

The nested container is disposed when jobs ends, and all dependencies that implement the `IDisposable` interface are also automatically disposed  (`Singleton` scoped instances are of course an exception, and are only disposed when the root container is disposed).
