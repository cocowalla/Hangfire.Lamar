using System;
using Lamar;

namespace Hangfire.Lamar
{
    /// <summary>
    /// Lamar Job Activator - builds job dependencies using nested Lamar containers
    /// </summary>
    public class LamarJobActivator : JobActivator
    {
        private readonly IContainer container;

        /// <summary>
        /// Initialize a new instance of <see cref="LamarJobActivator"/> with a Lamar container
        /// </summary>
        /// <param name="container">Container used to create nested containers, which will in turn build job dependencies
        /// during the activation process
        /// </param>
        public LamarJobActivator(IContainer container)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <inheritdoc />
        /// <summary>
        /// Activate a job using the parent container
        /// </summary>
        /// <returns>An activated job of type <paramref name="jobType"/></returns>
        public override object ActivateJob(Type jobType)
        {
            return this.container.GetInstance(jobType);
        }

        /// <inheritdoc />
        /// <summary>
        /// Begin a new job activation scope using a nested container
        /// </summary>
        /// <param name="context">Job activator context</param>
        /// <returns>A new job activation scope</returns>
        public override JobActivatorScope BeginScope(JobActivatorContext context)
        {
            return new LamarDependencyScope(this.container.GetNestedContainer());
        }

#pragma warning disable CS0672 // Member overrides obsolete member
        /// <inheritdoc />
        public override JobActivatorScope BeginScope()
#pragma warning restore CS0672 // Member overrides obsolete member
        {
            return new LamarDependencyScope(this.container.GetNestedContainer());
        }
    }
}
