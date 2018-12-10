using System;
using Lamar;
using Hangfire.Annotations;

namespace Hangfire.Lamar
{
    /// <summary>
    /// Global Configuration extensions for Lamar job activation
    /// </summary>
    public static class GlobalConfigurationExtensions
    {
        /// <summary>
        /// Use the specified Lamar container to create a <see cref="LamarJobActivator"/> for resolving job dependencies
        /// </summary>
        /// <param name="configuration">Global configuration</param>
        /// <param name="container">Container used to create nested containers, which will in turn build job dependencies
        /// during the activation process
        /// </param>
        /// <returns>An instance of <see cref="IGlobalConfiguration{LamarJobActivator}"/>, configured to use Lamar
        /// to resolve job dependencies
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/>, <paramref name="container"/></exception>
        public static IGlobalConfiguration<LamarJobActivator> UseLamarActivator([NotNull] this IGlobalConfiguration configuration, 
            [NotNull] IContainer container)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (container == null) throw new ArgumentNullException(nameof(container));

            return configuration.UseActivator(new LamarJobActivator(container));
        }
    }
}
