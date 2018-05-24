using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Daenet.DurableTaskMicroservices.Core
{
    /// <summary>
    /// Implements observer of events traced by DTF framework internally.
    /// </summary>
    internal sealed class TraceEventReceiver : IObserver<EventEntry>, IDisposable
    {
        private int onCompletedCalls;
        private int onErrorCalls;
        private ConcurrentBag<EventEntry> onNextCalls = new ConcurrentBag<EventEntry>();
        private Action<string> OnEvent;
        private object m_Filter;

        /// <summary>
        /// Creates and configures th einstance of receiver.
        /// </summary>
        /// <param name="onEvent">Action to be invoked when event is observed.</param>
        /// <param name="filter">Currently still in design. Waiting on final DTF tracing concept.
        /// If you pass a string "errors", all errors will be passed to specified action.
        /// If null value is specified, then all kind of events will be passed to specified action.
        /// </param>
        public TraceEventReceiver(Action<string> onEvent, object filter)
        {
            this.OnEvent = onEvent;
            this.m_Filter = filter;
        }

        public int OnCompletedCalls { get { return this.onCompletedCalls; } }
        public int OnErrorCalls { get { return this.onErrorCalls; } }
        public IEnumerable<EventEntry> OnNextCalls { get { return this.onNextCalls; } }
        public bool DisposeCalled { get; private set; }

        void IObserver<EventEntry>.OnCompleted()
        {
            Interlocked.Increment(ref this.onCompletedCalls);
        }

        void IObserver<EventEntry>.OnError(Exception error)
        {
            Interlocked.Increment(ref this.onErrorCalls);
        }

        void IObserver<EventEntry>.OnNext(EventEntry value)
        {
            this.onNextCalls.Add(value);

            formatPayload(value);
        }

        public void Dispose()
        {
            this.DisposeCalled = true;
        }

        /// <summary>
        /// Filters required events.
        /// </summary>
        /// <param name="entry"></param>
        private void formatPayload(EventEntry entry)
        {
            var eventSchema = entry.Schema;
            var sb = new StringBuilder();
            for (int i = 0; i < entry.Payload.Count; i++)
            {
                // Any errors will be handled in the sink.
                sb.AppendFormat(" [{0} : {1}]", eventSchema.Payload[i], entry.Payload[i]);

                if (OnEvent != null)
                {
                    string filter = this.m_Filter as string;
                    if (filter != null && filter == "errors")
                    {
                        if (eventSchema.Payload[i] == "exception")
                        {
                            this.OnEvent(sb.ToString());
                        }
                    }
                    else
                        this.OnEvent(sb.ToString());
                }
            }
        }

    }
}
