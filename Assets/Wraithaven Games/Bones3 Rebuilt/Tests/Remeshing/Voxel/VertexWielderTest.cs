using NUnit.Framework;
using Bones3Rebuilt;
using Bones3Rebuilt.Remeshing;
using System;

namespace Test
{
    public class VertexWielderTest
    {
        [Test]
        public void WieldVertices_3Verts_SameProperties()
        {
            var mesh = new ProcMesh();
            mesh.Vertices.Add(new Vec3(0, 0, 0));
            mesh.Vertices.Add(new Vec3(0, 0, 0));
            mesh.Vertices.Add(new Vec3(1, 0, 0));
            mesh.Vertices.Add(new Vec3(0, 0, 0));
            mesh.Normals.Add(new Vec3(0, 1, 0));
            mesh.Normals.Add(new Vec3(0, 1, 0));
            mesh.Normals.Add(new Vec3(1, 0, 0));
            mesh.Normals.Add(new Vec3(0, 1, 0));
            mesh.UVs.Add(new Vec3(0, 0, 0));
            mesh.UVs.Add(new Vec3(0, 0, 0));
            mesh.UVs.Add(new Vec3(1, 0, 0));
            mesh.UVs.Add(new Vec3(0, 0, 0));
            mesh.Triangles.Add(0);
            mesh.Triangles.Add(1);
            mesh.Triangles.Add(2);
            mesh.Triangles.Add(3);

            mesh.WieldVertices();

            Assert.AreEqual(2, mesh.Vertices.Count);
            Assert.AreEqual(new Vec3(0, 0, 0), mesh.Vertices[0]);
            Assert.AreEqual(new Vec3(1, 0, 0), mesh.Vertices[1]);

            Assert.AreEqual(2, mesh.Normals.Count);
            Assert.AreEqual(new Vec3(0, 1, 0), mesh.Normals[0]);
            Assert.AreEqual(new Vec3(1, 0, 0), mesh.Normals[1]);

            Assert.AreEqual(2, mesh.UVs.Count);
            Assert.AreEqual(new Vec3(0, 0, 0), mesh.UVs[0]);
            Assert.AreEqual(new Vec3(1, 0, 0), mesh.UVs[1]);

            Assert.AreEqual(4, mesh.Triangles.Count);
            Assert.AreEqual(0, mesh.Triangles[0]);
            Assert.AreEqual(0, mesh.Triangles[1]);
            Assert.AreEqual(1, mesh.Triangles[2]);
            Assert.AreEqual(0, mesh.Triangles[3]);
        }

        [Test]
        public void WieldVertices_NoNormals()
        {
            var mesh = new ProcMesh();
            mesh.Vertices.Add(new Vec3(0, 0, 0));
            mesh.Vertices.Add(new Vec3(0, 0, 0));
            mesh.Vertices.Add(new Vec3(1, 0, 0));
            mesh.Vertices.Add(new Vec3(0, 0, 0));
            mesh.UVs.Add(new Vec3(0, 0, 0));
            mesh.UVs.Add(new Vec3(0, 0, 0));
            mesh.UVs.Add(new Vec3(1, 0, 0));
            mesh.UVs.Add(new Vec3(0, 0, 0));
            mesh.Triangles.Add(0);
            mesh.Triangles.Add(1);
            mesh.Triangles.Add(2);
            mesh.Triangles.Add(3);

            mesh.WieldVertices();

            Assert.AreEqual(2, mesh.Vertices.Count);
            Assert.AreEqual(new Vec3(0, 0, 0), mesh.Vertices[0]);
            Assert.AreEqual(new Vec3(1, 0, 0), mesh.Vertices[1]);

            Assert.AreEqual(0, mesh.Normals.Count);

            Assert.AreEqual(2, mesh.UVs.Count);
            Assert.AreEqual(new Vec3(0, 0, 0), mesh.UVs[0]);
            Assert.AreEqual(new Vec3(1, 0, 0), mesh.UVs[1]);

            Assert.AreEqual(4, mesh.Triangles.Count);
            Assert.AreEqual(0, mesh.Triangles[0]);
            Assert.AreEqual(0, mesh.Triangles[1]);
            Assert.AreEqual(1, mesh.Triangles[2]);
            Assert.AreEqual(0, mesh.Triangles[3]);
        }

        [Test]
        public void WieldVertices_NoUVs()
        {
            var mesh = new ProcMesh();
            mesh.Vertices.Add(new Vec3(0, 0, 0));
            mesh.Vertices.Add(new Vec3(0, 0, 0));
            mesh.Vertices.Add(new Vec3(1, 0, 0));
            mesh.Vertices.Add(new Vec3(0, 0, 0));
            mesh.Normals.Add(new Vec3(0, 1, 0));
            mesh.Normals.Add(new Vec3(0, 1, 0));
            mesh.Normals.Add(new Vec3(1, 0, 0));
            mesh.Normals.Add(new Vec3(0, 1, 0));
            mesh.Triangles.Add(0);
            mesh.Triangles.Add(1);
            mesh.Triangles.Add(2);
            mesh.Triangles.Add(3);

            mesh.WieldVertices();

            Assert.AreEqual(2, mesh.Vertices.Count);
            Assert.AreEqual(new Vec3(0, 0, 0), mesh.Vertices[0]);
            Assert.AreEqual(new Vec3(1, 0, 0), mesh.Vertices[1]);

            Assert.AreEqual(2, mesh.Normals.Count);
            Assert.AreEqual(new Vec3(0, 1, 0), mesh.Normals[0]);
            Assert.AreEqual(new Vec3(1, 0, 0), mesh.Normals[1]);

            Assert.AreEqual(0, mesh.UVs.Count);

            Assert.AreEqual(4, mesh.Triangles.Count);
            Assert.AreEqual(0, mesh.Triangles[0]);
            Assert.AreEqual(0, mesh.Triangles[1]);
            Assert.AreEqual(1, mesh.Triangles[2]);
            Assert.AreEqual(0, mesh.Triangles[3]);
        }

        [Test]
        public void UnequalMeshCounts()
        {
            var mesh = new ProcMesh();
            mesh.Vertices.Add(new Vec3(0, 0, 0));
            mesh.Vertices.Add(new Vec3(0, 0, 0));
            mesh.Vertices.Add(new Vec3(1, 0, 0));
            mesh.Vertices.Add(new Vec3(0, 0, 0));
            mesh.Normals.Add(new Vec3(0, 1, 0));
            mesh.Normals.Add(new Vec3(0, 1, 0));
            mesh.UVs.Add(new Vec3(0, 0, 0));
            mesh.Triangles.Add(0);
            mesh.Triangles.Add(1);
            mesh.Triangles.Add(2);
            mesh.Triangles.Add(3);

            Assert.Throws<InvalidOperationException>(() => mesh.WieldVertices());
        }

        [Test]
        public void PropertyCount()
        {
            Assert.AreEqual(4, typeof(ProcMesh).GetProperties().Length);
        }
    }
}