using System.Collections.Generic;

using Bones3Rebuilt;

using UnityEditor;

using UnityEngine;

namespace WraithavenGames.Bones3
{
    /// <summary>
    /// A utility class for rendering a block preview.
    /// </summary>
    public class BlockTypesPreview : System.IDisposable
    {
        private readonly PreviewRenderUtility previewUtility = new PreviewRenderUtility();
        private readonly Mesh mesh = new Mesh();
        private readonly BlockTypeList blockTypes;
        private readonly RemeshHandler remeshHandler;

        private Vector2 previewDir = new Vector2(45f, -30f);
        private Material[] materials = new Material[0];
        private int materialIndexPass;

        /// <summary>
        /// Creates a new block type preview object.
        /// </summary>
        /// <param name="blockTypes">The block type list to reference when rendering.</param>
        public BlockTypesPreview(BlockTypeList blockTypes)
        {
            this.blockTypes = blockTypes;

            EditorUtility.SetCameraAnimateMaterials(previewUtility.camera, true);

            remeshHandler = new RemeshHandler();
            remeshHandler.AddDistributor(new StandardDistributor());
            remeshHandler.OnRemeshFinish += OnRemeshFinish;
        }

        /// <summary>
        /// Cleans up this preview object.
        /// </summary>
        public void Dispose()
        {
            previewUtility.Cleanup();
            Object.DestroyImmediate(mesh);
        }

        /// <summary>
        /// Renders the preview.
        /// </summary>
        /// <param name="r">The display rect.</param>
        /// <param name="background">The background rendering style.</param>
        public void Render(Rect r, GUIStyle background)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                if (Event.current.type == EventType.Repaint)
                    EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40), "Unable to render\nblock preview");
                return;
            }

            UpdatePreviewDirection(r);

            if (Event.current.type != EventType.Repaint)
                return;

            previewUtility.BeginPreview(r, background);

            DoRenderPreview();

            previewUtility.EndAndDrawPreview(r);
        }

        /// <summary>
        /// Rotates the preview direction based on mouse inputs.
        /// </summary>
        /// <param name="position">The display rect.</param>
        /// <returns></returns>
        void UpdatePreviewDirection(Rect position)
        {
            int sliderHash = "Slider".GetHashCode();
            int id = GUIUtility.GetControlID(sliderHash, FocusType.Passive);
            Event evt = Event.current;

            switch (evt.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (position.Contains(evt.mousePosition) && position.width > 50)
                    {
                        GUIUtility.hotControl = id;
                        evt.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        previewDir -= evt.delta * (evt.shift ? 3 : 1) / Mathf.Min(position.width, position.height) * 140.0f;
                        evt.Use();
                        GUI.changed = true;
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                        GUIUtility.hotControl = 0;

                    EditorGUIUtility.SetWantsMouseJumping(0);
                    break;
            }
        }

        void DoRenderPreview()
        {
            previewUtility.camera.transform.position = -Vector3.forward * 7f;
            previewUtility.camera.transform.rotation = Quaternion.identity;

            previewUtility.lights[0].intensity = 1.0f;
            previewUtility.lights[0].transform.rotation = Quaternion.Euler(50f, 50f, 0f);
            previewUtility.lights[1].intensity = 1.0f;

            previewUtility.ambientColor = new Color(0.2f, 0.2f, 0.2f, 0f);

            Quaternion rot = Quaternion.Euler(previewDir.y, 0, 0) * Quaternion.Euler(0, previewDir.x, 0);

            previewUtility.camera.transform.position = Quaternion.Inverse(rot) * previewUtility.camera.transform.position;
            previewUtility.camera.transform.LookAt(Vector3.zero);

            for (int i = 0; i < materials.Length; i++)
                previewUtility.DrawMesh(mesh, Vector3.one * -0.5f, Quaternion.identity, materials[i], i);

            previewUtility.Render(true);
        }

        private void OnRemeshFinish(RemeshFinishEvent ev)
        {
            var report = ev.Report;
            var visual = report.VisualMesh;

            var meshBuilder = new ChunkMeshBuilder();
            meshBuilder.UpdateMesh(visual, mesh);

            materials = new Material[visual.TotalLayers];
            for (int i = 0; i < materials.Length; i++)
                materials[i] = null;
        }

        public void RebuildMesh(BlockType blockType)
        {
            mesh.Clear();

            if (!blockType.IsVisible)
                return;

            var properties = new SingleBlockHolder(blockType, blockTypes.GetBlockType(0));
            remeshHandler.RemeshChunk(properties);
            remeshHandler.FinishTasks();
        }

        /// <summary>
        /// A simple utility which acts as a chunk properties object for a chunk of 1x1x1.
        /// </summary>
        class SingleBlockHolder : IChunkProperties
        {
            private readonly BlockType m_BlockType;
            private readonly BlockType m_Ungenerated;

            /// <inheritdoc cref="IChunkProperties"/>
            public GridSize ChunkSize => default;

            /// <inheritdoc cref="IChunkProperties"/>
            public ChunkPosition ChunkPosition => default;

            /// <inheritdoc cref="IChunkProperties"/>
            public BlockType GetBlock(BlockPosition pos) => m_BlockType;

            /// <inheritdoc cref="IChunkProperties"/>
            public BlockType GetNextBlock(BlockPosition pos, int side) => m_Ungenerated;

            public SingleBlockHolder(BlockType blockType, BlockType ungenerated)
            {
                m_BlockType = blockType;
                m_Ungenerated = ungenerated;
            }
        }
    }
}
