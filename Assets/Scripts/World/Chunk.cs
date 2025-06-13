using UnityEngine;

public class Chunk
{
    public readonly World       World;
    public readonly Vector2Int  Coord;          // chunk coordinate (xz)
    public readonly GameObject  Root;
    public readonly SubChunk[]  Subs = new SubChunk[VoxelData.SubChunksPerCol];

    public Chunk(World world, Vector2Int coord)
    {
        World = world;
        Coord = coord;

        /*  ▼▼  KEY FIX  ▼▼
         *  The whole column is now anchored at world Y = -64, so the grass
         *  layer (world-Y = 0) ends up exactly where we expect.           */
        Root = new($"Chunk_{coord.x}_{coord.y}");
        Root.transform.position = new Vector3(
            coord.x * VoxelData.ChunkWidth,
            VoxelData.WorldBottomY,                //  -64
            coord.y * VoxelData.ChunkWidth);

        for (int i = 0; i < Subs.Length; i++)
        {
            Subs[i] = new SubChunk(this, i);
            Subs[i].PopulateFlat();
        }
    }

    public void BuildMeshes()
    {
        foreach (var s in Subs) s.BuildMesh();
    }
}