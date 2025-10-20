using Unity.Mathematics;
using UnityEngine;

public static class ChunkDefs
{
    public const int ChunkSizeXZ = 16; // width/length
    public const int ChunkSizeY = 384; // height

    public const int ChunkArea = ChunkSizeXZ * ChunkSizeXZ;
    public const int VoxelsPerChunk = ChunkSizeXZ * ChunkSizeXZ * ChunkSizeY;

    public const int MaxFacesPerVoxel = 6;
    public const int MaxVerticesPerFace = 4;
    public const int MaxIndicesPerFace = 6;

    public const int MaxVerticesPerVoxel = MaxFacesPerVoxel * MaxVerticesPerFace;
    public const int MaxIndicesPerVoxel = MaxFacesPerVoxel * MaxIndicesPerFace;

    // GPU threadgroup sizes
    public const int ThreadsPerGroup = 8; // 8x8x8 = 512 threads; good occupancy on most GPUs

    public static int3 ChunkToVoxelOrigin(int3 chunkCoord)
    {
        return new int3(chunkCoord.x * ChunkSizeXZ, chunkCoord.y * ChunkSizeY, chunkCoord.z * ChunkSizeXZ);
    }
}

public enum ChunkState : byte
{
    None = 0,
    Requested,
    Generating,
    ReadyForMesh,
    MeshingGPU,
    Uploading,
    Active,
    Unloading,
}
