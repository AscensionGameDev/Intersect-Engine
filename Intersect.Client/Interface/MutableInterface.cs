using Intersect.Client.Framework.Gwen.Control;
using Intersect.Client.Interface.Debugging;
using Intersect.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Client.Interface
{

    public abstract partial class MutableInterface : IMutableInterface
    {

        private static DebugWindow _debugWindow;

        public static void DetachDebugWindow()
        {
            if (_debugWindow != null)
            {
                _debugWindow.Parent = default;
            }
        }

        internal static void DisposeDebugWindow() => _debugWindow?.Dispose();

        private static void EnsureDebugWindowInitialized(Base parent)
        {
            if (_debugWindow == default)
            {
                _debugWindow = new DebugWindow(parent);
            }

            _debugWindow.Parent = parent;
        }

        protected internal MutableInterface(Base root)
        {
            Root = root;

            EnsureDebugWindowInitialized(root);
        }

        internal Base Root { get; }

        /// <inheritdoc />
        public List<Base> Children => Root.Children ?? new List<Base>();

        /// <inheritdoc />
        public TElement Create<TElement>(params object[] parameters) where TElement : Base
        {
            var fullParameterList = new List<object> {Root};
            fullParameterList.AddRange(parameters);

            var fullParameters = fullParameterList.ToArray();
            var constructor = typeof(TElement).FindConstructors(fullParameters).FirstOrDefault();

            if (constructor != null)
            {
                return constructor.Invoke(fullParameters) as TElement;
            }

            throw new NullReferenceException("Failed to find constructor that matches parameters.");
        }

        /// <inheritdoc />
        public TElement Find<TElement>(string name = null, bool recurse = false) where TElement : Base
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return Root.FindChildByName(name, recurse) as TElement;
            }

            return Root.Find(child => child is TElement) as TElement;
        }

        /// <inheritdoc />
        public IEnumerable<TElement> FindAll<TElement>(bool recurse = false) where TElement : Base =>
            Root.FindAll(child => child is TElement, recurse).Select(child => child as TElement);

        /// <inheritdoc />
        public void Remove<TElement>(TElement element, bool dispose = false) where TElement : Base =>
            Root.RemoveChild(element, dispose);

        public static bool ToggleDebug()
        {
            _debugWindow.ToggleHidden();
            return _debugWindow.IsVisible;
        }
    }
}
