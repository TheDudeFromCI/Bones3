using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using WraithavenGames.Bones3.Filter;

namespace WraithavenGames.Bones3.SceneEditing
{
    public class ToolBelt : ScriptableObject
    {
        // Modes
        public const int NORMAL_MODE = 0;
        public const int PAINT_MODE = 1;
        public const int DO_NOTHING_MODE = 2;

        // Shapes
        public const int BOX_SHAPE = 0;
        public const int WALL_SHAPE = 1;
		public const int ELLIPSE_SHAPE = 2;

        private static ToolBelt m_Instance;
        public static ToolBelt Instance
        {
            get
            {
                if (m_Instance == null)
                {
#if UNITY_EDITOR
                    // Find toolbelt properties location
                    string guid = AssetDatabase.FindAssets("ToolBeltTemp")[0];
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    path = new Regex("ToolBelt$").Replace(path, "ToolsState.asset");

                    // Load toolbelt properties
                    m_Instance = AssetDatabase.LoadAssetAtPath(path, typeof(ToolBelt)) as ToolBelt;

                    // If toolbelt properties doesn't exist, create it
                    if (m_Instance == null)
                    {
                        m_Instance = ScriptableObject.CreateInstance<ToolBelt>();
                        AssetDatabase.CreateAsset(m_Instance, path);
                        AssetDatabase.SaveAssets();
                    }
#else
					m_Instance = ScriptableObject.CreateInstance<ToolBelt>();
#endif
                }

                return m_Instance;
            }
        }

		public static IFilter ActiveFilter
		{
			get
			{
				switch(Shape)
				{
					case BOX_SHAPE:
						return null;
					case WALL_SHAPE:
						return WallFilter.Instance;
					case ELLIPSE_SHAPE:
						return EllipseFilter.Instance;
				}

				return null;
			}
		}

        public static int Mode { get{ return Instance.m_mode; } set{ Instance.m_mode = value; } }
        public static int Shape { get{ return Instance.m_shape; } set{ Instance.m_shape = value; } }
        public static bool EraserEnabled { get{ return Instance.m_eraserEnabled; } set{ Instance.m_eraserEnabled = value; } }
        public static bool FloorEnabled { get{ return Instance.m_floorEnabled; } set{ Instance.m_floorEnabled = value; } }
        public static bool ClearSelectionMode { get{ return Instance.clearSelectionMode; } set{ Instance.clearSelectionMode = value; } }

        [SerializeField] private int m_mode = NORMAL_MODE;
        [SerializeField] private int m_shape = BOX_SHAPE;
        [SerializeField] private bool m_eraserEnabled = false;
        [SerializeField] private bool m_floorEnabled = false;
        private bool clearSelectionMode = false;
    }
}
