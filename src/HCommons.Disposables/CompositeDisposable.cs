using System.Buffers;
using System.Collections;

namespace HCommons.Disposables;

/// <summary>
/// Represents a collection of disposable resources that can be disposed together.
/// Implements <see cref="ICollection{T}"/> to allow managing multiple <see cref="IDisposable"/> objects.
/// </summary>
public sealed class CompositeDisposable : ICollection<IDisposable>, IDisposable {
    readonly List<IDisposable> disposables;

    /// <summary>
    /// Gets a value indicating whether this instance has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }
    
    /// <summary>
    /// Gets the number of disposables contained in the collection.
    /// </summary>
    public int Count => disposables.Count;
    
    /// <summary>
    /// Gets a value indicating whether the collection is read-only. Always returns false.
    /// </summary>
    public bool IsReadOnly => false;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeDisposable"/> class with the specified capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity of the collection.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when capacity is negative.</exception>
    public CompositeDisposable(int capacity = 0) {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity cannot be negative.");
        
        disposables = new List<IDisposable>(capacity);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeDisposable"/> class with the specified disposables.
    /// </summary>
    /// <param name="disposables">The disposables to add to the collection.</param>
    public CompositeDisposable(params IDisposable[] disposables) {
        this.disposables = new List<IDisposable>(disposables);
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeDisposable"/> class with the specified disposables.
    /// </summary>
    /// <param name="disposables">The disposables to add to the collection.</param>
    public CompositeDisposable(IEnumerable<IDisposable> disposables) {
        this.disposables = new List<IDisposable>(disposables);
    }
    
    /// <summary>
    /// Adds a disposable to the collection. If already disposed, the item is disposed immediately.
    /// </summary>
    /// <param name="item">The disposable to add.</param>
    public void Add(IDisposable item) {
        if (IsDisposed) {
            item.Dispose();
            return;
        }
        disposables.Add(item);
    }
    
    /// <summary>
    /// Removes a disposable from the collection.
    /// </summary>
    /// <param name="item">The disposable to remove.</param>
    /// <returns>true if the item was successfully removed; otherwise, false.</returns>
    public bool Remove(IDisposable item) => disposables.Remove(item);

    /// <summary>
    /// Disposes all disposables in the collection and clears the collection.
    /// If any disposables throw exceptions during disposal, all disposables are still attempted
    /// to be disposed, and an <see cref="AggregateException"/> is thrown containing all exceptions.
    /// </summary>
    /// <exception cref="AggregateException">Thrown when one or more disposables throw during disposal.</exception>
    public void Clear() {
        List<Exception>? exceptions = null;
        
        // Take a snapshot to avoid collection modified exceptions if a disposable
        // attempts to modify this collection during disposal
        var count = disposables.Count;
        var snapshot = ArrayPool<IDisposable>.Shared.Rent(count);
        
        try {
            // Copy items to the rented array using CopyTo for better performance
            disposables.CopyTo(snapshot, 0);
            
            // Clear the collection immediately after snapshotting so that any items
            // added during disposal remain in the collection for future management
            disposables.Clear();
            
            // Dispose all items from the snapshot
            for (var i = 0; i < count; i++) {
                try {
                    snapshot[i].Dispose();
                }
                catch (Exception ex) {
                    exceptions ??= new List<Exception>();
                    exceptions.Add(ex);
                }
            }
        }
        finally {
            ArrayPool<IDisposable>.Shared.Return(snapshot, clearArray: true);
        }
        
        if (exceptions is not null) {
            throw new AggregateException("One or more exceptions occurred during disposal.", exceptions);
        }
    }

    /// <summary>
    /// Disposes all disposables in the collection and marks this instance as disposed.
    /// Subsequent calls have no effect.
    /// If any disposables throw exceptions during disposal, all disposables are still attempted
    /// to be disposed, and an <see cref="AggregateException"/> is thrown containing all exceptions.
    /// </summary>
    /// <exception cref="AggregateException">Thrown when one or more disposables throw during disposal.</exception>
    public void Dispose() {
        if (IsDisposed) return;
        
        try {
            Clear();
        }
        finally {
            IsDisposed = true;
        }
    }

    /// <summary>
    /// Determines whether the collection contains a specific disposable.
    /// </summary>
    /// <param name="item">The disposable to locate.</param>
    /// <returns>true if the item is found; otherwise, false.</returns>
    public bool Contains(IDisposable item) => disposables.Contains(item);
    
    /// <summary>
    /// Copies the disposables to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
    public void CopyTo(IDisposable[] array, int arrayIndex) => disposables.CopyTo(array, arrayIndex);
    
    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    public IEnumerator<IDisposable> GetEnumerator() => disposables.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}