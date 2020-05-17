using Bones3Rebuilt;

using WraithavenGames.Bones3;

namespace WraithavenGames.Bones3.Demo
{
    /// <summary>
    /// A simple block loader which builds the demo block types.
    /// </summary>
    public class SimpleBlockList : UnityBlockLoader
    {
        /// <inheritdoc cref="UnityBlockLoader"/>
        public override void LoadBlocks(IBlockTypeList blockList)
        {
            var atlas1 = m_TextureAtlasList.GetAtlas(0);
            var grass = atlas1.GetTexture(0);
            var sideDirt = atlas1.GetTexture(1);
            var dirt = atlas1.GetTexture(2);

            var atlas2 = m_TextureAtlasList.GetAtlas(1);
            var glass = atlas2.GetTexture(0);

            blockList.AddBlockType(new BlockBuilder(blockList.NextBlockID)
                .Name("Grass")
                .Texture(0, sideDirt)
                .Texture(1, sideDirt)
                .Texture(2, grass)
                .Texture(3, dirt)
                .Texture(4, sideDirt)
                .Texture(5, sideDirt)
                .FaceRotation(4, FaceRotation.Clockwise90)
                .FaceRotation(5, FaceRotation.Clockwise270)
                .Build());

            blockList.AddBlockType(new BlockBuilder(blockList.NextBlockID)
                .Name("Glass")
                .Texture(0, glass)
                .Texture(1, glass)
                .Texture(2, glass)
                .Texture(3, glass)
                .Texture(4, glass)
                .Texture(5, glass)
                .FaceRotation(4, FaceRotation.Clockwise90)
                .FaceRotation(5, FaceRotation.Clockwise270)
                .Build());
        }
    }
}
