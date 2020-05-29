using UnityEditor;
using UnityEngine;

namespace WraithavenGames.Bones3.Editor
{
    /// <summary>
    /// A simple wand type that fill a region based on a single mouse drag.
    /// </summary>
    public abstract class RegionWand : IWand
    {
        /// <summary>
        /// The selection start point.
        /// </summary>
        public TargetBlock Point1 { get; private set; }

        /// <summary>
        /// The current selection end point.
        /// </summary>
        public TargetBlock Point2 { get; private set; }

        /// <summary>
        /// Whether or not a selection is currently being made.
        /// </summary>
        public bool IsDragging { get; private set; }

        /// <summary>
        /// The active fill pattern for this wand.
        /// </summary>
        public IFillPattern FillPattern { get; private set; }

        /// <inheritdoc/>
        public void SetFillPattern(IFillPattern fillPattern) =>
            FillPattern = fillPattern;

        /// <inheritdoc/>
        public void OnMouseEvent(TargetBlock target, WandEventType eventType)
        {
            if (eventType == WandEventType.ExitWindow)
            {
                IsDragging = false;
                Point1 = default;
                Point2 = default;
            }

            if (eventType == WandEventType.MouseDown)
            {
                IsDragging = true;
                Point1 = target;
                Point2 = target;
            }

            if (eventType == WandEventType.MouseMove)
            {
                if (IsDragging)
                    Point2 = target;
                else
                    Point1 = Point2 = target;
            }

            if (eventType == WandEventType.MouseUp)
            {
                if (IsDragging)
                {
                    Point2 = target;

                    HandleRegion();

                    IsDragging = false;
                    Point1 = default;
                    Point2 = default;
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
        private readonly CuboidFill m_CubeFill = new CuboidFill();
        private readonly FloodFill m_DeleteArea = new FloodFill(1);

        /// <inheritdoc/>
        public override void Render()
        {
            if (BlockWorldEditor.BlockWorld == null)
                return;

            Vector3 p1, p2;

            if (Point2.HasShift)
            {
                p1 = new Vector3(Point1.Over.X, Point1.Over.Y, Point1.Over.Z);
                p2 = new Vector3(Point2.Over.X, Point2.Over.Y, Point2.Over.Z);
            }
            else
            {
                p1 = new Vector3(Point1.Inside.X, Point1.Inside.Y, Point1.Inside.Z);
                p2 = new Vector3(Point2.Inside.X, Point2.Inside.Y, Point2.Inside.Z);
            }

            Vector3 min = Vector3.Min(p1, p2);
            Vector3 max = Vector3.Max(p1, p2) + Vector3.one;

            Vector3 center = (min + max) / 2f;
            Vector3 size = max - min;

            if (Point2.HasShift)
                Handles.color = Color.red;
            else
                Handles.color = Color.cyan;

            Handles.matrix = BlockWorldEditor.BlockWorld.transform.localToWorldMatrix;
            Handles.DrawWireCube(center, size);
        }

        /// <inheritdoc/>
        public override void HandleRegion()
        {
            if (Point2.HasShift)
            {
                m_CubeFill.Set(Point1.Over, Point2.Over, m_DeleteArea);
                BlockWorldEditor.BlockWorld.SetBlocks(m_CubeFill);
            }
            else
            {
                m_CubeFill.Set(Point1.Inside, Point2.Inside, FillPattern);
                BlockWorldEditor.BlockWorld.SetBlocks(m_CubeFill);
            }
        }
    }
}
