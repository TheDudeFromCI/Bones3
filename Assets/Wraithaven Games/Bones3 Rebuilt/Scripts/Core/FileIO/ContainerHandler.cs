using System.IO;
using System.Threading.Tasks;

namespace Bones3Rebuilt
{
    /// <summary>
    /// Used to maintain the saving and loading of block containers.
    /// </summary>
    public class BlockContainerHandler : IFileHandler<IBlockContainer>
    {
        /// <inheritdoc cref="IFileHandler{T}"/>
        public IFileLoadTask<IBlockContainer> Load(string rootFolder, IBlockContainer obj = null)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IFileHandler{T}"/>
        public IFileSaveTask<IBlockContainer> Save(string rootFolder, IBlockContainer obj)
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// A background task for saving block containers.
    /// </summary>
    public class BlockContainerSaveTask : IFileSaveTask<IBlockContainer>
    {
        /// <summary>
        /// An identifier for handling file versioning, to aid in future-proofing.
        /// </summary>
        private const int REGION_FILE_VERSION = 1;

        private readonly Task m_Task;
        private readonly ushort[] m_Blocks;
        private readonly ChunkPosition m_Position;
        private readonly string m_File;
        private readonly GridSize m_Size;

        public BlockContainerSaveTask(string rootFolder, IBlockContainer container)
        {
            m_File = rootFolder + DetermineRegionFile(container.Position);
            m_Size = container.Size;
            m_Position = container.Position;
            m_Blocks = new ushort[m_Size.Volume];

            CopyBlocks(container);

            m_Task = Task.Run(Run);
        }

        /// <summary>
        /// Determines the name of region file based on the chunk position.
        /// </summary>
        /// <param name="chunkPos">The chunk position.</param>
        /// <returns>The region file name.</returns>
        private string DetermineRegionFile(ChunkPosition chunkPos)
        {
            var x = chunkPos.X >> 5;
            var y = chunkPos.Y >> 5;
            var z = chunkPos.Z >> 5;
            return $"/RegionData/{x}-{y}-{z}.dat";
        }

        /// <summary>
        /// Copies all of the block data from the container.
        /// </summary>
        /// <param name="container">The container to read from.</param>
        private void CopyBlocks(IBlockContainer container)
        {
            for (int x = 0; x < m_Size.Value; x++)
            {
                for (int y = 0; y < m_Size.Value; y++)
                {
                    for (int z = 0; z < m_Size.Value; z++)
                    {
                        var pos = new BlockPosition(x, y, z);
                        m_Blocks[pos.Index(m_Size)] = container.GetBlockID(pos);
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to save the block container data to the region file.
        /// </summary>
        /// <exception cref="System.IOException">
        /// If an error occurs while saving the file.
        /// </exception>
        private void Run()
        {
            var fileStream = File.Open(m_File, FileMode.OpenOrCreate);
            using (var writer = new BinaryWriter(fileStream))
            {
                writer.Write(REGION_FILE_VERSION);

                // TODO Create region header
                // TODO Scroll to correct position within file based on header
                // TODO Compress chunk data.

                for (int i = 0; i < m_Blocks.Length; i++)
                    writer.Write(m_Blocks[i]);
            }
        }

        public FileSaveStatus FinishTask()
        {
            try
            {
                m_Task.Wait();
                return new FileSaveStatus(true, null);
            }
            catch (System.Exception e)
            {
                return new FileSaveStatus(false, e);
            }
        }
    }

    public class BlockContainerLoadTask : IFileLoadTask<IBlockContainer>
    {
        public FileLoadStatus FinishTask()
        {
            throw new System.NotImplementedException();
        }
    }
}
