using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WraithavenGames.Bones3
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	public class DepthSorter : MonoBehaviour
	{
		public int[] subMeshIndex;

		[SerializeField] private Mesh mesh;
		[SerializeField] private Vector3[] verts;
		[SerializeField] private int[][] tris;
		[SerializeField] private Tri[][] triSortables;

		private void Awake()
		{
			mesh = GetComponent<MeshFilter>().sharedMesh;
			mesh.MarkDynamic();
		}

		public void UpdateVertices()
		{
			if (mesh == null)
				Awake();

			if (subMeshIndex == null)
				return;

			verts = mesh.vertices;

			if (tris == null || tris.Length < subMeshIndex.Length)
				tris = new int[subMeshIndex.Length][];

			for (int i = 0; i < subMeshIndex.Length; i++)
				tris[i] = mesh.GetTriangles(subMeshIndex[i]);

			if (triSortables == null || triSortables.Length != tris.Length)
				triSortables = new Tri[tris.Length][];

			for (int i = 0; i < subMeshIndex.Length; i++)
			{
				if (triSortables[i] == null || triSortables[i].Length != tris[i].Length / 3)
					triSortables[i] = new Tri[tris[i].Length / 3];

				for (int j = 0; j < triSortables[i].Length; j++)
				{
					if (triSortables[i][j] == null)
						triSortables[i][j] = new Tri();

					triSortables[i][j].v0 = tris[i][j * 3 + 0];
					triSortables[i][j].v1 = tris[i][j * 3 + 1];
					triSortables[i][j].v2 = tris[i][j * 3 + 2];
				}
			}
		}

		private void OnWillRenderObject()
		{
			if (mesh == null)
				Awake();

			if (subMeshIndex == null)
				return;

			if (triSortables == null)
				UpdateVertices();

			Vector3 cam = Camera.current.transform.position;

			for (int i = 0; i < subMeshIndex.Length; i++)
			{
				for (int j = 0; j < triSortables[i].Length; j++)
					triSortables[i][j].FindDist(verts, transform, cam);

				Array.Sort(triSortables[i], 0, triSortables[i].Length);

				for (int j = 0; j < triSortables[i].Length; j++)
				{
					tris[i][j * 3 + 0] = triSortables[i][j].v0;
					tris[i][j * 3 + 1] = triSortables[i][j].v1;
					tris[i][j * 3 + 2] = triSortables[i][j].v2;
				}

				mesh.SetTriangles(tris[i], subMeshIndex[i]);
			}
		}
	}

	public class Tri : IComparable
	{
		public int v0;
		public int v1;
		public int v2;
		public float dist;

		public void FindDist(Vector3[] verts, Transform transform, Vector3 camPos)
		{
			dist = (transform.TransformPoint(verts[v0]) - camPos).sqrMagnitude;
			dist = Mathf.Max(dist, (transform.TransformPoint(verts[v1]) - camPos).sqrMagnitude);
			dist = Mathf.Max(dist, (transform.TransformPoint(verts[v2]) - camPos).sqrMagnitude);
		}

		public int CompareTo(System.Object x)
		{
			float o = ((Tri)x).dist;

			if (dist == o) return 0;
			return dist > o ? -1 : 1;
		}
	}
}
