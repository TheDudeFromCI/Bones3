using NUnit.Framework;
using Bones3Rebuilt;

namespace Test
{
    public class VoxelPositionUtilsTest
    {
        [Test]
        public void ShiftVoxelPosition_XPlus()
        {
            var pos = new BlockPosition();
            pos = pos.ShiftAlongDirection(0);

            Assert.AreEqual(1, pos.X);
            Assert.AreEqual(0, pos.Y);
            Assert.AreEqual(0, pos.Z);
        }

        [Test]
        public void ShiftVoxelPosition_XMinus()
        {
            var pos = new BlockPosition();
            pos = pos.ShiftAlongDirection(1, 1);

            Assert.AreEqual(-1, pos.X);
            Assert.AreEqual(0, pos.Y);
            Assert.AreEqual(0, pos.Z);
        }

        [Test]
        public void ShiftVoxelPosition_YPlus()
        {
            var pos = new BlockPosition();
            pos = pos.ShiftAlongDirection(2, 2);

            Assert.AreEqual(0, pos.X);
            Assert.AreEqual(2, pos.Y);
            Assert.AreEqual(0, pos.Z);
        }

        [Test]
        public void ShiftVoxelPosition_YMinus()
        {
            var pos = new BlockPosition();
            pos = pos.ShiftAlongDirection(3, 3);

            Assert.AreEqual(0, pos.X);
            Assert.AreEqual(-3, pos.Y);
            Assert.AreEqual(0, pos.Z);
        }

        [Test]
        public void ShiftVoxelPosition_ZPlus()
        {
            var pos = new BlockPosition();
            pos = pos.ShiftAlongDirection(4, 4);

            Assert.AreEqual(0, pos.X);
            Assert.AreEqual(0, pos.Y);
            Assert.AreEqual(4, pos.Z);
        }

        [Test]
        public void ShiftVoxelPosition_ZMinus()
        {
            var pos = new BlockPosition();
            pos = pos.ShiftAlongDirection(5, 5);

            Assert.AreEqual(0, pos.X);
            Assert.AreEqual(0, pos.Y);
            Assert.AreEqual(-5, pos.Z);
        }

        [Test]
        public void IsWithinGrid_True()
        {
            var grid = new GridSize(5);
            var pos = new BlockPosition(5, 9, 31);

            Assert.IsTrue(pos.IsWithinGrid(grid));
        }

        [Test]
        public void IsWithinGrid_False()
        {
            var grid = new GridSize(3);
            var pos = new BlockPosition(5, 9, 31);

            Assert.IsFalse(pos.IsWithinGrid(grid));
        }

        [Test]
        public void IsWithinGrid_Negative_False()
        {
            var grid = new GridSize(3);
            var pos = new BlockPosition(1, -2, 2);

            Assert.IsFalse(pos.IsWithinGrid(grid));
        }

        [Test]
        public void IsWithinGrid_Zero_True()
        {
            var grid = new GridSize(3);
            var pos = new BlockPosition();

            Assert.IsTrue(pos.IsWithinGrid(grid));
        }
    }
}