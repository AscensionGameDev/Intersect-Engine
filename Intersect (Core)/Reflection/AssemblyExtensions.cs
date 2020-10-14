using JetBrains.Annotations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Intersect.Reflection
{
    /// <summary>
    /// Extension methods for <see cref="Assembly"/>.
    /// </summary>
    [UsedImplicitly]
    public static class AssemblyExtensions
    {
        /// <inheritdoc cref="CreateInstanceOf{TParentType}(Assembly, Func{Type, bool}, object[])"/>
        [NotNull]
        [Pure]
        public static TParentType CreateInstanceOf<TParentType>([NotNull] this Assembly assembly, params object[] args) =>
            assembly.CreateInstanceOf<TParentType>(_ => true, args);

        /// <summary>
        /// Creates an instance of <typeparamref name="TParentType"/> given <paramref name="args"/>.
        /// </summary>
        /// <typeparam name="TParentType">the type to search for subtypes and create an instance of</typeparam>
        /// <param name="assembly">the <see cref="Assembly"/> to search for subtypes in</param>
        /// <param name="predicate">a function to filter subtypes with</param>
        /// <param name="args">the arguments to create the instance with</param>
        /// <returns>an instance of <typeparamref name="TParentType"/></returns>
        /// <exception cref="InvalidOperationException">if no matching subtypes are found, or instance creation fails</exception>
        [NotNull]
        [Pure]
        public static TParentType CreateInstanceOf<TParentType>([NotNull] this Assembly assembly, [NotNull] Func<Type, bool> predicate, params object[] args)
        {
            var validTypes = assembly.FindDefinedSubtypesOf<TParentType>();
            var type = validTypes.FirstOrDefault(predicate);

            if (type == default)
            {
                throw new InvalidOperationException($"Found no matching subtype of {typeof(TParentType).FullName} that can be created.");
            }

            if (Activator.CreateInstance(type, args) is TParentType instance)
            {
                return instance;
            }

            throw new InvalidOperationException($"Failed to create instance of {typeof(TParentType).FullName}.");
        }

        [NotNull]
        public static IEnumerable<Type> FindAbstractSubtypesOf([NotNull] this Assembly assembly, [NotNull] Type type) =>
            FindSubtypesOf(assembly, type).Where(subtype => subtype?.IsAbstract ?? false);

        [NotNull]
        public static IEnumerable<Type> FindAbstractSubtypesOf<TParentType>([NotNull] this Assembly assembly) =>
            FindAbstractSubtypesOf(assembly, typeof(TParentType));

        [NotNull]
        public static IEnumerable<Type> FindDefinedSubtypesOf([NotNull] this Assembly assembly, [NotNull] Type type) =>
            FindSubtypesOf(assembly, type).Where(subtype => !(subtype == null || subtype.IsAbstract || subtype.IsGenericType || subtype.IsInterface));

        [NotNull]
        public static IEnumerable<Type> FindDefinedSubtypesOf<TParentType>([NotNull] this Assembly assembly) =>
            FindDefinedSubtypesOf(assembly, typeof(TParentType));

        [NotNull]
        public static IEnumerable<Type> FindGenericSubtypesOf([NotNull] this Assembly assembly, [NotNull] Type type) =>
            FindSubtypesOf(assembly, type).Where(subtype => subtype?.IsGenericType ?? false);

        [NotNull]
        public static IEnumerable<Type> FindGenericSubtypesOf<TParentType>([NotNull] this Assembly assembly) =>
            FindGenericSubtypesOf(assembly, typeof(TParentType));

        [NotNull]
        public static IEnumerable<Type> FindInterfaceSubtypesOf([NotNull] this Assembly assembly, [NotNull] Type type) =>
            FindSubtypesOf(assembly, type).Where(subtype => subtype?.IsInterface ?? false);

        [NotNull]
        public static IEnumerable<Type> FindInterfaceSubtypesOf<TParentType>([NotNull] this Assembly assembly) =>
            FindInterfaceSubtypesOf(assembly, typeof(Type));

        [NotNull]
        public static IEnumerable<Type> FindSubtypesOf([NotNull] this Assembly assembly, [NotNull] Type type) =>
            assembly.GetTypes().Where(type.IsAssignableFrom);

        [NotNull]
        public static IEnumerable<Type> FindSubtypesOf<TParentType>([NotNull] this Assembly assembly) =>
            FindGenericSubtypesOf(assembly, typeof(TParentType));

        [NotNull]
        public static IEnumerable<Type> FindValueSubtypesOf([NotNull] this Assembly assembly, [NotNull] Type type) =>
            FindSubtypesOf(assembly, type).Where(subtype => subtype?.IsValueType ?? false);

        [NotNull]
        public static IEnumerable<Type> FindValueSubtypesOf<TParentType>([NotNull] this Assembly assembly) =>
            FindValueSubtypesOf(assembly, typeof(Type));
    }
}
