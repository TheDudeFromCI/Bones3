using Bones3Rebuilt;
using NUnit.Framework;
using Bones3Rebuilt.Util;

namespace Test
{
    public class ProcMeshTest
    {
        [Test]
        public void PropertyCount()
        {
            // This test exists in order to fail if the proc mesh ever recieves
            // new property fields so that existing tests in this class may be
            // modified accordingly.

            Assert.AreEqual(5, typeof(ProcMesh).GetProperties().Length);
        }

        [Test]
        public void ClearData()
        {
            var mesh = new ProcMesh();
            Assert.IsFalse(mesh.HasTriangles);

            mesh.Vertices.Add(new Vec3(1f, 10f, 100f));
            mesh.Normals.Add(new Vec3(5f, 5f, 5f));
            mesh.UVs.Add(new Vec3(9f, 99f, 999f));
            mesh.Triangles.Add(34);
            Assert.IsTrue(mesh.HasTriangles);

            mesh.Clear();
            Assert.IsFalse(mesh.HasTriangles);
        }

        [Test]
        public void CombineMeshes()
        {
            var a = new ProcMesh();
            a.Vertices.Add(new Vec3(1f, 1f, 1f));
            a.Normals.Add(new Vec3(1f, 1f, 1f));
            a.UVs.Add(new Vec3(1f, 1f, 1f));
            a.Triangles.Add(0);
            a.Triangles.Add(1);
            a.Triangles.Add(2);

            var b = new ProcMesh();
            b.Vertices.Add(new Vec3(3f, 3f, 3f));
            b.Normals.Add(new Vec3(3f, 3f, 3f));
            b.UVs.Add(new Vec3(3f, 3f, 3f));
            b.Triangles.Add(2);
            b.Triangles.Add(0);
            b.Triangles.Add(1);

            a.AddData(b);

            Assert.AreEqual(2, a.Vertices.Count);
            Assert.AreEqual(2, a.Normals.Count);
            Assert.AreEqual(2, a.UVs.Count);
            Assert.AreEqual(6, a.Triangles.Count);
        }
    }
}
