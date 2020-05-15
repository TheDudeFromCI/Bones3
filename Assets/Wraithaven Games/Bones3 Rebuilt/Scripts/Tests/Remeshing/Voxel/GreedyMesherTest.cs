using NUnit.Framework;
using Bones3Rebuilt;
using Bones3Rebuilt.Util;
using System.Linq;
using System;

namespace Test
{
    public class GreedyMesherTest
    {
        [Test]
        public void SetQuads_Shape1()
        {
            var chunkSize = new GridSize(2);
            var mesher = new GreedyMesher(chunkSize, true);

            mesher.SetQuad(0, 0, new GreedyMesher.Quad(0, 0));
            mesher.SetQuad(1, 0, new GreedyMesher.Quad(0, 0));
            mesher.SetQuad(2, 0, new GreedyMesher.Quad(0, 0));

            mesher.SetQuad(0, 1, new GreedyMesher.Quad(0, 0));
            mesher.SetQuad(0, 2, new GreedyMesher.Quad(0, 0));
            mesher.SetQuad(0, 3, new GreedyMesher.Quad(0, 0));

            var mesh = new ProcMesh();
            mesher.Mesh(0, 3, mesh);

            Assert.AreEqual(12, mesh.Triangles.Count);
            Assert.AreEqual(8, mesh.Vertices.Count);
            Assert.AreEqual(8, mesh.Normals.Count);
            Assert.AreEqual(8, mesh.UVs.Count);

            Assert.IsTrue(mesh.Vertices.Contains(new Vec3(0, 0, 0)));
            Assert.IsTrue(mesh.Vertices.Contains(new Vec3(3, 0, 0)));
            Assert.IsTrue(mesh.Vertices.Contains(new Vec3(3, 0, 1)));
            Assert.IsTrue(mesh.Vertices.Contains(new Vec3(0, 0, 1)));

            Assert.IsTrue(mesh.Vertices.Contains(new Vec3(0, 0, 1)));
            Assert.IsTrue(mesh.Vertices.Contains(new Vec3(1, 0, 1)));
            Assert.IsTrue(mesh.Vertices.Contains(new Vec3(1, 0, 4)));
            Assert.IsTrue(mesh.Vertices.Contains(new Vec3(0, 0, 4)));

            Assert.AreEqual(2, mesh.Triangles.Where(a => a == 0).Count());
            Assert.AreEqual(1, mesh.Triangles.Where(a => a == 1).Count());
            Assert.AreEqual(2, mesh.Triangles.Where(a => a == 2).Count());
            Assert.AreEqual(1, mesh.Triangles.Where(a => a == 3).Count());

            Assert.AreEqual(2, mesh.Triangles.Where(a => a == 4).Count());
            Assert.AreEqual(1, mesh.Triangles.Where(a => a == 5).Count());
            Assert.AreEqual(2, mesh.Triangles.Where(a => a == 6).Count());
            Assert.AreEqual(1, mesh.Triangles.Where(a => a == 7).Count());
        }

        [Test]
        public void SetQuad_NoUVs()
        {
            var chunkSize = new GridSize(2);
            var mesher = new GreedyMesher(chunkSize, false);

            mesher.SetQuad(0, 0, new GreedyMesher.Quad(0, 0));
            mesher.SetQuad(1, 1, new GreedyMesher.Quad(0, 0));
            mesher.SetQuad(2, 2, new GreedyMesher.Quad(0, 0));
            mesher.SetQuad(2, 3, new GreedyMesher.Quad(0, 0));

            var mesh = new ProcMesh();
            mesher.Mesh(0, 1, mesh);

            Assert.AreEqual(18, mesh.Triangles.Count);
            Assert.AreEqual(12, mesh.Vertices.Count);
            Assert.AreEqual(12, mesh.Normals.Count);
            Assert.AreEqual(0, mesh.UVs.Count);
        }

        [Test]
        public void SetQuad_TwoTextures_TwoRotations()
        {
            var chunkSize = new GridSize(2);
            var mesher = new GreedyMesher(chunkSize, true);

            mesher.SetQuad(0, 0, new GreedyMesher.Quad(0, 0));
            mesher.SetQuad(0, 1, new GreedyMesher.Quad(0, 1));
            mesher.SetQuad(1, 0, new GreedyMesher.Quad(1, 0));
            mesher.SetQuad(1, 1, new GreedyMesher.Quad(1, 1));

            var mesh = new ProcMesh();
            mesher.Mesh(0, 4, mesh);

            Assert.AreEqual(24, mesh.Triangles.Count);
            Assert.AreEqual(16, mesh.Vertices.Count);
            Assert.AreEqual(16, mesh.Normals.Count);
            Assert.AreEqual(16, mesh.UVs.Count);
        }

        [Test]
        public void SetQuad_TwoTextures_TwoRotations_TwoTextures_NoUVs()
        {
            var chunkSize = new GridSize(2);
            var mesher = new GreedyMesher(chunkSize, false);

            mesher.SetQuad(0, 0, new GreedyMesher.Quad(0, 0));
            mesher.SetQuad(0, 1, new GreedyMesher.Quad(0, 1));
            mesher.SetQuad(1, 0, new GreedyMesher.Quad(1, 0));
            mesher.SetQuad(1, 1, new GreedyMesher.Quad(1, 1));

            var mesh = new ProcMesh();
            mesher.Mesh(0, 0, mesh);

            Assert.AreEqual(6, mesh.Triangles.Count);
            Assert.AreEqual(4, mesh.Vertices.Count);
            Assert.AreEqual(4, mesh.Normals.Count);
            Assert.AreEqual(0, mesh.UVs.Count);
        }

        [Test]
        public void SetQuad_IndexOutOfBounds()
        {
            var chunkSize = new GridSize(2);
            var mesher = new GreedyMesher(chunkSize, false);

            Assert.Throws<ArgumentOutOfRangeException>(() => mesher.SetQuad(-1, 3, default));
            Assert.Throws<ArgumentOutOfRangeException>(() => mesher.SetQuad(5, 1, default));
            Assert.Throws<ArgumentOutOfRangeException>(() => mesher.SetQuad(0, 12, default));
        }

        [Test]
        public void SetQuad_NoQuads()
        {
            var chunkSize = new GridSize(2);
            var mesher = new GreedyMesher(chunkSize, true);

            var mesh = new ProcMesh();
            mesher.Mesh(0, 5, mesh);

            Assert.AreEqual(0, mesh.Triangles.Count);
            Assert.AreEqual(0, mesh.Vertices.Count);
            Assert.AreEqual(0, mesh.Normals.Count);
            Assert.AreEqual(0, mesh.UVs.Count);
        }
    }
}
