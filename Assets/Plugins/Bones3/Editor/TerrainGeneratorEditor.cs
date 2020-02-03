using UnityEngine;
using UnityEditor;

namespace WraithavenGames.Bones3.Terrain
{
	[CustomEditor(typeof(TerrainGenerator))]
	public class TerrainGeneratorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUILayout.Space(15);

			if (GUILayout.Button("Regenerate"))
			{
				TerrainGenerator t = target as TerrainGenerator;
				t.ResetChunkLoader();
			}
		}
	}
}
