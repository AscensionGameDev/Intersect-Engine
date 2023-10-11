namespace Intersect.OpenPortChecker;

internal record struct PendingRequest(Guid Id, TaskCompletionSource<string?> TaskCompletionSource);