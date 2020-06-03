using UnityEngine;

namespace WraithavenGames.Bones3
{
    [AddComponentMenu("Bones3/Voxel Render Distance")]
    [ExecuteAlways, DisallowMultipleComponent, RequireComponent(typeof(BlockWorld))]
    public class VoxelRenderDistance : MonoBehaviour
    {
        [Tooltip("The camera to load chunks around. Defaults to main camera if not assigned.")]
        [SerializeField] protected Camera m_Camera;

        [Tooltip("The number of chunks to load along each axis. (Radius)")]
        [SerializeField] protected Vector3Int m_ViewDistance = new Vector3Int(10, 5, 10);

        [Tooltip("The order in which chunks are loaded.")]
        [SerializeField] protected ChunkLoadingPattern m_ChunkLoadingPattern;

        [Tooltip("The number of chunks to check loading per frame.")]
        [SerializeField, Range(5, 50)] protected int m_ChecksPerFrame = 20;

        [Tooltip("Whether or not to force chunks directly touching the camera to be loaded immediately.")]
        [SerializeField] protected bool m_EmergencyChunkLoading = true;

        private BlockWorld m_BlockWorld;
        private ChunkPosition m_LastCameraPosition;

        /// <summary>
        /// Gets the loading pattern currently being used.
        /// </summary>
        internal ChunkLoadPatternIterator LoadPatternIterator { get; private set; }

        /// <summary>
        /// Called when the behaviour is initialized to load the block world
        /// reference and set up the chunk loading pattern.
        /// </summary>
        protected void Awake()
        {
            m_BlockWorld = GetComponent<BlockWorld>();
            UpdatePatternIterator();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Called when the world is enabled to subscribe to editor frame updates
        /// and initialize.
        /// </summary>
        protected void OnEnable()
        {
            if (!Application.isPlaying)
                UnityEditor.EditorApplication.update += Update;

            LoadPatternIterator.Reset();
        }

        /// <summary>
        /// Called when the world is disabled to unsubscribe from editor frame updates.
        /// </summary>
        protected void OnDisable()
        {
            if (!Application.isPlaying)
                UnityEditor.EditorApplication.update -= Update;
        }

        /// <summary>
        /// Called whenever the behaviour properties are updated to reset the
        /// chunk loading pattern.
        /// </summary>
        protected void OnValidate()
        {
            UpdatePatternIterator();
        }
#endif

        /// <summary>
        /// Creates a new chunk loading pattern iterator based on the current properties.
        /// </summary>
        private void UpdatePatternIterator()
        {
            LoadPatternIterator = ChunkLoadPatternIterator.Build(m_ChunkLoadingPattern, m_ViewDistance);
        }

        /// <summary>
        /// Updates the camera pointer in case it becomes null.
        /// </summary>
        /// <returns>True if the camera is ready, false otherwise.</returns>
        private bool UpdateCameraReference()
        {
            if (m_Camera != null)
                return true;

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                if (m_Camera == null)
                    m_Camera = Camera.main;
            }
            else
                m_Camera = UnityEditor.SceneView.GetAllSceneCameras()?[0];
#else
            if (m_Camera == null)
                m_Camera = Camera.main;
#endif

            return m_Camera != null;
        }

        /// <summary>
        /// Called each frame to trigger chunks to start loading and unloading.
        /// </summary>
        protected void Update()
        {
            if (!UpdateCameraReference())
                return;

            UpdateCameraPosition();

            if (CheckEmergencyChunks())
                return; // No need to cause extra processing this frame

            LoadNearbyChunks();
            UnloadDistantChunks();
        }

        /// <summary>
        /// Forces the chunks directly around the camera to be loaded instantly.
        /// </summary>
        private bool CheckEmergencyChunks()
        {
            if (!m_EmergencyChunkLoading)
                return false;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                return false;
#endif

            return m_BlockWorld.LoadChunkRegion(m_LastCameraPosition, Vector3Int.one);
        }

        /// <summary>
        /// Checks for changes in the camera's chunk position and updates the field
        /// and resets the iterator as needed.
        /// </summary>
        private void UpdateCameraPosition()
        {
            var pos = m_Camera.transform.position;
            var chunkPos = new ChunkPosition
            {
                X = Mathf.FloorToInt(pos.x) >> m_BlockWorld.ChunkSize.IntBits,
                Y = Mathf.FloorToInt(pos.y) >> m_BlockWorld.ChunkSize.IntBits,
                Z = Mathf.FloorToInt(pos.z) >> m_BlockWorld.ChunkSize.IntBits,
            };

            if (!chunkPos.Equals(m_LastCameraPosition))
            {
                m_LastCameraPosition = chunkPos;
                LoadPatternIterator.Reset();
            }
        }

        /// <summary>
        /// Loads the next few chunks as indicated by the chunk load order.
        /// </summary>
        private void LoadNearbyChunks()
        {
            if (m_BlockWorld.ActiveChunkLoadingTasks > 0 ||
                m_BlockWorld.ActiveRemeshingTasks > 0)
                return;

            for (int i = 0; i < m_ChecksPerFrame; /*i++*/)
            {
                if (!LoadPatternIterator.HasNext)
                    return;

                var pos = LoadPatternIterator.Next;
                m_BlockWorld.LoadChunkAsync(pos + m_LastCameraPosition);

                // TODO Write server task for loading chunks async
                return;
            }
        }

        /// <summary>
        /// Unloads any chunks which are outside of the render distance + 1.
        /// </summary>
        private void UnloadDistantChunks()
        {
            // TODO Unload distant chunks
        }

        /// <summary>
        /// Called when all world data is cleared. Should reset the iterator.
        /// </summary>
        protected void OnWorldClear()
        {
            LoadPatternIterator.Reset();
        }
    }
}
