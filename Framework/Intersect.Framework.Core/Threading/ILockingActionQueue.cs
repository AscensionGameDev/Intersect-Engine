namespace Intersect.Threading;

public interface ILockingActionQueue
{
    Action? NextAction { get; set; }
}