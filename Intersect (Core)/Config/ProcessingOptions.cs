using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Config
{
    /// <summary>
    /// Thread count and timing options for game processing.
    /// </summary>
    public class ProcessingOptions
    {
        /// <summary>
        /// Determined whether or not the server logic thread sleeps after each sloop by default
        /// </summary>
        public bool CpsLock { get; set; } = true;

        /// <summary>
        /// Minimum of threads that will be created process map logic, npc/event updates, hots/dots/status effects, projectiles and more.
        /// </summary>
        public int MinLogicThreads { get; set; } = 2;

        /// <summary>
        /// Maximum number threads that will be created to handle map logic, npc/event updates, hots/dots/status effects, projectiles while the server is under load.
        /// </summary>
        public int MaxLogicThreads { get; set; } = 4;

        /// <summary>
        /// This is how long (in ms) a logic thread in our logic pool should be idle before it is considered unneeded and therefore disposed.
        /// </summary>
        public int LogicThreadIdleTimeout { get; set; } = 20000;

        /// <summary>
        /// Minimum number of threads that will be used for packet processing which we don't want to slow down our game loop.
        /// </summary>
        public int MinNetworkThreads { get; set; } = 2;

        /// <summary>
        /// Maximum number of threads that will be used for packet processing which we don't want to slow down our game loop.
        /// </summary>
        public int MaxNetworkThreads { get; set; } = 4;

        /// <summary>
        /// This is how long (in ms) a network thread in our network pool should be idle before it is considered unneeded and therefore disposed.
        /// </summary>
        public int NetworkThreadIdleTimeout { get; set; } = 20000;

        /// <summary>
        /// Minimum number of threads that will be used for database interactions like player or variable saving
        /// </summary>
        public int MinDatabaseThreads { get; set; } = 2;
        /// <summary>
        /// Maximum number of threads that will be used for database interactions like player or variable saving
        /// </summary>
        public int MaxDatabaseThreads { get; set; } = 4;

        /// <summary>
        /// This is how long (in ms) a database thread should be idle before it is considered unneeded and therefore disposed.
        /// </summary>
        public int DatabaseThreadIdleTimeout { get; set; } = 20000;

        /// <summary>
        /// How often should the server save changes to server variable values?
        /// </summary>
        public int DatabaseSaveServerVariablesInterval { get; set; } = 60000;

        /// <summary>
        /// Minimum number of worker threads that will be used in the system managed threadpool (-1 for default)
        /// </summary>
        public int MinSystemThreadpoolWorkerThreads { get; set; } = -1;
        /// <summary>
        /// Maximum number of worker threads that will be used in the system managed threadpool (-1 for default)
        /// </summary>
        public int MaxSystemThreadpoolWorkerThreads { get; set; } = -1;
        /// <summary>
        /// Minimum number of worker threads that will be used for io the system managed threadpool (-1 for default)
        /// </summary>
        public int MinSystemThreadpoolIOThreads { get; set; } = -1;
        /// <summary>
        /// Maximum number of worker threads that will be used for io the system managed threadpool (-1 for default)
        /// </summary>
        public int MaxSystemThreadpoolIOThreads { get; set; } = -1;

        /// <summary>
        /// This controls how often maps/npcs should be updated in ms.
        /// </summary>
        public int MapUpdateInterval { get; set; } = 50;

        /// <summary>
        /// This controls how often projectiles should updated in ms. (It is recommended to keep this the same value as the MapUpdateInterval so they are all updated at the same time)
        /// </summary>
        public int ProjectileUpdateInterval { get; set; } = 50;

        /// <summary>
        /// How long to delay between player saves (default 2 min)
        /// </summary>
        public int PlayerSaveInterval { get; set; } = 120000;

        /// <summary>
        /// How often should the server try to execute autorun common events for a player? This can get expensive if you have a lot of autorun common events with lots of complex conditions.
        /// </summary>
        public int CommonEventAutorunStartInterval { get; set; } = 500;

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Validate();
        }

        public void Validate()
        {
            if (MinLogicThreads < 1)
            {
                throw new InvalidOperationException("Need at least 1 logic thread.");
            }

            if (MaxLogicThreads < MinLogicThreads)
            {
                throw new InvalidOperationException("The maximum number of logic threads should be greater than the minimum number of logic threads.");
            }

            if (LogicThreadIdleTimeout < 1000)
            {
                throw new Exception("Logic thread idle timeout is too low, should be above 1000ms else you may run into significant overhead due to threads being created/destroyed too often.");
            }

            if (MinNetworkThreads < 1)
            {
                throw new InvalidOperationException("Need at least 1 network thread.");
            }

            if (MaxNetworkThreads < MinNetworkThreads)
            {
                throw new InvalidOperationException("The maximum number of network threads should be greater than the minimum number of network threads.");
            }

            if (NetworkThreadIdleTimeout < 1000)
            {
                throw new Exception("Network thread idle timeout is too low, should be above 1000ms else you may run into significant overhead due to threads being created/destroyed too often.");
            }

            if (MinDatabaseThreads < 1)
            {
                throw new InvalidOperationException("Need at least 1 database io thread.");
            }

            if (MaxDatabaseThreads < MinDatabaseThreads)
            {
                throw new InvalidOperationException("The maximum number of database io threads should be greater than the minimum number of database io threads.");
            }

            if (DatabaseThreadIdleTimeout < 1000)
            {
                throw new Exception("Database thread idle timeout is too low, should be above 1000ms else you may run into significant overhead due to threads being created/destroyed too often.");
            }

            if (DatabaseSaveServerVariablesInterval < 5000)
            {
                throw new Exception("Server variables should not be saved more often than once per every 5 seconds, extra db writes will hinder performance.");
            }

            if (MapUpdateInterval > 200)
            {
                throw new InvalidOperationException("Map update interval is too high to provide a smooth gaming experience.");
            }

            if (PlayerSaveInterval < 20000)
            {
                throw new InvalidOperationException("Player save interval is too low and would cause performance issues, consider raising.");
            }
        }
    }
}
