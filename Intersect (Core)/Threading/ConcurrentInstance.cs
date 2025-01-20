using System.Diagnostics;
using Intersect.Core;
using Microsoft.Extensions.Logging;

namespace Intersect.Threading;


public partial class ConcurrentInstance<TInstance> where TInstance : class
{

    private readonly object mLock;

    private TInstance mInstance;

    public ConcurrentInstance()
    {
        mLock = new object();
    }

    public bool HasInstance => mInstance != null;

    public TInstance Instance => mInstance ?? throw new InvalidOperationException();

    public void ClearWith(TInstance instance, Action action)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        ApplicationContext.Context.Value?.Logger.LogInformation($@"Acquiring context lock... ({stopwatch.ElapsedMilliseconds}ms)");
        Acquire();
        ApplicationContext.Context.Value?.Logger.LogInformation($@"Acquired. ({stopwatch.ElapsedMilliseconds}ms)");

        if (mInstance != instance)
        {
            ApplicationContext.Context.Value?.Logger.LogInformation($@"Exiting lock... ({stopwatch.ElapsedMilliseconds}ms)");
            Monitor.Exit(mLock);
        }

        action.Invoke();

        ApplicationContext.Context.Value?.Logger.LogInformation($@"Clearing instance... ({stopwatch.ElapsedMilliseconds}ms)");
        Clear(instance);

        ApplicationContext.Context.Value?.Logger.LogInformation($@"Releasing context lock... ({stopwatch.ElapsedMilliseconds}ms)");
        Release();
        ApplicationContext.Context.Value?.Logger.LogInformation($@"Released. ({stopwatch.ElapsedMilliseconds}ms)");
    }

    public void Acquire()
    {
        Monitor.Enter(mLock);
    }

    public void Release()
    {
        if (!Monitor.IsEntered(mLock))
        {
            return;
        }

        Monitor.Pulse(mLock);
        Monitor.Exit(mLock);
    }

    public void Set(TInstance instance)
    {
        if (!Monitor.TryEnter(mLock, 1000))
        {
            throw new InvalidOperationException();
        }

        try
        {
            if (mInstance != null)
            {
                Monitor.Wait(mLock);
            }

            mInstance = instance;
        }
        finally
        {
            Release();
        }
    }

    public void Clear(TInstance instance)
    {
        if (mInstance == instance)
        {
            mInstance = null;
        }
    }

    public static implicit operator TInstance(ConcurrentInstance<TInstance> concurrentInstance)
    {
        return concurrentInstance.mInstance;
    }

}
