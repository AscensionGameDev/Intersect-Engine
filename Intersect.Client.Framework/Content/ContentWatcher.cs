using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Intersect.Client.Framework.Content
{
    public sealed partial class ContentWatcher
    {
        public enum Event
        {
            Change,
            Create,
            Delete,
            Rename,
        }

        private readonly ConcurrentDictionary<string, bool> _ignore;
        private readonly Dictionary<Event, Dictionary<string, List<Action>>> _mappedHandlers;
        private readonly string _root;
        private readonly FileSystemWatcher _watcher;

        public ContentWatcher(string root)
        {

            _ignore = new ConcurrentDictionary<string, bool>();
            _mappedHandlers = new Dictionary<Event, Dictionary<string, List<Action>>>();
            _root = !string.IsNullOrWhiteSpace(root) ? root : throw new ArgumentNullException(nameof(root));
            _watcher = new FileSystemWatcher(_root)
            {
                EnableRaisingEvents = false,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size
            };

            _watcher.Changed += HandleChanged;
            _watcher.Created += HandleCreated;
            _watcher.Deleted += HandleDeleted;
            _watcher.Renamed += HandleRenamed;
        }

        public bool Enabled
        {
            get => _watcher.EnableRaisingEvents;
            set => _watcher.EnableRaisingEvents = value;
        }

        public void AddEventListener(Event watcherEvent, string path, Action handler) =>
            GetHandlersFor(watcherEvent, path, true).Add(handler);

        private Dictionary<string, List<Action>> GetHandlerMapFor(Event watcherEvent, bool createIfMissing = false)
        {
            if (_mappedHandlers.TryGetValue(watcherEvent, out var eventHandlerMap))
            {
                return eventHandlerMap;
            }

            if (!createIfMissing)
            {
                return default;
            }

            eventHandlerMap = new Dictionary<string, List<Action>>();
            _mappedHandlers.Add(watcherEvent, eventHandlerMap);
            return eventHandlerMap;
        }

        private List<Action> GetHandlersFor(Event watcherEvent, string path, bool createIfMissing = false)
        {
            var eventHandlerMap = GetHandlerMapFor(watcherEvent, createIfMissing);
            if (eventHandlerMap == default)
            {
                return default;
            }

            if (eventHandlerMap.TryGetValue(path, out var handlers))
            {
                return handlers;
            }

            if (!createIfMissing)
            {
                return default;
            }

            handlers = new List<Action>();
            eventHandlerMap.Add(path, handlers);
            return handlers;
        }

        public void Modify(string path, Action modificationAction)
        {
            if (modificationAction == default)
            {
                throw new ArgumentNullException(nameof(modificationAction));
            }

            _ = _ignore.TryAdd(path, true);
            modificationAction();
            _ = Task.Delay(1000).ContinueWith(completedTask =>
            {
                _ = _ignore.TryRemove(path, out _);
            }, TaskScheduler.Current);
        }

        private void HandleEvent(object sender, FileSystemEventArgs args, Event watcherEvent)
        {
            var relativePath = args.FullPath.Replace(_root, string.Empty);
            if (relativePath.StartsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                relativePath = relativePath.Substring(1);
            }

            if (_ignore.ContainsKey(relativePath))
            {
                return;
            }

            var handlers = GetHandlersFor(watcherEvent, relativePath);
            if (handlers == default)
            {
                return;
            }

            _ = Task.Delay(1000).ContinueWith(completedTask =>
            {
                foreach (var handler in handlers)
                {
                    handler?.Invoke();
                }
            }, TaskScheduler.Current);
        }

        private void HandleChanged(object sender, FileSystemEventArgs args) => HandleEvent(sender, args, Event.Change);

        private void HandleCreated(object sender, FileSystemEventArgs args) => HandleEvent(sender, args, Event.Create);

        private void HandleDeleted(object sender, FileSystemEventArgs args) => HandleEvent(sender, args, Event.Delete);

        private void HandleRenamed(object sender, RenamedEventArgs args) => HandleEvent(sender, args, Event.Rename);
    }
}
