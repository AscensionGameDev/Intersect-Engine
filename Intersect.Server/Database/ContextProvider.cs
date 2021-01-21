using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Server.Database
{
    public sealed partial class ContextProvider
    {
        private Dictionary<Type, DbContext> Contexts { get; }
        private Dictionary<Type, Type> ContextInterfaceTypes { get; }

        public ContextProvider()
        {
            Contexts = new Dictionary<Type, DbContext>();
            ContextInterfaceTypes = new Dictionary<Type, Type>();
        }

        public void Add<TContext>(TContext context) where TContext : IntersectDbContext<TContext>
        {
            var contextType = typeof(TContext);
            var contextInterfaceType = contextType.GetInterfaces()
                .FirstOrDefault(type => typeof(IDbContext) != type && typeof(IDbContext).IsAssignableFrom(type));
            if (Contexts.ContainsKey(contextInterfaceType))
            {
                throw new Exception($"Context for {contextInterfaceType.Name} already exists.");
            }

            Contexts[contextInterfaceType] = context;
        }

        #region Implementation of IContextProvider

        public TContext Access<TContext, TContextInterface>() where TContext : class, IDbContext
        {
            if (!Contexts.TryGetValue(typeof(TContext), out var context))
            {
                throw new Exception($"No context of type {typeof(TContext).Name} exists.");
            }

            return Activator.CreateInstance(typeof(TContextInterface), new object[] { context }) as TContext;
        }

        #endregion
    }
}
