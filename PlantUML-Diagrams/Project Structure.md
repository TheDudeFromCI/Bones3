@startuml

' Bones3Rebuilt

class Bones3Rebuilt.BlockFace
Bones3Rebuilt.BlockFace : +Texture m_Texture
Bones3Rebuilt.BlockFace : +FaceRotation m_Rotation
Bones3Rebuilt.BlockFace --o Bones3Rebuilt.ITexture

class Bones3Rebuilt.BlockType
Bones3Rebuilt.BlockType : +BlockFace[6] m_Faces
Bones3Rebuilt.BlockType : +Int m_ID
Bones3Rebuilt.BlockType : +String m_Name
Bones3Rebuilt.BlockType : +Bool m_IsSolid
Bones3Rebuilt.BlockType : +Bool m_IsVisible
Bones3Rebuilt.BlockType "1" --* "6" Bones3Rebuilt.BlockFace

interface Bones3Rebuilt.IBlockList
Bones3Rebuilt.IBlockList : -List<BlockType> m_BlockTypes
Bones3Rebuilt.IBlockList : +AddBlockType(blockType) void
Bones3Rebuilt.IBlockList : +GetBlockType(id) void
Bones3Rebuilt.IBlockList --o Bones3Rebuilt.BlockType

interface Bones3Rebuilt.ITexture
Bones3Rebuilt.ITexture : +Index

interface Bones3Rebuilt.ITextureAtlas
Bones3Rebuilt.ITextureAtlas : -List<ITexture> m_Textures
Bones3Rebuilt.ITextureAtlas : +AddTexture() ITexture
Bones3Rebuilt.ITextureAtlas : +GetTexture(index) ITexture
Bones3Rebuilt.ITextureAtlas --o Bones3Rebuilt.ITexture

interface Bones3Rebuilt.IBlockContainer
Bones3Rebuilt.IBlockContainer : +GetBlockAt(blockPosition) ushort
Bones3Rebuilt.IBlockContainer : +SetBlockAt(blockPosition, type) void

interface Bones3Rebuilt.IBlockContainerProvider
Bones3Rebuilt.IBlockContainerProvider : +GetContainerAt(containerPosition) IBlockContainer
Bones3Rebuilt.IBlockContainerProvider : +DeleteContainerAt(containerPosition) void
Bones3Rebuilt.IBlockContainerProvider --* Bones3Rebuilt.IBlockContainer

interface Bones3Rebuilt.Remeshing.IChunkProperties
Bones3Rebuilt.Remeshing.IChunkProperties : +GridSize m_ContainerSize
Bones3Rebuilt.Remeshing.IChunkProperties : +GetBlockTypeAt(blockPosition) BlockType
Bones3Rebuilt.Remeshing.IChunkProperties --> Bones3Rebuilt.IBlockList
Bones3Rebuilt.Remeshing.IChunkProperties --o Bones3Rebuilt.BlockType
Bones3Rebuilt.Remeshing.IChunkProperties --> Bones3Rebuilt.IBlockContainer

interface Bones3Rebuilt.Remeshing.IRemeshHandler
Bones3Rebuilt.Remeshing.IRemeshHandler : +RemeshContainer(chunkProperties) void
Bones3Rebuilt.Remeshing.IRemeshHandler --o Bones3Rebuilt.Remeshing.IChunkProperties

class Bones3Rebuilt.WorldContainer
Bones3Rebuilt.WorldContainer : -IBlockContainerProvider m_World
Bones3Rebuilt.WorldContainer : -IBlockList m_BlockList
Bones3Rebuilt.WorldContainer : -IRemeshHandler m_RemeshHandler
Bones3Rebuilt.WorldContainer : +SetBlock(blockPos, type) void
Bones3Rebuilt.WorldContainer : +GetBlock(blockPos) BlockType
Bones3Rebuilt.WorldContainer : +SetBlocks(editBatch) void
Bones3Rebuilt.WorldContainer --o Bones3Rebuilt.IBlockContainerProvider
Bones3Rebuilt.WorldContainer --o Bones3Rebuilt.Remeshing.IRemeshHandler
Bones3Rebuilt.WorldContainer --o Bones3Rebuilt.IBlockList
Bones3Rebuilt.WorldContainer --> Bones3Rebuilt.WorldEditing.IEditBatch

' Bones3Rebuilt.WorldEditing

class Bones3Rebuilt.WorldEditing.NextBlock
Bones3Rebuilt.WorldEditing.NextBlock : +ushort m_ID
Bones3Rebuilt.WorldEditing.NextBlock : +BlockPosition m_Position

interface Bones3Rebuilt.WorldEditing.IEditBatch
Bones3Rebuilt.WorldEditing.IEditBatch : -IFillPattern m_FillPattern
Bones3Rebuilt.WorldEditing.IEditBatch : +GetNextBlock() NextBlock
Bones3Rebuilt.WorldEditing.IEditBatch --* Bones3Rebuilt.WorldEditing.NextBlock
Bones3Rebuilt.WorldEditing.IEditBatch --o Bones3Rebuilt.WorldEditing.IFillPattern

interface Bones3Rebuilt.WorldEditing.IFillPattern
Bones3Rebuilt.WorldEditing.IFillPattern : +GetBlockAt(blockPos) ushort

class Bones3Rebuilt.WorldEditing.FloodFill
Bones3Rebuilt.WorldEditing.FloodFill <|-- Bones3Rebuilt.WorldEditing.IFillPattern

class Bones3Rebuilt.WorldEditing.RandomFill
Bones3Rebuilt.WorldEditing.RandomFill <|-- Bones3Rebuilt.WorldEditing.IFillPattern

class Bones3Rebuilt.WorldEditing.Cuboid
Bones3Rebuilt.WorldEditing.Cuboid <|-- Bones3Rebuilt.WorldEditing.IEditBatch

class Bones3Rebuilt.WorldEditing.Paint
Bones3Rebuilt.WorldEditing.Paint <|-- Bones3Rebuilt.WorldEditing.IEditBatch

class Bones3Rebuilt.WorldEditing.Sphere
Bones3Rebuilt.WorldEditing.Sphere <|-- Bones3Rebuilt.WorldEditing.IEditBatch

' WraithavenGames.Bones3

class WraithavenGames.Bones3.TextureAtlas << (B, #FF7700) MonoBehaviour>>
WraithavenGames.Bones3.TextureAtlas : -ITextureAtlas m_TextureAtlas
WraithavenGames.Bones3.TextureAtlas : +Material m_Material
WraithavenGames.Bones3.TextureAtlas --* Bones3Rebuilt.ITextureAtlas

class WraithavenGames.Bones3.TextureAtlasList << (B, #FF7700) MonoBehaviour>>
WraithavenGames.Bones3.TextureAtlasList : -List<Bones3TextureAtlas> m_TextureAtlasList
WraithavenGames.Bones3.TextureAtlasList --* WraithavenGames.Bones3.TextureAtlas

class WraithavenGames.Bones3.BlockWorld << (B, #FF7700) MonoBehaviour>>
WraithavenGames.Bones3.BlockWorld : -WorldContainer m_WorldContainer
WraithavenGames.Bones3.BlockWorld : +GetBlock(blockPos) BlockType
WraithavenGames.Bones3.BlockWorld : +SetBlock(blockPos, type) void
WraithavenGames.Bones3.BlockWorld --* Bones3Rebuilt.WorldContainer
WraithavenGames.Bones3.BlockWorld --> Bones3Rebuilt.BlockType

@enduml