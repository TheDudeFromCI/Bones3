using UnityEngine;
using UnityEditor;
using WraithavenGames.Bones3.Terrain;

namespace WraithavenGames.Bones3.Demo
{
	[CustomEditor(typeof(BasicPerlinNoise))]
	public class BasicPerlinNoiseEditor : Editor
	{
		private BasicPerlinNoise noise;
		private TerrainGenerator gen;

		private void OnEnable()
		{
			noise = target as BasicPerlinNoise;
			gen = noise.gameObject.GetComponent<TerrainGenerator>();
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			base.OnInspectorGUI();

			if (EditorGUI.EndChangeCheck() && noise.autoRegenerate)
			{
				if (gen != null && Application.isPlaying)
					gen.ResetChunkLoader();
			}
		}
	}
}
