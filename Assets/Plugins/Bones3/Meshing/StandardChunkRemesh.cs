using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using WraithavenGames.Bones3;
using WraithavenGames.Bones3.BlockProperties;

namespace WraithavenGames.Bones3.Meshing
{
	public class StandardChunkRemesh : IChunkRemeshTask
	{
		private bool[,] quads = new bool[16,16];
		private int[,] storage = new int[16,16];
		private ArrayList used = new ArrayList();
		private bool[] references = new bool[16 * 16 / 2];

		private Chunk chunk;
		private MeshFilter filter;
		private Renderer render;
		private MeshCollider collider;
		private int chunkX, chunkY, chunkZ;
		private ArrayList materialsUsed = new ArrayList();
		private ArrayList toSort = new ArrayList();
		private bool emptyChunk;
		private ushort[] blocks = new ushort[16 * 16 * 16];
		private byte[] blockStates = new byte[16 * 16 * 16];
		private ushort[] nearbyBlocks = new ushort[16 * 16 * 16 * 6];
		private byte[] nearbyBlockStates = new byte[16 * 16 * 16 * 6];
		private TempMesh collisionMesh = new TempMesh();
		private TempMesh visualMesh = new TempMesh();
		private Material[] materialArray;
		private BlockTypeList blockTypes;
		private bool activeInPool;

		public void MarkActive()
		{
			activeInPool = true;
		}

		public void Initalize(Transform player, Chunk chunk)
		{
			this.chunk = chunk;
			render = chunk.GetComponent<Renderer>();
			collider = chunk.GetComponent<MeshCollider>();
			filter = chunk.GetComponent<MeshFilter>();
			blockTypes = chunk.BlockTypes;

			Array.Copy(chunk.GetBlocksRaw(), blocks, blocks.Length);
			Array.Copy(chunk.GetBlockStatesRaw(), blockStates, blockStates.Length);

			Chunk side;
			for (int i = 0; i < 6; i++)
			{
				side = chunk.GetNearbyChunk(i);
				if (side == null)
				{
					for (int j = 0; j < 4096; j++)
					{
						nearbyBlocks[i * 4096 + j] = 0;
						nearbyBlockStates[i * 4096 + j] = 0;
					}
				}
				else
				{
					Array.Copy(side.GetBlocksRaw(), 0, nearbyBlocks, i * 4096, 4096);
					Array.Copy(side.GetBlockStatesRaw(), 0, nearbyBlockStates, i * 4096, 4096);
				}
			}

			chunkX = chunk.chunkX;
			chunkY = chunk.chunkY;
			chunkZ = chunk.chunkZ;
			emptyChunk = chunk.EmptyChunk;
		}

		public bool MatchesTask(IChunkRemeshTask other)
		{
			StandardChunkRemesh o = other as StandardChunkRemesh;

			if (o == null)
				return false;

			return chunk == o.chunk;
		}

		public void CleanupRemesh()
		{
			// Clear references to avoid memory leaks
			chunk = null;
			filter = null;
			render = null;
			collider = null;
			materialArray = null;
			blockTypes = null;

			// Clear lists to prepare for next pool use
			materialsUsed.Clear();
			toSort.Clear();

			// Mark object as unused in pool
			activeInPool = false;
		}

		public bool IsBeginUsed()
		{
			return activeInPool;
		}

		public void FinishTask()
		{
			if (chunk == null || render == null || collider == null)
				return;

			chunk.MarkRemeshComplete();

			if (emptyChunk)
			{
				render.enabled = false;
				collider.enabled = false;
				return;
			}
			render.enabled = true;
			collider.enabled = true;

			Mesh mesh;

			// Compile collision
			{
				mesh = collider.sharedMesh;
				if (mesh == null)
				{
					mesh = new Mesh();
					mesh.name = string.Format("Chunk COL [{0}, {1}, {2}]", chunkX, chunkY, chunkZ);
					collider.sharedMesh = mesh;
				}

				mesh.Clear();
				collisionMesh.Transfer(mesh);
				collider.enabled = false;
				collider.enabled = true;
			}

			// Compile visual
			{
				mesh = filter.sharedMesh;
				if (mesh == null)
				{
					mesh = new Mesh();
					mesh.name = string.Format("Chunk [{0}, {1}, {2}]", chunkX, chunkY, chunkZ);
					filter.sharedMesh = mesh;
				}

				mesh.Clear();
				mesh.subMeshCount = materialsUsed.Count;
				visualMesh.Transfer(mesh);

				if (toSort.Count == 0)
				{
					DepthSorter ds = chunk.GetComponent<DepthSorter>();
					if (ds != null)
					{
						if (Application.isPlaying)
							GameObject.Destroy(ds);
						else
							GameObject.DestroyImmediate(ds);
						;
					}
				}
				else
				{
					DepthSorter ds = chunk.GetComponent<DepthSorter>();
					if (ds == null)
						ds = chunk.gameObject.AddComponent<DepthSorter>();

					int[] indices = new int[toSort.Count];
					for (int i = 0; i < toSort.Count; i++)
						indices[i] = (int)toSort[i];
					ds.subMeshIndex = indices;

					ds.UpdateVertices();
				}

				render.materials = materialArray;
			}
		}

		public void RemeshChunk()
		{
			if (emptyChunk)
				return;

			RebuildCollisionMesh();
			RegenerateVisualMesh();

			if (!emptyChunk)
			{
				collisionMesh.Compile();
				visualMesh.Compile();
			}

			collisionMesh.Clear();
			visualMesh.Clear();
		}

		private void RegenerateVisualMesh()
		{
			// Find materials used in this chunk
			for (int i = 0; i < blocks.Length; i++)
			{
				// Ignore air blocks
				if (blocks[i] == 0)
					continue;

				if (!materialsUsed.Contains(blocks[i]))
					materialsUsed.Add(blocks[i]);
			}

			for (int submesh = 0; submesh < materialsUsed.Count;)
			{
				MaterialBlock matProps = blockTypes.GetMaterialProperties((ushort)materialsUsed[submesh]);
				if (matProps == null)
				{
					// Block data corrupted?
					materialsUsed.RemoveAt(submesh);
					continue;
				}

				bool rendered = true;

				if (matProps.GroupBlocks)
					rendered = RebuildMeshStrong(visualMesh, submesh, matProps.Id);
				else
					RebuildMesh(visualMesh, submesh, matProps.Id);

				if (rendered)
				{
					if (matProps.DepthSort)
						toSort.Add(submesh);

					submesh++;
				}
				else
					materialsUsed.RemoveAt(submesh);
			}

			if (materialsUsed.Count == 0)
			{
				emptyChunk = true;
				return;
			}

			materialArray = new Material[materialsUsed.Count];
			for (int i = 0; i < materialArray.Length; i++)
				materialArray[i] = blockTypes.GetMaterialProperties((ushort)materialsUsed[i]).Material;
		}

	    private bool RebuildMeshStrong(TempMesh mesh, int submeshIndex, ushort blockId)
	    {
	        int x, y, z, j;
	        int index, next, end, chunkBegin;
	        int nearbyOffset;
	        bool hasQuads = false;

	        for (j = 0; j < 6; j++)
	        {
	        	nearbyOffset = j * 4096;
				end = j % 2 == 0 ? 15 : 0;

	        	if (j <= 1)
	        	{
	        		for (x = 0; x < 16; x++)
	        		{
	        			for (y = 0; y < 16; y++)
	        				for (z = 0; z < 16; z++)
	        				{
	        					index = x * 16 * 16 + y * 16 + z;

	        					if (j == 0)
	        					{
									chunkBegin = 0 * 16 * 16 + y * 16 + z;
									next = (x + 1) * 16 * 16 + y * 16 + z;
	        					}
	        					else
	        					{
									chunkBegin = 15 * 16 * 16 + y * 16 + z;
									next = (x - 1) * 16 * 16 + y * 16 + z;
	        					}

	        					quads[y, z] = blocks[index] == blockId;
        						quads[y, z] &= x == end || blocks[next] == 0 || (blockStates[next] & MaterialBlock.BLOCK_STATE_TRANSPARENT) > 0;

	                    		if (x != end && blocks[index] == blocks[next])
	                    			quads[y, z] &= (blockStates[index] & MaterialBlock.BLOCK_STATE_VIEW_INSIDES) > 0;

								if (x == end)
								{
									quads[y, z] &= nearbyBlocks[chunkBegin + nearbyOffset] == 0
									|| (nearbyBlockStates[chunkBegin + nearbyOffset] & MaterialBlock.BLOCK_STATE_TRANSPARENT) > 0;

		                    		if (blocks[index] == nearbyBlocks[chunkBegin + nearbyOffset])
		                    			quads[y, z] &= (blockStates[index] & MaterialBlock.BLOCK_STATE_VIEW_INSIDES) > 0;
								}
	        				}

	        			if (GreedyMesher() == 0)
	        				continue;
						hasQuads |= AddQuads(mesh, submeshIndex, j, x);
	        		}
	        	}
	        	else if (j <= 3)
	        	{
	        		for (y = 0; y < 16; y++)
	        		{
	        			for (x = 0; x < 16; x++)
	        				for (z = 0; z < 16; z++)
	        				{
	        					index = x * 16 * 16 + y * 16 + z;

	        					if (j == 2)
	        					{
									chunkBegin = x * 16 * 16 + 0 * 16 + z;
									next = x * 16 * 16 + (y + 1) * 16 + z;
	        					}
	        					else
	        					{
									chunkBegin = x * 16 * 16 + 15 * 16 + z;
									next = x * 16 * 16 + (y - 1) * 16 + z;
	        					}

	        					quads[x, z] = blocks[index] == blockId;
        						quads[x, z] &= y == end || blocks[next] == 0 || (blockStates[next] & MaterialBlock.BLOCK_STATE_TRANSPARENT) > 0;

	                    		if (y != end && blocks[index] == blocks[next])
	                    			quads[x, z] &= (blockStates[index] & MaterialBlock.BLOCK_STATE_VIEW_INSIDES) > 0;

								if (y == end)
								{
									quads[x, z] &= nearbyBlocks[chunkBegin + nearbyOffset] == 0
									|| (nearbyBlockStates[chunkBegin + nearbyOffset] & MaterialBlock.BLOCK_STATE_TRANSPARENT) > 0;

		                    		if (blocks[index] == nearbyBlocks[chunkBegin + nearbyOffset])
		                    			quads[x, z] &= (blockStates[index] & MaterialBlock.BLOCK_STATE_VIEW_INSIDES) > 0;
								}
	        				}

	        			if (GreedyMesher() == 0)
	        				continue;
						hasQuads |= AddQuads(mesh, submeshIndex, j, y);
	        		}
	        	}
	        	else
	        	{
	        		for (z = 0; z < 16; z++)
	        		{
	        			for (x = 0; x < 16; x++)
	        				for (y = 0; y < 16; y++)
	        				{
	        					index = x * 16 * 16 + y * 16 + z;

	        					if (j == 4)
	        					{
									chunkBegin = x * 16 * 16 + y * 16 + 0;
									next = x * 16 * 16 + y * 16 + (z + 1);
	        					}
	        					else
	        					{
									chunkBegin = x * 16 * 16 + y * 16 + 15;
									next = x * 16 * 16 + y * 16 + (z - 1);
	        					}

	        					quads[x, y] = blocks[index] == blockId;
        						quads[x, y] &= z == end || blocks[next] == 0 || (blockStates[next] & MaterialBlock.BLOCK_STATE_TRANSPARENT) > 0;

	                    		if (z != end && blocks[index] == blocks[next])
	                    			quads[x, y] &= (blockStates[index] & MaterialBlock.BLOCK_STATE_VIEW_INSIDES) > 0;

								if (z == end)
								{
									quads[x, y] &= nearbyBlocks[chunkBegin + nearbyOffset] == 0
									|| (nearbyBlockStates[chunkBegin + nearbyOffset] & MaterialBlock.BLOCK_STATE_TRANSPARENT) > 0;

		                    		if (blocks[index] == nearbyBlocks[chunkBegin + nearbyOffset])
		                    			quads[x, y] &= (blockStates[index] & MaterialBlock.BLOCK_STATE_VIEW_INSIDES) > 0;
								}
	        				}

	        			if (GreedyMesher() == 0)
	        				continue;
						hasQuads |= AddQuads(mesh, submeshIndex, j, z);
	        		}
	        	}
	        }

	        return hasQuads;
	    }

	    private bool AddQuads(TempMesh mesh, int submesh, int side, int offset)
	    {
	    	int x, y, w, h, o;

	    	for (int i = 0; i < used.Count; i++)
	    		references[i] = false;

	    	bool hasQuads = false;

	    	for (x = 0; x < 16; x++)
	    		for (y = 0; y < 16; y++)
	    		{
	    			if (storage[x, y] == -1)
	    				continue;

    				if (references[storage[x, y]])
    					continue;

					o = storage[x, y];
					references[o] = true;

					for (w = x; w < 16; w++)
						if (storage[w, y] != o)
							break;

					for (h = y; h < 16; h++)
						if (storage[x, h] != o)
							break;

					w -= x;
					h -= y;

					AddSingleQuad(mesh, submesh, side, x, y, w, h, offset);
					hasQuads = true;
	    		}
	    	return hasQuads;
	    }

	    private void AddSingleQuad(TempMesh mesh, int submesh, int j, int x, int y, int w, int h, int o)
	    {
	    	LinkedList<int> triangles = mesh.GetTriangleList(submesh);
	    	LinkedList<Vector3> verts = mesh.GetVertices();
	    	LinkedList<Vector2> uvs = mesh.GetUVs();

			int vertexCount = verts.Count;
			triangles.AddLast(vertexCount + 0);
			triangles.AddLast(vertexCount + 1);
			triangles.AddLast(vertexCount + 2);
			triangles.AddLast(vertexCount + 0);
			triangles.AddLast(vertexCount + 2);
			triangles.AddLast(vertexCount + 3);

			if(j == 0)
			{
				float sx = o;
				float sy = x;
				float sz = y;
				float bx = sx + 1;
				float by = sy + w;
				float bz = sz + h;

				verts.AddLast(new Vector3(bx, by, bz));
				verts.AddLast(new Vector3(bx, sy, bz));
				verts.AddLast(new Vector3(bx, sy, sz));
				verts.AddLast(new Vector3(bx, by, sz));

				if (uvs != null)
				{
					uvs.AddLast(new Vector2(0, 0));
					uvs.AddLast(new Vector2(0, w));
					uvs.AddLast(new Vector2(h, w));
					uvs.AddLast(new Vector2(h, 0));
				}
			}
			else if(j == 1)
			{
				float sx = o;
				float sy = x;
				float sz = y;
				float by = sy + w;
				float bz = sz + h;

				verts.AddLast(new Vector3(sx, sy, sz));
				verts.AddLast(new Vector3(sx, sy, bz));
				verts.AddLast(new Vector3(sx, by, bz));
				verts.AddLast(new Vector3(sx, by, sz));

				if (uvs != null)
				{
					uvs.AddLast(new Vector2(h, w));
					uvs.AddLast(new Vector2(0, w));
					uvs.AddLast(new Vector2(0, 0));
					uvs.AddLast(new Vector2(h, 0));
				}
			}
			else if(j == 2)
			{
				float sx = x;
				float sy = o;
				float sz = y;
				float bx = sx + w;
				float by = sy + 1;
				float bz = sz + h;

				verts.AddLast(new Vector3(sx, by, sz));
				verts.AddLast(new Vector3(sx, by, bz));
				verts.AddLast(new Vector3(bx, by, bz));
				verts.AddLast(new Vector3(bx, by, sz));

				if (uvs != null)
				{
					uvs.AddLast(new Vector2(0, 0));
					uvs.AddLast(new Vector2(0, h));
					uvs.AddLast(new Vector2(w, h));
					uvs.AddLast(new Vector2(w, 0));
				}
			}
			else if(j == 3)
			{
				float sx = x;
				float sy = o;
				float sz = y;
				float bx = sx + w;
				float bz = sz + h;

				verts.AddLast(new Vector3(bx, sy, bz));
				verts.AddLast(new Vector3(sx, sy, bz));
				verts.AddLast(new Vector3(sx, sy, sz));
				verts.AddLast(new Vector3(bx, sy, sz));

				if (uvs != null)
				{
					uvs.AddLast(new Vector2(w, h));
					uvs.AddLast(new Vector2(0, h));
					uvs.AddLast(new Vector2(0, 0));
					uvs.AddLast(new Vector2(w, 0));
				}
			}
			else if(j == 4)
			{
				float sx = x;
				float sy = y;
				float sz = o;
				float bx = sx + w;
				float by = sy + h;
				float bz = sz + 1;

				verts.AddLast(new Vector3(bx, by, bz));
				verts.AddLast(new Vector3(sx, by, bz));
				verts.AddLast(new Vector3(sx, sy, bz));
				verts.AddLast(new Vector3(bx, sy, bz));

				if (uvs != null)
				{
					uvs.AddLast(new Vector2(0, 0));
					uvs.AddLast(new Vector2(0, w));
					uvs.AddLast(new Vector2(h, w));
					uvs.AddLast(new Vector2(h, 0));
				}
			}
			else
			{
				float sx = x;
				float sy = y;
				float sz = o;
				float bx = sx + w;
				float by = sy + h;

				verts.AddLast(new Vector3(sx, sy, sz));
				verts.AddLast(new Vector3(sx, by, sz));
				verts.AddLast(new Vector3(bx, by, sz));
				verts.AddLast(new Vector3(bx, sy, sz));

				if (uvs != null)
				{
					uvs.AddLast(new Vector2(0, 0));
					uvs.AddLast(new Vector2(0, h));
					uvs.AddLast(new Vector2(w, h));
					uvs.AddLast(new Vector2(w, 0));
				}
			}
	    }

	    private int GreedyMesher()
	    {
	    	int x, y, t, q;
	    	int w = -1;
	    	int total = 0;

	    	for (x = 0; x < 16; x++)
	    		for (y = 0; y < 16; y++)
	    			storage[x, y] = -1;

			for (y = 0; y < 16; y++)
			{
				w = -1;
				for (x = 0; x < 16; x++)
				{
					if (!quads[x, y])
					{
						w = -1;
						continue;
					}

					if (w == -1)
					{
						w = total;
						total++;
					}

					storage[x, y] = w;
				}
			}

			for (y = 0; y < 15; y++)
			{
				w = 0;
				for (x = 0; x < 16; x++)
				{
					if (!quads[x, y])
					{
						if (w > 0)
						{
							q = 0;
							if (!quads[x, y+1] && (x == w || !quads[x-w-1, y+1]))
							{
								for (t = x-w; t < x; t++)
								{
									if (!quads[t, y+1])
										break;

									q++;
								}

								if (q == w)
									for (t = x-w; t < x; t++)
										storage[t, y+1] = storage[t, y];
							}
							w = 0;
						}
						continue;
					}
					w++;
				}

				if (w > 0)
				{
					q = 0;
					if (x == w || !quads[x-w-1, y+1])
					{
						for (t = x-w; t<x; t++)
						{
							if (!quads[t, y+1])
								break;
							q++;
						}

						if (q == w)
							for (t = x-w; t < x; t++)
								storage[t, y+1] = storage[t, y];
					}
				}
			}

			for (x = 0; x < 16; x++)
				for (y = 0; y < 16; y++)
					if (storage[x, y] > -1 && !used.Contains(storage[x, y]))
						used.Add(storage[x, y]);
			for (x = 0; x < 16; x++)
				for (y = 0; y < 16; y++)
					if (storage[x, y] > -1)
						storage[x, y] = used.IndexOf((int)storage[x, y]);
			return used.Count;
	    }

	    private void RebuildCollisionMesh()
	    {
	        int x, y, z, j;

	        for (j = 0; j < 6; j++)
	        {
	        	if (j <= 1)
	        	{
	        		for (x = 0; x < 16; x++)
	        		{
	        			for (y = 0; y < 16; y++)
	        				for (z = 0; z < 16; z++)
	        				{
	        					quads[y, z] = blocks[x * 16 * 16 + y * 16 + z] != 0;

	        					if (j == 0) quads[y, z] &= x == 16 - 1 || blocks[(x + 1) * 16 * 16 + y * 16 + z] == 0;
								else quads[y, z] &= x == 0 || blocks[(x - 1) * 16 * 16 + y * 16 + z] == 0;
	        				}
	        			if (GreedyMesher() == 0)
	        				continue;
						AddQuads(collisionMesh, 0, j, x);
	        		}
	        	}
	        	else if (j <= 3)
	        	{
	        		for (y = 0; y < 16; y++)
	        		{
	        			for (x = 0; x < 16; x++)
	        				for (z = 0; z < 16; z++)
	        				{
	        					quads[x, z] = blocks[x * 16 * 16 + y * 16 + z] != 0;

	        					if (j == 2) quads[x, z] &= y == 16 - 1 || blocks[x * 16 * 16 + (y + 1) * 16 + z] == 0;
	        					else quads[x, z] &= y == 0 || blocks[x * 16 * 16 + (y - 1) * 16 + z] == 0;
	        				}
	        			if (GreedyMesher() == 0)
	        				continue;
						AddQuads(collisionMesh, 0, j, y);
	        		}
	        	}
	        	else
	        	{
	        		for (z = 0; z < 16; z++)
	        		{
	        			for (x = 0; x < 16; x++)
	        				for (y = 0; y < 16; y++)
	        				{
	        					quads[x, y] = blocks[x * 16 * 16 + y * 16 + z] != 0;

	        					if (j == 4) quads[x, y] &= z == 16 - 1 || blocks[x * 16 * 16 + y * 16 + (z + 1)] == 0;
	        					else quads[x, y] &= z == 0 || blocks[x * 16 * 16 + y * 16 + (z - 1)] == 0;
	        				}
	        			if (GreedyMesher() == 0)
	        				continue;
						AddQuads(collisionMesh, 0, j, z);
	        		}
	        	}
	        }
	    }

		private void RebuildMesh(TempMesh mesh, int submesh, int blockId)
	    {
	        int x, y, z, index, next;

	        for (x = 0; x < 16; x++)
	    		for (y = 0; y < 16; y++)
		            for (z = 0; z < 16; z++)
		            {
		                index = x * 16 * 16 + y * 16 + z;

		                if (blocks[index] != blockId)
		                    continue;

		                next = (x + 1) * 16 * 16 + y * 16 + z;
	                    if (x == 15 || blocks[next] == 0 || (blockStates[next] & MaterialBlock.BLOCK_STATE_TRANSPARENT) > 0)
	                    	if (x == 15 || blocks[index] != blocks[next] || (blockStates[index] & MaterialBlock.BLOCK_STATE_VIEW_INSIDES) > 0)
	                    		AddSingleQuad(visualMesh, submesh, 0, y, z, 1, 1, x);

		                next = (x - 1) * 16 * 16 + y * 16 + z;
	                    if (x == 0 || blocks[next] == 0 || (blockStates[next] & MaterialBlock.BLOCK_STATE_TRANSPARENT) > 0)
	                    	if (x == 0 || blocks[index] != blocks[next] || (blockStates[index] & MaterialBlock.BLOCK_STATE_VIEW_INSIDES) > 0)
	                    		AddSingleQuad(visualMesh, submesh, 1, y, z, 1, 1, x);

		                next = x * 16 * 16 + (y + 1) * 16 + z;
	                    if (y == 15 || blocks[next] == 0 || (blockStates[next] & MaterialBlock.BLOCK_STATE_TRANSPARENT) > 0)
	                    	if (y == 15 || blocks[index] != blocks[next] || (blockStates[index] & MaterialBlock.BLOCK_STATE_VIEW_INSIDES) > 0)
	                    		AddSingleQuad(visualMesh, submesh, 2, x, z, 1, 1, y);

		                next = x * 16 * 16 + (y - 1) * 16 + z;
	                    if (y == 0 || blocks[next] == 0 || (blockStates[next] & MaterialBlock.BLOCK_STATE_TRANSPARENT) > 0)
	                    	if (y == 0 || blocks[index] != blocks[next] || (blockStates[index] & MaterialBlock.BLOCK_STATE_VIEW_INSIDES) > 0)
	                    		AddSingleQuad(visualMesh, submesh, 3, x, z, 1, 1, y);

		                next = x * 16 * 16 + y * 16 + (z + 1);
	                    if (z == 15 || blocks[next] == 0 || (blockStates[next] & MaterialBlock.BLOCK_STATE_TRANSPARENT) > 0)
	                    	if (z == 15 || blocks[index] != blocks[next] || (blockStates[index] & MaterialBlock.BLOCK_STATE_VIEW_INSIDES) > 0)
	                    		AddSingleQuad(visualMesh, submesh, 4, x, y, 1, 1, z);

		                next = x * 16 * 16 + y * 16 + (z + 1);
	                    if (z == 0 || blocks[next] == 0 || (blockStates[next] & MaterialBlock.BLOCK_STATE_TRANSPARENT) > 0)
	                    	if (z == 0 || blocks[index] != blocks[next] || (blockStates[index] & MaterialBlock.BLOCK_STATE_VIEW_INSIDES) > 0)
	                    		AddSingleQuad(visualMesh, submesh, 5, x, y, 1, 1, z);
	                }
	    }
	}
}
