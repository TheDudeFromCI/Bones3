using NUnit.Framework;
using Bones3Rebuilt.Remeshing;
using Bones3Rebuilt;
using Moq;

namespace Test
{
    public abstract class VoxelTest
    {
        public ChunkProperties ChunkProperties { get; set; }
        public IMeshBlockDetails AirBlock { get; set; }
        public IMeshBlockDetails NoCollisionBlock { get; set; }
        public IMeshBlockDetails NormalBlock { get; set; }
        public IMeshBlockDetails InvisibleBlock { get; set; }
        public IMeshBlockDetails DifferentMaterialBlock { get; set; }
        public IMeshBlockDetails RandomTexturesBlock { get; set; }

        [SetUp]
        public void Init()
        {
            var airBlock = new Mock<IMeshBlockDetails>();
            airBlock.Setup(b => b.IsSolid).Returns(false);
            airBlock.Setup(b => b.IsVisible).Returns(false);
            AirBlock = airBlock.Object;

            var noCollisionBlock = new Mock<IMeshBlockDetails>();
            noCollisionBlock.Setup(b => b.IsSolid).Returns(false);
            noCollisionBlock.Setup(b => b.IsVisible).Returns(true);
            NoCollisionBlock = noCollisionBlock.Object;

            var normalBlock = new Mock<IMeshBlockDetails>();
            normalBlock.Setup(b => b.IsSolid).Returns(true);
            normalBlock.Setup(b => b.IsVisible).Returns(true);
            NormalBlock = normalBlock.Object;

            var invisibleBlock = new Mock<IMeshBlockDetails>();
            invisibleBlock.Setup(b => b.IsSolid).Returns(true);
            invisibleBlock.Setup(b => b.IsVisible).Returns(false);
            InvisibleBlock = invisibleBlock.Object;

            var differentMaterialBlock = new Mock<IMeshBlockDetails>();
            differentMaterialBlock.Setup(b => b.IsSolid).Returns(true);
            differentMaterialBlock.Setup(b => b.IsVisible).Returns(true);
            differentMaterialBlock.Setup(b => b.GetMaterialID(It.IsAny<int>())).Returns(1);
            DifferentMaterialBlock = differentMaterialBlock.Object;

            var randomTexturesBlock = new Mock<IMeshBlockDetails>();
            randomTexturesBlock.Setup(b => b.IsSolid).Returns(true);
            randomTexturesBlock.Setup(b => b.IsVisible).Returns(true);
            randomTexturesBlock.Setup(b => b.GetRotation(It.IsAny<int>())).Returns(FaceRotation.Random);
            RandomTexturesBlock = randomTexturesBlock.Object;

            ChunkProperties = new ChunkProperties();
            ChunkProperties.Reset(default, new GridSize(4));

            for (int x = -1; x <= 16; x++)
                for (int y = -1; y <= 16; y++)
                    for (int z = -1; z <= 16; z++)
                    {
                        if (IsCorners(x, y, z))
                            continue;

                        ChunkProperties.SetBlock(Pos(x, y, z), AirBlock);
                    }
        }

        private bool IsCorners(int x, int y, int z)
        {
            int n = 0;

            if (x < 0 || x >= 16) n++;
            if (y < 0 || y >= 16) n++;
            if (z < 0 || z >= 16) n++;

            return n > 1;
        }

        public BlockPosition Pos(int x, int y, int z) => new BlockPosition(x, y, z);
    }
}