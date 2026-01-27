namespace HCommons.Disposables;

/// <summary>
/// A lightweight, struct-based collection for managing disposable resources.
/// Designed for high-performance scenarios with minimal allocations.
/// <para>
/// This type is not fully thread-safe. Multiple threads should not call Add() or Clear() concurrently.
/// However, Dispose() can be safely called multiple times (idempotent) and will dispose each item exactly once.
/// </para>
/// </summary>
public struct DisposableBag : IDisposable {
    IDisposable[]? disposables;
    int isDisposed; // 0 = false, 1 = true (for thread-safe Interlocked operations)
    int count;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableBag"/> struct with the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity of the bag.</param>
    public DisposableBag(int capacity) {
        disposables = new IDisposable[capacity];
    }

    /// <summary>
    /// Adds a disposable to the bag. If already disposed, the item is disposed immediately.
    /// The internal array grows automatically as needed.
    /// </summary>
    /// <param name="item">The disposable to add.</param>
    public void Add(IDisposable item) {
        if (Volatile.Read(ref isDisposed) == 1) {
            item.Dispose();
            return;
        }

        if (disposables == null) {
            disposables = new IDisposable[4];
        }
        else if (count == disposables.Length) {
            Array.Resize(ref disposables, count * 2);
        }

        disposables[count++] = item;
    }

    /// <summary>
    /// Disposes all disposables in the bag and optionally clears the internal array.
    /// </summary>
    /// <param name="keepAllocatedArray">If true, keeps the internal array allocated; otherwise, releases it.</param>
    public void Clear(bool keepAllocatedArray = false) {
        if (disposables == null)
            return;

        if (count == 0 && !keepAllocatedArray) {
            disposables = null;
            return;
        }

        for (var i = 0; i < count; i++) {
            disposables[i].Dispose();
        }

        if (!keepAllocatedArray) {
            disposables = null;
        }

        count = 0;
    }

    /// <summary>
    /// Disposes all disposables in the bag and marks this instance as disposed.
    /// Subsequent calls have no effect. This method is idempotent and safe to call multiple times,
    /// ensuring each item is disposed exactly once even if called concurrently.
    /// </summary>
    public void Dispose() {
        // Use Interlocked.CompareExchange for thread-safe, idempotent disposal
        if (Interlocked.CompareExchange(ref isDisposed, 1, 0) == 0) {
            Clear();
        }
    }
}