using System;
using System.Collections.Concurrent;

namespace WraithavenGames.Bones3
{
    internal class EventQueue
    {
        private readonly BlockingCollection<IBlockWorldEvent> m_Events = new BlockingCollection<IBlockWorldEvent>();

        /// <summary>
        /// Called whenever any events occur within the server.
        /// This is executed on the main thread.
        /// </summary>
        internal event EventHandler<IBlockWorldEvent> OnWorldEvent;

        /// <summary>
        /// Adds an event to this event queue.
        /// </summary>
        internal void AddEvent(IBlockWorldEvent ev) => m_Events.Add(ev);

        /// <summary>
        /// Called from the main thread to run all queued events.
        /// </summary>
        internal void RunEvents()
        {
            while (m_Events.TryTake(out IBlockWorldEvent e))
                OnWorldEvent?.Invoke(this, e);
        }
    }
}
