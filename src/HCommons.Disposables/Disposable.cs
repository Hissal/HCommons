namespace HCommons.Disposables;

/// <summary>
/// Provides factory methods and utilities for creating and managing disposable resources.
/// </summary>
public static class Disposable {
    /// <summary>
    /// Gets an empty disposable that does nothing when disposed.
    /// </summary>
    public static IDisposable Empty { get; } = new EmptyDisposable();

    sealed class EmptyDisposable : IDisposable {
        public void Dispose() { }
    }
    
    /// <summary>
    /// Creates a new <see cref="DisposableBuilder"/> for efficiently building composite disposables.
    /// </summary>
    /// <returns>A new disposable builder instance.</returns>
    public static DisposableBuilder CreateBuilder() => new DisposableBuilder();
    
    /// <summary>
    /// Creates a new <see cref="DisposableBag"/> with the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity of the bag.</param>
    /// <returns>A new disposable bag instance.</returns>
    public static DisposableBag CreateBag(int capacity) => new DisposableBag(capacity);

    /// <summary>
    /// Creates a disposable that invokes the specified action when disposed.
    /// </summary>
    /// <param name="onDispose">The action to invoke on disposal.</param>
    /// <returns>A disposable that executes the action when disposed.</returns>
    public static IDisposable Action(Action onDispose) => new DisposableAction(onDispose);
    
    /// <summary>
    /// Creates a disposable that invokes the specified action with state when disposed.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="state">The state to pass to the action.</param>
    /// <param name="onDispose">The action to invoke on disposal.</param>
    /// <returns>A disposable that executes the action with state when disposed.</returns>
    public static IDisposable Action<TState>(TState state, Action<TState> onDispose) => new DisposableAction<TState>(state, onDispose);
    
    /// <summary>
    /// Creates a new <see cref="DisposableBoolean"/> that tracks its disposal state.
    /// </summary>
    /// <returns>A new boolean disposable instance.</returns>
    public static DisposableBoolean Boolean() => new DisposableBoolean();

    /// <summary>
    /// Combines two disposables into a single disposable that disposes both when disposed.
    /// </summary>
    /// <param name="disposable1">The first disposable.</param>
    /// <param name="disposable2">The second disposable.</param>
    /// <returns>A composite disposable.</returns>
    public static IDisposable Combine(IDisposable? disposable1, IDisposable? disposable2) =>
        new CombinedDisposable2(disposable1, disposable2);

    /// <summary>
    /// Combines three disposables into a single disposable that disposes all when disposed.
    /// </summary>
    /// <param name="disposable1">The first disposable.</param>
    /// <param name="disposable2">The second disposable.</param>
    /// <param name="disposable3">The third disposable.</param>
    /// <returns>A composite disposable.</returns>
    public static IDisposable Combine(IDisposable? disposable1, IDisposable? disposable2, IDisposable? disposable3) =>
        new CombinedDisposable3(disposable1, disposable2, disposable3);

    /// <summary>
    /// Combines four disposables into a single disposable that disposes all when disposed.
    /// </summary>
    /// <param name="disposable1">The first disposable.</param>
    /// <param name="disposable2">The second disposable.</param>
    /// <param name="disposable3">The third disposable.</param>
    /// <param name="disposable4">The fourth disposable.</param>
    /// <returns>A composite disposable.</returns>
    public static IDisposable Combine(IDisposable? disposable1, IDisposable? disposable2, IDisposable? disposable3,
        IDisposable disposable4) =>
        new CombinedDisposable4(disposable1, disposable2, disposable3, disposable4);

    /// <summary>
    /// Combines five disposables into a single disposable that disposes all when disposed.
    /// </summary>
    /// <param name="disposable1">The first disposable.</param>
    /// <param name="disposable2">The second disposable.</param>
    /// <param name="disposable3">The third disposable.</param>
    /// <param name="disposable4">The fourth disposable.</param>
    /// <param name="disposable5">The fifth disposable.</param>
    /// <returns>A composite disposable.</returns>
    public static IDisposable Combine(IDisposable? disposable1, IDisposable? disposable2, IDisposable? disposable3,
        IDisposable? disposable4, IDisposable? disposable5) =>
        new CombinedDisposable5(disposable1, disposable2, disposable3, disposable4, disposable5);

    /// <summary>
    /// Combines six disposables into a single disposable that disposes all when disposed.
    /// </summary>
    /// <param name="disposable1">The first disposable.</param>
    /// <param name="disposable2">The second disposable.</param>
    /// <param name="disposable3">The third disposable.</param>
    /// <param name="disposable4">The fourth disposable.</param>
    /// <param name="disposable5">The fifth disposable.</param>
    /// <param name="disposable6">The sixth disposable.</param>
    /// <returns>A composite disposable.</returns>
    public static IDisposable Combine(IDisposable? disposable1, IDisposable? disposable2, IDisposable? disposable3,
        IDisposable? disposable4, IDisposable? disposable5, IDisposable? disposable6) {
        return new CombinedDisposable6(disposable1, disposable2, disposable3, disposable4, disposable5, disposable6);
    }

    /// <summary>
    /// Combines seven disposables into a single disposable that disposes all when disposed.
    /// </summary>
    /// <param name="disposable1">The first disposable.</param>
    /// <param name="disposable2">The second disposable.</param>
    /// <param name="disposable3">The third disposable.</param>
    /// <param name="disposable4">The fourth disposable.</param>
    /// <param name="disposable5">The fifth disposable.</param>
    /// <param name="disposable6">The sixth disposable.</param>
    /// <param name="disposable7">The seventh disposable.</param>
    /// <returns>A composite disposable.</returns>
    public static IDisposable Combine(IDisposable? disposable1, IDisposable? disposable2, IDisposable? disposable3,
        IDisposable? disposable4, IDisposable? disposable5, IDisposable? disposable6, IDisposable? disposable7) {
        return new CombinedDisposable7(disposable1, disposable2, disposable3, disposable4, disposable5, disposable6,
            disposable7);
    }

    /// <summary>
    /// Combines eight disposables into a single disposable that disposes all when disposed.
    /// </summary>
    /// <param name="disposable1">The first disposable.</param>
    /// <param name="disposable2">The second disposable.</param>
    /// <param name="disposable3">The third disposable.</param>
    /// <param name="disposable4">The fourth disposable.</param>
    /// <param name="disposable5">The fifth disposable.</param>
    /// <param name="disposable6">The sixth disposable.</param>
    /// <param name="disposable7">The seventh disposable.</param>
    /// <param name="disposable8">The eighth disposable.</param>
    /// <returns>A composite disposable.</returns>
    public static IDisposable Combine(IDisposable? disposable1, IDisposable? disposable2, IDisposable? disposable3,
        IDisposable? disposable4, IDisposable? disposable5, IDisposable? disposable6, IDisposable? disposable7,
        IDisposable? disposable8) {
        return new CombinedDisposable8(disposable1, disposable2, disposable3, disposable4, disposable5, disposable6,
            disposable7, disposable8);
    }

    /// <summary>
    /// Combines multiple disposables into a single disposable that disposes all when disposed.
    /// </summary>
    /// <param name="disposables">An array of disposables to combine.</param>
    /// <returns>A composite disposable.</returns>
    public static IDisposable Combine(params IDisposable?[] disposables) =>
        new CombinedDisposable(disposables);

    /// <summary>
    /// Disposes two disposables in sequence.
    /// </summary>
    /// <param name="disposable1">The first disposable to dispose.</param>
    /// <param name="disposable2">The second disposable to dispose.</param>
    public static void Dispose(IDisposable? disposable1, IDisposable? disposable2) {
        List<Exception>? exceptions = null;

        try {
            disposable1?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable2?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        if (exceptions != null) {
            throw new AggregateException(exceptions);
        }
    }

    /// <summary>
    /// Disposes three disposables in sequence.
    /// </summary>
    /// <param name="disposable1">The first disposable to dispose.</param>
    /// <param name="disposable2">The second disposable to dispose.</param>
    /// <param name="disposable3">The third disposable to dispose.</param>
    public static void Dispose(IDisposable? disposable1, IDisposable? disposable2, IDisposable? disposable3) {
        List<Exception>? exceptions = null;

        try {
            disposable1?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable2?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable3?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        if (exceptions != null) {
            throw new AggregateException(exceptions);
        }
    }

    /// <summary>
    /// Disposes four disposables in sequence.
    /// </summary>
    /// <param name="disposable1">The first disposable to dispose.</param>
    /// <param name="disposable2">The second disposable to dispose.</param>
    /// <param name="disposable3">The third disposable to dispose.</param>
    /// <param name="disposable4">The fourth disposable to dispose.</param>
    public static void Dispose(IDisposable? disposable1, IDisposable? disposable2, IDisposable? disposable3,
        IDisposable? disposable4) {
        List<Exception>? exceptions = null;

        try {
            disposable1?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable2?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable3?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable4?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        if (exceptions != null) {
            throw new AggregateException(exceptions);
        }
    }

    /// <summary>
    /// Disposes five disposables in sequence.
    /// </summary>
    /// <param name="disposable1">The first disposable to dispose.</param>
    /// <param name="disposable2">The second disposable to dispose.</param>
    /// <param name="disposable3">The third disposable to dispose.</param>
    /// <param name="disposable4">The fourth disposable to dispose.</param>
    /// <param name="disposable5">The fifth disposable to dispose.</param>
    public static void Dispose(IDisposable? disposable1, IDisposable? disposable2, IDisposable? disposable3,
        IDisposable? disposable4, IDisposable? disposable5) {
        List<Exception>? exceptions = null;

        try {
            disposable1?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable2?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable3?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable4?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable5?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        if (exceptions != null) {
            throw new AggregateException(exceptions);
        }
    }

    /// <summary>
    /// Disposes six disposables in sequence.
    /// </summary>
    /// <param name="disposable1">The first disposable to dispose.</param>
    /// <param name="disposable2">The second disposable to dispose.</param>
    /// <param name="disposable3">The third disposable to dispose.</param>
    /// <param name="disposable4">The fourth disposable to dispose.</param>
    /// <param name="disposable5">The fifth disposable to dispose.</param>
    /// <param name="disposable6">The sixth disposable to dispose.</param>
    public static void Dispose(IDisposable? disposable1, IDisposable? disposable2, IDisposable? disposable3,
        IDisposable? disposable4, IDisposable? disposable5, IDisposable? disposable6) {
        List<Exception>? exceptions = null;

        try {
            disposable1?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable2?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable3?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable4?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable5?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable6?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        if (exceptions != null) {
            throw new AggregateException(exceptions);
        }
    }

    /// <summary>
    /// Disposes seven disposables in sequence.
    /// </summary>
    /// <param name="disposable1">The first disposable to dispose.</param>
    /// <param name="disposable2">The second disposable to dispose.</param>
    /// <param name="disposable3">The third disposable to dispose.</param>
    /// <param name="disposable4">The fourth disposable to dispose.</param>
    /// <param name="disposable5">The fifth disposable to dispose.</param>
    /// <param name="disposable6">The sixth disposable to dispose.</param>
    /// <param name="disposable7">The seventh disposable to dispose.</param>
    public static void Dispose(IDisposable? disposable1, IDisposable? disposable2, IDisposable? disposable3,
        IDisposable? disposable4, IDisposable? disposable5, IDisposable? disposable6, IDisposable? disposable7) {
        List<Exception>? exceptions = null;

        try {
            disposable1?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable2?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable3?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable4?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable5?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable6?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable7?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        if (exceptions != null) {
            throw new AggregateException(exceptions);
        }
    }

    /// <summary>
    /// Disposes eight disposables in sequence.
    /// </summary>
    /// <param name="disposable1">The first disposable to dispose.</param>
    /// <param name="disposable2">The second disposable to dispose.</param>
    /// <param name="disposable3">The third disposable to dispose.</param>
    /// <param name="disposable4">The fourth disposable to dispose.</param>
    /// <param name="disposable5">The fifth disposable to dispose.</param>
    /// <param name="disposable6">The sixth disposable to dispose.</param>
    /// <param name="disposable7">The seventh disposable to dispose.</param>
    /// <param name="disposable8">The eighth disposable to dispose.</param>
    public static void Dispose(IDisposable? disposable1, IDisposable? disposable2, IDisposable? disposable3,
        IDisposable? disposable4, IDisposable? disposable5, IDisposable? disposable6, IDisposable? disposable7,
        IDisposable? disposable8) {
        List<Exception>? exceptions = null;

        try {
            disposable1?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable2?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable3?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable4?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable5?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable6?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable7?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        try {
            disposable8?.Dispose();
        } catch (Exception ex) {
            (exceptions ??= []).Add(ex);
        }

        if (exceptions != null) {
            throw new AggregateException(exceptions);
        }
    }

    /// <summary>
    /// Disposes all disposables in the array in sequence.
    /// </summary>
    /// <param name="disposables">An array of disposables to dispose.</param>
    public static void Dispose(params IDisposable?[] disposables) {
        List<Exception>? exceptions = null;

        foreach (var disposable in disposables) {
            try {
                disposable?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }
        }

        if (exceptions != null) {
            throw new AggregateException(exceptions);
        }
    }

    sealed class CombinedDisposable2(IDisposable? disposable1, IDisposable? disposable2) : IDisposable {
        public void Dispose() {
            List<Exception>? exceptions = null;

            try {
                disposable1?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable2?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            if (exceptions != null) {
                throw new AggregateException(exceptions);
            }
        }
    }

    sealed class CombinedDisposable3(IDisposable? disposable1, IDisposable? disposable2, IDisposable? disposable3)
        : IDisposable 
    {
        public void Dispose() {
            List<Exception>? exceptions = null;

            try {
                disposable1?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable2?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable3?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            if (exceptions != null) {
                throw new AggregateException(exceptions);
            }
        }
    }

    sealed class CombinedDisposable4(
        IDisposable? disposable1,
        IDisposable? disposable2,
        IDisposable? disposable3,
        IDisposable? disposable4)
        : IDisposable 
    {
        public void Dispose() {
            List<Exception>? exceptions = null;

            try {
                disposable1?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable2?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable3?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable4?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            if (exceptions != null) {
                throw new AggregateException(exceptions);
            }
        }
    }

    sealed class CombinedDisposable5(
        IDisposable? disposable1,
        IDisposable? disposable2,
        IDisposable? disposable3,
        IDisposable? disposable4,
        IDisposable? disposable5)
        : IDisposable 
    {
        public void Dispose() {
            List<Exception>? exceptions = null;

            try {
                disposable1?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable2?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable3?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable4?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable5?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            if (exceptions != null) {
                throw new AggregateException(exceptions);
            }
        }
    }

    sealed class CombinedDisposable6(
        IDisposable? disposable1,
        IDisposable? disposable2,
        IDisposable? disposable3,
        IDisposable? disposable4,
        IDisposable? disposable5,
        IDisposable? disposable6)
        : IDisposable 
    {
        public void Dispose() {
            List<Exception>? exceptions = null;

            try {
                disposable1?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable2?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable3?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable4?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable5?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable6?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            if (exceptions != null) {
                throw new AggregateException(exceptions);
            }
        }
    }

    sealed class CombinedDisposable7(
        IDisposable? disposable1,
        IDisposable? disposable2,
        IDisposable? disposable3,
        IDisposable? disposable4,
        IDisposable? disposable5,
        IDisposable? disposable6,
        IDisposable? disposable7)
        : IDisposable 
    {
        public void Dispose() {
            List<Exception>? exceptions = null;

            try {
                disposable1?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable2?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable3?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable4?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable5?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable6?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable7?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            if (exceptions != null) {
                throw new AggregateException(exceptions);
            }
        }
    }

    sealed class CombinedDisposable8(
        IDisposable? disposable1,
        IDisposable? disposable2,
        IDisposable? disposable3,
        IDisposable? disposable4,
        IDisposable? disposable5,
        IDisposable? disposable6,
        IDisposable? disposable7,
        IDisposable? disposable8)
        : IDisposable 
    {
        public void Dispose() {
            List<Exception>? exceptions = null;

            try {
                disposable1?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable2?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable3?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable4?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable5?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable6?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable7?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            try {
                disposable8?.Dispose();
            } catch (Exception ex) {
                (exceptions ??= []).Add(ex);
            }

            if (exceptions != null) {
                throw new AggregateException(exceptions);
            }
        }
    }

    sealed class CombinedDisposable(IDisposable?[] disposables) : IDisposable {
        public void Dispose() {
            List<Exception>? exceptions = null;

            foreach (var disposable in disposables) {
                try {
                    disposable?.Dispose();
                } catch (Exception ex) {
                    (exceptions ??= []).Add(ex);
                }
            }

            if (exceptions != null) {
                throw new AggregateException(exceptions);
            }
        }
    }

    sealed class DisposableAction(Action onDispose) : IDisposable {
        Action? onDispose = onDispose;

        public void Dispose() {
            Interlocked.Exchange(ref onDispose, null)?.Invoke();
        }
    }

    sealed class DisposableAction<TState>(TState state, Action<TState> onDispose) : IDisposable {
        Action<TState>? onDispose = onDispose;
        TState state = state;

        public void Dispose() {
            Interlocked.Exchange(ref onDispose, null)?.Invoke(state);
            state = default!;
        }
    }
}