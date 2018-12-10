using System;
using Lamar;

namespace Hangfire.Lamar
{
    internal class LamarDependencyScope : JobActivatorScope
    {
        private readonly INestedContainer container;

        public LamarDependencyScope(INestedContainer container)
        {
            this.container = container;
        }

        public override object Resolve(Type type)
        {
            return this.container.GetInstance(type);
        }

        public override void DisposeScope()
        {
            this.container.Dispose();
        }
    }
}
