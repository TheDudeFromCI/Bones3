using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WraithavenGames.Bones3.Meshing
{
	public class TempMesh
	{
		private LinkedList<Vector3> vertices = new LinkedList<Vector3>();
		private LinkedList<Vector2> uvs = new LinkedList<Vector2>();
		private ArrayList submeshes = new ArrayList();

		private Vector3[] verts;
		private Vector2[] uv;
		private int[][] tris;

		public void Clear()
		{
			vertices.Clear();
			uvs.Clear();

			for (int i = 0; i < submeshes.Count; i++)
				((LinkedList<int>)submeshes[i]).Clear();
		}

		public LinkedList<int> GetTriangleList(int submesh)
		{
			while (submesh >= submeshes.Count)
				submeshes.Add(new LinkedList<int>());

			return submeshes[submesh] as LinkedList<int>;
		}

		public void Compile()
		{
			int i;

			i = 0;
			verts = new Vector3[vertices.Count];
			foreach (Vector3 v in vertices)
				verts[i++] = v;

			i = 0;
			uv = new Vector2[uvs.Count];
			foreach (Vector2 v in uvs)
				uv[i++] = v;

			tris = new int[submeshes.Count][];
			for (i = 0; i < tris.Length; i++)
			{
				LinkedList<int> list = GetTriangleList(i);
				if (list.Count == 0)
					break;

				tris[i] = new int[list.Count];

				int j = 0;
				foreach (int v in list)
					tris[i][j++] = v;
			}
		}

		public LinkedList<Vector3> GetVertices()
		{
			return vertices;
		}

		public LinkedList<Vector2> GetUVs()
		{
			return uvs;
		}

		public void Transfer(Mesh mesh)
		{
			mesh.vertices = verts;
			if (uv.Length > 0) mesh.uv = uv;

			for (int i = 0; i < tris.Length; i++)
			{
				if (tris[i] == null)
					break;
				mesh.SetTriangles(tris[i], i);
			}

			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
		}
	}
}
