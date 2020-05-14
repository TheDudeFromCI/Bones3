using System.Collections.Generic;

namespace Bones3Rebuilt
{
    /// <summary>
    /// Contains a set of procedural meshes representing submesh layers within a single mesh.
    /// </summary>
    public class LayeredProcMesh
    {
        private readonly List<ProcMesh> m_Layers = new List<ProcMesh>();

        /// <summary>
        /// Gets the number of non-empty layers in this mesh, based on triangle count.
        /// </summary>
        /// <value>The active layer count.</value>
        public int ActiveLayers
        {
            get
            {
                int layers = 0;
                foreach (var m in m_Layers)
                    if (m.HasTriangles)
                        layers++;

                return layers;
            }
        }

        /// <summary>
        /// Gets the total number of layers in this mesh.
        /// </summary>
        /// <value>The number of layers, including empty layers.</value>
        public int TotalLayers { get => m_Layers.Count; }

        /// <summary>
        /// Clears all data in this proc mesh.
        /// </summary>
        public void Clear()
        {
            foreach (var mesh in m_Layers)
                mesh.Clear();
        }

        /// <summary>
        /// Gets the proc mesh for the given layer, allocating new layers as needed.
        /// </summary>
        /// <param name="layer">The layer to retrieve.</param>
        /// <returns>The target procmesh layer.</returns>
        public ProcMesh GetLayer(int layer)
        {
            while (m_Layers.Count <= layer)
                m_Layers.Add(new ProcMesh());

            return m_Layers[layer];
        }
    }
}
