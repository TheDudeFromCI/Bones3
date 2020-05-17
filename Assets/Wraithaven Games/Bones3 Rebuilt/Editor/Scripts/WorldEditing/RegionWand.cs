using Bones3Rebuilt;

using UnityEditor;

using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A simple wand type that fill a region based on a single mouse drag.
    /// </summary>
    public abstract class RegionWand : IWand
    {
        private BlockWorld world;
        private IFillPattern fillPattern;
        private TargetBlock point1;
        private TargetBlock point2;
        private bool dragging;

        /// <summary>
        /// The world currently being handled.
        /// </summary>
        /// <value>The world.</value>
        public BlockWorld World { get => world; }

        /// <summary>
        /// The selection start point.
        /// </summary>
        /// <value>One corner of the selection region.</value>
        public TargetBlock Point1 { get => point1; }

        /// <summary>
        /// The current selection end point.
        /// </summary>
        /// <value>The opposite corner of the selection region.</value>
        public TargetBlock Point2 { get => point2; }

        /// <summary>
        /// Whether or not a selection is currently being made.
        /// </summary>
        /// <value>True if a selection is currently being made, false otherwise.</value>
        public bool IsDragging { get => dragging; }

        /// <summary>
        /// The active fill pattern for this wand.
        /// </summary>
        /// <value>The fill pattern.</value>
        public IFillPattern FillPattern { get => fillPattern; }

        /// <inheritdoc/>
        public void SetWorld(BlockWorld world)
        {
            this.world = world;
            point1 = default;
            point2 = default;
            dragging = false;
        }

        /// <inheritdoc/>
        public void SetFillPattern(IFillPattern fillPattern) =>
            this.fillPattern = fillPattern;

        /// <inheritdoc/>
        public void OnMouseEvent(TargetBlock target, WandEventType eventType)
        {
            if (eventType == WandEventType.ExitWindow)
            {
                dragging = false;
                point1 = default;
                point2 = default;
                return;
            }

            if (eventType == WandEventType.MouseDown)
            {
                if (World != null)
                {
                    dragging = true;
                    point1 = target;
                    point2 = target;
                }

                return;
            }

            if (eventType == WandEventType.MouseMove)
            {
                if (dragging)
                    point2 = target;
                else
                    point1 = point2 = target;

                return;
            }

            if (eventType == WandEventType.MouseUp)
            {
                if (dragging)
                {
                    point2 = target;

                    HandleRegion();

                    dragging = false;
                    point1 = default;
                    point2 = default;
                }
            }
        }

        /// <inheritdoc/>
        public abstract void Render();

        /// <summary>
        /// Called when the user released the mouse button with a region selected.
        /// </summary>
        public abstract void HandleRegion();
    }

    /// <summary>
    /// Fills the selected region with a given pattern.
    /// </summary>
    public class FillRegion : RegionWand
    {
        private readonly CuboidFill cubeFill = new CuboidFill();

        /// <inheritdoc/>
        public override void Render()
        {
            Vector3 p1 = new Vector3(Point1.Inside.X, Point1.Inside.Y, Point1.Inside.Z);
            Vector3 p2 = new Vector3(Point2.Inside.X, Point2.Inside.Y, Point2.Inside.Z);

            Vector3 min = Vector3.Min(p1, p2);
            Vector3 max = Vector3.Max(p1, p2) + Vector3.one;

            Vector3 center = (min + max) / 2f;
            Vector3 size = max - min;

            Handles.color = Color.cyan;
            Handles.matrix = World.transform.localToWorldMatrix;
            Handles.DrawWireCube(center, size);
        }

        /// <inheritdoc/>
        public override void HandleRegion()
        {
            cubeFill.Set(Point1.Inside, Point2.Inside, FillPattern);
            World.WorldContainer.SetBlocks(cubeFill);
        }
    }
}
