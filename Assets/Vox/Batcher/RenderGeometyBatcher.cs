﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Vox.Common;
using F.Memory;
using g3;

namespace Vox.Batcher 
{
	public class RenderGeometryBatcher
	{
		private readonly string m_prefabName;
		//! Materials our meshes are to use
		private readonly Material[] m_materials;
		//! A list of buffers for each material
		private readonly List<RenderGeometryBuffer>[] m_buffers;
		//! GameObjects used to hold our geometry
		private readonly List<GameObject> m_objects;
		//! A list of renderer used to render our geometry
		private readonly List<Renderer> m_renderers;

		private bool m_enabled;
		public bool Enabled
		{
			set
			{
				for (int i = 0; i < m_renderers.Count; i++)
				{
					Renderer renderer = m_renderers[i];
					renderer.enabled = value;
				}
				m_enabled = value;
			}
			get
			{
				return m_enabled;
			}
		}

		public RenderGeometryBatcher(string prefabName, Material[] materials)
		{
			m_prefabName = prefabName;
			m_materials = materials;

			int buffersCount = materials == null || materials.Length < 1 ? 1 : materials.Length;
			m_buffers = new List<RenderGeometryBuffer>[buffersCount];

			for (int i = 0; i < m_buffers.Length; i++)
			{
				/* TODO: Let's be optimistic and allocate enough room for just one buffer. It's going to suffice
                 * in >99% of cases. However, this prediction should maybe be based on chunk size rather then
                 * optimism. The bigger the chunk the more likely we're going to need to create more meshes to
                 * hold its geometry because of Unity's 65k-vertices limit per mesh. For chunks up to 32^3 big
                 * this should not be an issue, though.
                 */
				m_buffers[i] = new List<RenderGeometryBuffer>(1)
				{
                    // Default render buffer
                    new RenderGeometryBuffer()
				};
			}

			m_objects = new List<GameObject>(1);
			m_renderers = new List<Renderer>(1);

			Clear();
		}

		public void Reset()
		{
			// Buffers need to be reallocated. Otherwise, more and more memory would be consumed by them. This is
			// because internal arrays grow in capacity and we can't simply release their memory by calling Clear().
			// Objects and renderers are fine, because there's usually only 1 of them. In some extreme cases they
			// may grow more but only by 1 or 2 (and only if Env.ChunkPow>5).
			for (int i = 0; i < m_buffers.Length; i++)
			{
				var geometryBuffer = m_buffers[i];
				for (int j = 0; j < geometryBuffer.Count; j++)
				{
					if (geometryBuffer[j].WasUsed())
						geometryBuffer[j] = new RenderGeometryBuffer();
				}

			}

			ReleaseOldData();
			m_enabled = false;
		}

		/// <summary>
		///     Clear all draw calls
		/// </summary>
		public void Clear()
		{
			for (int i = 0; i < m_buffers.Length; i++)
			{
				var geometryBuffer = m_buffers[i];
				for (int j = 0; j < geometryBuffer.Count; j++)
				{
					geometryBuffer[j].Clear();
				}

			}

			ReleaseOldData();
			m_enabled = false;
		}

		/// <summary>
		///     Addds one face to our render buffer
		/// </summary>
		/// <param name="tris">Triangles to be processed</param>
		/// <param name="verts">Vertices to be processed</param>
		/// <param name="texture">Texture coordinates</param>
		/// <param name="offset">Offset to apply to verts</param>
		/// <param name="materialID">ID of material to use when building the mesh</param>
		public void AddMeshData(int[] tris, VertexData[] verts, ref Rect texture, Vector3 offset, int materialID)
		{
			List<RenderGeometryBuffer> holder = m_buffers[materialID];
			RenderGeometryBuffer buffer = holder[holder.Count - 1];

			int initialVertCount = buffer.Vertices.Count;

			for (int i = 0; i < verts.Length; i++)
			{
				// If there are too many vertices we need to create a new separate buffer for them
				if (buffer.Vertices.Count + 1 > 65000)
				{
					buffer = new RenderGeometryBuffer();
					holder.Add(buffer);
				}

				VertexData v = new VertexData()
				{
					Color = verts[i].Color,
					Normal = verts[i].Normal,
					//Tangent = verts[i].Tangent,
					// Adjust UV coordinates based on provided texture atlas
					UV = new Vector2(
						(verts[i].UV.x * texture.width) + texture.x,
						(verts[i].UV.y * texture.height) + texture.y
						),
					Vertex = verts[i].Vertex + offset
				};
				buffer.AddVertex(ref v);
			}

			for (int i = 0; i < tris.Length; i++)
				buffer.AddIndex(tris[i] + initialVertCount);
		}

		/// <summary>
		///     Addds one face to our render buffer
		/// </summary>
		/// <param name="vertexData"> An array of 4 vertices forming the face</param>
		/// <param name="backFace">If false, vertices are added clock-wise</param>
		/// <param name="materialID">ID of material to use when building the mesh</param>
		public void AddFace(VertexData[] vertexData, bool backFace, int materialID)
		{
			Assert.IsTrue(vertexData.Length == 4);

			List<RenderGeometryBuffer> holder = m_buffers[materialID];
			RenderGeometryBuffer buffer = holder[holder.Count - 1];

			// If there are too many vertices we need to create a new separate buffer for them
			if (buffer.Vertices.Count + 4 > 65000)
			{
				buffer = new RenderGeometryBuffer();
				holder.Add(buffer);
			}

			// Add data to the render buffer
			buffer.AddVertices(vertexData);
			buffer.AddIndices(buffer.Vertices.Count, backFace);
		}

		/// <summary>
		///     Finalize the draw calls
		/// </summary>
		public void Commit(Vector3 position, Quaternion rotation
#if DEBUG
			, string debugName = null
#endif
			)
		{
			ReleaseOldData();

			for (int j = 0; j < m_buffers.Length; j++)
			{
				var holder = m_buffers[j];

				for (int i = 0; i < holder.Count; i++)
				{
					RenderGeometryBuffer buffer = holder[i];

					// No data means there's no mesh to build
					if (buffer.IsEmpty())
						continue;

					var go = GameObjectPool.PopObject(m_prefabName);
					Assert.IsTrue(go != null);
					if (go != null)
					{
#if DEBUG
						go.name = string.Format(debugName, "_", i.ToString());
#endif

                        Mesh mesh = Globals.MeshPool.Pop();
						Assert.IsTrue(mesh.vertices.Length <= 0);
						BuildRenderMesh(mesh, buffer);

						MeshFilter filter = go.GetComponent<MeshFilter>();
						filter.sharedMesh = null;
						filter.sharedMesh = mesh;
						filter.transform.position = position;
						filter.transform.rotation = rotation;

						Renderer renderer = go.GetComponent<Renderer>();
						renderer.sharedMaterial = (m_materials == null || m_materials.Length < 1) ? null : m_materials[j];

						m_objects.Add(go);
						m_renderers.Add(renderer);
					}

					buffer.Clear();
				}

			}
		}

		private void ReleaseOldData()
		{
			Assert.IsTrue(m_objects.Count == m_renderers.Count);
			for (int i = 0; i < m_objects.Count; i++)
			{
				var go = m_objects[i];
				// If the component does not exist it means nothing else has been added as well
				if (go == null)
					continue;

#if DEBUG
				go.name = m_prefabName;
#endif

				MeshFilter filter = go.GetComponent<MeshFilter>();
				filter.sharedMesh.Clear(false);
				Globals.MeshPool.Push(filter.sharedMesh);
				filter.sharedMesh = null;

				Renderer renderer = go.GetComponent<Renderer>();
				renderer.sharedMaterial = null;

				GameObjectPool.PushObject(m_prefabName, go);
			}

			m_objects.Clear();
			m_renderers.Clear();
		}

		public void BuildRenderMesh(Mesh mesh, RenderGeometryBuffer buffer)
		{
			int size = buffer.Vertices.Count;

			// Avoid allocations by retrieving buffers from the pool
            Vector3[] vertices = Globals.Vector3ArrayPool.Pop(size);
			Vector2[] uvs = Globals.Vector2ArrayPool.Pop(size);
			Color32[] colors = Globals.Color32ArrayPool.Pop(size);

			// Fill buffers with data.
			// Due to the way the memory pools work we might have received more
			// data than necessary. This little overhead is well worth it, though.
			// Fill unused data with "zeroes"
			// TODO: Make it so that vertex count is known ahead of time
			for (int i = 0; i < size; i++)
				vertices[i] = buffer.Vertices[i].Vertex;
			for (int i = size; i < vertices.Length; i++)
				vertices[i] = Vector3.zero;
			for (int i = 0; i < size; i++)
				uvs[i] = buffer.Vertices[i].UV;
			for (int i = 0; i < size; i++)
				colors[i] = buffer.Vertices[i].Color;

			// Prepare mesh
			mesh.vertices = vertices;
			mesh.uv = uvs;
			mesh.uv2 = null;
			mesh.uv3 = null;
			mesh.uv4 = null;
			mesh.colors32 = colors;
			mesh.normals = null;
			mesh.tangents = null;
			mesh.SetTriangles(buffer.Triangles, 0);
			mesh.RecalculateNormals();

			// Return memory back to pool
			Globals.Vector3ArrayPool.Push(vertices);
			Globals.Vector2ArrayPool.Push(uvs);
			Globals.Color32ArrayPool.Push(colors);
		}

	}
}
