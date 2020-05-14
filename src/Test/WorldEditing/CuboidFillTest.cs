using System.Collections.Generic;
using System.Linq;

using Bones3Rebuilt;

using Moq;

using NUnit.Framework;

namespace Test
{
    public class CuboidFillTest
    {
        [Test]
        public void FillRegion()
        {
            var fillPattern = new Mock<IFillPattern>();
            fillPattern.Setup(p => p.GetBlockID(It.IsAny<BlockPosition>())).Returns(2);

            var p1 = new BlockPosition(5, 1, -5);
            var p2 = new BlockPosition(0, 10, -5);

            var tool = new CuboidFill();
            tool.Set(p1, p2, fillPattern.Object);

            List<BlockPlacement> placements = new List<BlockPlacement>();
            foreach (var b in tool.GetBlocks())
                placements.Add(b);

            Assert.AreEqual(60, placements.Count);
            Assert.AreEqual(0, placements.Select(x => x.Position).GroupBy(x => x).Where(g => g.Count() > 1).Count());
            Assert.AreEqual(1, placements.Select(x => x.BlockID).GroupBy(x => x).Count());
            fillPattern.Verify(p => p.GetBlockID(It.IsAny<BlockPosition>()), Times.Exactly(60));
        }
    }
}
