using System.Diagnostics;

namespace HCommons.Reflection;

public static partial class RuntimeTypeCache {
    sealed class Binding : IDisposable {
        readonly object _gate = new();
        readonly Action<IReadOnlyList<Type>> _onChanged;
        readonly SynchronizationContext? _synchronizationContext;

        IReadOnlyList<Type>? _pendingSnapshot;
        bool _started;
        bool _deliveryInProgress;
        bool _dispatchScheduled;
        bool _disposed;

        public Binding(
            Type baseType,
            Action<IReadOnlyList<Type>> onChanged,
            SynchronizationContext? synchronizationContext) {
            BaseType = baseType;
            _onChanged = onChanged;
            _synchronizationContext = synchronizationContext;
        }

        public Type BaseType { get; }

        public void Dispose() {
            lock (_gate) {
                if (_disposed) {
                    return;
                }

                _disposed = true;
                _pendingSnapshot = null;
            }

            Unbind(this);
        }

        public void Start(IReadOnlyList<Type> snapshot) {
            IReadOnlyList<Type>? snapshotToDeliver;

            lock (_gate) {
                if (_disposed) {
                    return;
                }

                _started = true;
                _pendingSnapshot ??= snapshot;
                snapshotToDeliver = _pendingSnapshot;
                _pendingSnapshot = null;
                _deliveryInProgress = true;
            }

            Deliver(snapshotToDeliver);
        }

        public void Queue(IReadOnlyList<Type> snapshot) {
            var shouldSchedule = false;

            lock (_gate) {
                if (_disposed) {
                    return;
                }

                _pendingSnapshot = snapshot;

                if (_started && !_deliveryInProgress && !_dispatchScheduled) {
                    _dispatchScheduled = true;
                    shouldSchedule = true;
                }
            }

            if (shouldSchedule) {
                ScheduleDispatch();
            }
        }

        void ScheduleDispatch() {
            try {
                if (_synchronizationContext is not null) {
                    _synchronizationContext.Post(_ => Dispatch(), null);
                    return;
                }

                if (ThreadPool.QueueUserWorkItem(_ => Dispatch())) {
                    return;
                }

                Trace.TraceWarning("Unable to queue a runtime type cache binding callback.");
            }
            catch (Exception exception) when (exception is not OutOfMemoryException) {
                Trace.TraceWarning("Unable to queue a runtime type cache binding callback: {0}", exception);
            }

            lock (_gate) {
                _dispatchScheduled = false;
            }
        }

        void Dispatch() {
            IReadOnlyList<Type>? snapshot;

            lock (_gate) {
                _dispatchScheduled = false;

                if (_disposed || _pendingSnapshot is null) {
                    return;
                }

                snapshot = _pendingSnapshot;
                _pendingSnapshot = null;
                _deliveryInProgress = true;
            }

            Deliver(snapshot);
        }

        void Deliver(IReadOnlyList<Type> snapshot) {
            try {
                _onChanged(snapshot);
            }
            catch (Exception exception) when (exception is not OutOfMemoryException) {
                Trace.TraceWarning("A runtime type cache binding callback failed: {0}", exception);
            }
            finally {
                CompleteDelivery();
            }
        }

        void CompleteDelivery() {
            var shouldSchedule = false;

            lock (_gate) {
                _deliveryInProgress = false;

                if (!_disposed && _started && _pendingSnapshot is not null && !_dispatchScheduled) {
                    _dispatchScheduled = true;
                    shouldSchedule = true;
                }
            }

            if (shouldSchedule) {
                ScheduleDispatch();
            }
        }
    }
}
