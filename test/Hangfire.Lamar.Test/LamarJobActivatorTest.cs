using System;
using FakeItEasy;
using Shouldly;
using Lamar;
using Xunit;

namespace Hangfire.Lamar.Test
{
    public class LamarJobActivatorTest
    {
        [Fact]
        public void Ctor_Should_Throw_When_Container_Is_Null()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Should.Throw<ArgumentNullException>(() => new LamarJobActivator(null));
        }

        [Fact]
        public void Class_Is_Based_On_JobActivator()
        {
            var activator = CreateActivator(Container.Empty());

            // ReSharper disable once IsExpressionAlwaysTrue
            var isJobActivator = activator is JobActivator;
            isJobActivator.ShouldBeTrue();
        }

        [Fact]
        public void ActivateJob_Resolves_Instance_Using_Lamar()
        {
            var dependency = new BackgroundJobDependency();
            var container = new Container(c => c.For<BackgroundJobDependency>().Use(dependency));

            var activator = CreateActivator(container);

            var result = activator.ActivateJob(typeof(BackgroundJobDependency));
            result.ShouldBe(dependency);
        }

        [Fact]
        public void Container_Scoped_Instance_Is_Disposed_When_Job_Scope_Is_Disposed()
        {
            var container = new Container(c => c.ForConcreteType<BackgroundJobDependency>().Configure.Scoped());

            BackgroundJobDependency disposable;
            using (var scope = BeginJobScope(container))
            {
                disposable = (BackgroundJobDependency)scope.Resolve(typeof(BackgroundJobDependency));

                disposable.Disposed.ShouldBeFalse();
            }

            // Now the scope is disposed, dependencies should be too
            disposable.Disposed.ShouldBeTrue();
        }

        [Fact]
        public void Singleton_Scoped_Instance_Is_Not_Disposed_When_Job_Scope_Is_Disposed()
        {
            var disposable = new BackgroundJobDependency();
            var container = new Container(c => c.ForSingletonOf<BackgroundJobDependency>().Use(disposable));

            using (var scope = BeginJobScope(container))
            {
                var instance = scope.Resolve(typeof(BackgroundJobDependency)) as BackgroundJobDependency;

                instance.ShouldBe(disposable);
                instance.Disposed.ShouldBeFalse();
            }

            // Singletons should live on after the scope is disposed
            disposable.Disposed.ShouldBeFalse();
        }

        [Fact]
        public void Transient_Scoped_Instance_Is_Disposed_When_Job_Scope_Is_Disposed()
        {
            var container = new Container(c => c.For<BackgroundJobDependency>().Use(_ => new BackgroundJobDependency()).Transient());

            BackgroundJobDependency disposable;
            using (var scope = BeginJobScope(container))
            {
                disposable = scope.Resolve(typeof(BackgroundJobDependency)) as BackgroundJobDependency;

                disposable.ShouldNotBeNull();
                disposable.Disposed.ShouldBeFalse();
            }

            // Now the scope is disposed, dependencies should be too
            disposable.Disposed.ShouldBeTrue();
        }

        /// <summary>
        /// Injecting an existing object into the Container makes it a de facto singleton (or at least, it's up to
        /// the injector to manage its lifecycle)
        /// <seealso cref="http://jasperfx.github.io/lamar/documentation/ioc/registration/existing-objects/"/>
        /// </summary>
        [Fact]
        public void Implicitly_Singleton_Scoped_Instance_Is_Not_Disposed_When_Job_Scope_Is_Disposed()
        {
            var existingInstance = new BackgroundJobDependency();
            var container = new Container(c => c.For<BackgroundJobDependency>().Use(existingInstance));

            using (var scope = BeginJobScope(container))
            {
                var disposable = scope.Resolve(typeof(BackgroundJobDependency)) as BackgroundJobDependency;

                disposable.ShouldBe(existingInstance);
                disposable.Disposed.ShouldBeFalse();
            }
            
            existingInstance.Disposed.ShouldBeFalse();
        }

        [Fact]
        public void Container_Scoped_Instances_Are_Not_Reused_Between_Different_Job_Scopes()
        {
            var container = new Container(c => c.For<object>().Use(_ => new object()).Scoped());

            object instance1;
            using (var scope1 = BeginJobScope(container))
            {
                instance1 = scope1.Resolve(typeof(object));
            }

            object instance2;
            using (var scope2 = BeginJobScope(container))
            {
                instance2 = scope2.Resolve(typeof(object));
            }

            instance1.ShouldNotBe(instance2);
        }

        [Fact]
        public void Container_Scoped_Instance_Is_Reused_Within_Same_Job_Scope()
        {
            var container = new Container(c => c.ForConcreteType<BackgroundJobDependency>().Configure.Scoped());

            using (var scope = BeginJobScope(container))
            {
                var instance = (TestJob)scope.Resolve(typeof(TestJob));

                instance.BackgroundJobDependency.ShouldBe(instance.SameDependencyObject.BackgroundJobDependency);
            }
        }

        [Fact]
        public void Transient_Scoped_Instance_Is_Not_Reused_Within_Same_Job_Scope()
        {
            var container = new Container(c => c.ForConcreteType<UniqueDependency>().Configure.Transient());

            using (var scope = BeginJobScope(container))
            {
                var instance = (TestJob)scope.Resolve(typeof(TestJob));

                instance.UniqueDependency.ShouldNotBe(instance.SameDependencyObject.UniqueDependency);
            }
        }

        private static JobActivatorScope BeginJobScope(IContainer container)
        {
            var activator = CreateActivator(container);
#if NET461
#pragma warning disable CS0618 // Type or member is obsolete
            return activator.BeginScope();
#pragma warning restore CS0618 // Type or member is obsolete
#else
            return activator.BeginScope(null);
#endif
        }

#if NET461
        [Fact]
        public void Bootstrapper_Use_LamarActivator_Passes_Correct_Activator()
        {
#pragma warning disable 618
            var configuration = A.Fake<IBootstrapperConfiguration>();

            configuration.UseLamarActivator(Container.Empty());

            A.CallTo(() => configuration.UseActivator(A<LamarJobActivator>.That.IsNotNull())).MustHaveHappened();
#pragma warning restore 618
        }
#endif

        private static LamarJobActivator CreateActivator(IContainer container) => new LamarJobActivator(container);
    }
}
