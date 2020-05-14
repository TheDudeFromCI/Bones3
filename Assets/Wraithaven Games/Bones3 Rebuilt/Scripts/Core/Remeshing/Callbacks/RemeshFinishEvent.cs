namespace Bones3Rebuilt
{
    /// <summary>
    /// Called when a chunk remesh task has finished.
    /// </summary>
    public class RemeshFinishEvent
    {
        /// <summary>
        /// Gets the output of the remesh tasks.
        /// </summary>
        /// <value>The output meshes.</value>
        public RemeshReport Report { get; }

        /// <summary>
        /// Creates a new remesh event.
        /// </summary>
        /// <param name="remeshReport">The remesh task report.</param>
        public RemeshFinishEvent(RemeshReport remeshReport)
        {
            Report = remeshReport;
        }
    }

    /// <summary>
    /// A callback for remesh finish events.
    /// </summary>
    /// <param name="ev">The event.</param>
    public delegate void RemeshFinishCallback(RemeshFinishEvent ev);
}