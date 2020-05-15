using Bones3Rebuilt;
using Bones3Rebuilt.Util;
using NUnit.Framework;

namespace Test
{
    public class LayeredProcMeshTest
    {
        [Test]
        public void GetLayer_Generates()
        {
            var mesh = new LayeredProcMesh();
            var layer = mesh.GetLayer(7);

            Assert.IsNotNull(layer);
            Assert.AreEqual(8, mesh.TotalLayers);
        }

        [Test]
        public void GetExistingLayer_NotCreated()
        {
            var mesh = new LayeredProcMesh();

            var l0 = mesh.GetLayer(0);
            var l1 = mesh.GetLayer(1);
            var l2 = mesh.GetLayer(2);
            var l3 = mesh.GetLayer(3);

            Assert.AreEqual(l0, mesh.GetLayer(0));
            Assert.AreEqual(l1, mesh.GetLayer(1));
            Assert.AreEqual(l2, mesh.GetLayer(2));
            Assert.AreEqual(l3, mesh.GetLayer(3));

            Assert.AreNotEqual(l0, l1);
            Assert.AreNotEqual(l1, l2);
            Assert.AreNotEqual(l2, l3);
            Assert.AreNotEqual(l3, l0);
        }

        [Test]
        public void ClearAllData_ChildLayers()
        {
            var mesh = new LayeredProcMesh();
            var layer = mesh.GetLayer(3);
            layer.Vertices.Add(new Vec3());

            mesh.Clear();

            Assert.AreEqual(0, layer.Vertices.Count);
        }

        [Test]
        public void GetActiveLayers_GetTotalLayers()
        {
            var mesh = new LayeredProcMesh();

            mesh.GetLayer(0).Triangles.Add(23);
            mesh.GetLayer(3).Triangles.Add(3);
            mesh.GetLayer(4).Triangles.Add(13);

            Assert.AreEqual(3, mesh.ActiveLayers);
            Assert.AreEqual(5, mesh.TotalLayers);
        }
    }
}
