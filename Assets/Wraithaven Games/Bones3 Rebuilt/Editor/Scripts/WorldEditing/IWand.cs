namespace Bones3Rebuilt
{
    /// <summary>
    /// A simple interface for determining how a specific world edit tool should be used.
    /// </summary>
    public interface IWand
    {
        /// <summary>
        /// Called when the user switches the target world to edit.
        /// </summary>
        /// <param name="world">The world being edited, or null if no target is selected.</param>
        void SetWorld(BlockWorld world);

        /// <summary>
        /// Called when a mouse event is executed.
        /// </summary>
        /// <param name="target">The target block.</param>
        /// <param name="eventType">The type of event being called.</param>
        void OnMouseEvent(TargetBlock target, WandEventType eventType);

        /// <summary>
        /// Called to render the gizmos for this wands.
        /// </summary>
        void Render();

        /// <summary>
        /// Sets the fill pattern which should be used for this wand.
        /// </summary>
        /// <param name="fillPattern">The fill pattern.</param>
        void SetFillPattern(IFillPattern fillPattern);
    }
}
