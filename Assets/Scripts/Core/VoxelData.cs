using UnityEngine;

public static class VoxelData
{
    public const int ChunkWidth      = 16;
    public const int ChunkHeight     = 384;           // y-64 … 319
    public const int SubChunkHeight  = 16;
    public const int WorldBottomY    = -64;
    public const int SubChunksPerCol = ChunkHeight / SubChunkHeight;   // 24

    // Cube corner positions
    public static readonly Vector3[] Verts =
    {
        new(0,0,0), new(1,0,0), new(1,1,0), new(0,1,0),
        new(0,0,1), new(1,0,1), new(1,1,1), new(0,1,1)
    };

    /*  CORRECTED quad vertex order (counter-clockwise when viewed from outside):
     *  0  Back  (-Z) : 0,3,2,1
     *  1  Front (+Z) : 4,5,6,7
     *  2  Top   (+Y) : 3,7,6,2
     *  3  Bottom(-Y) : 0,1,5,4
     *  4  Left  (-X) : 4,7,3,0
     *  5  Right (+X) : 1,2,6,5
     */
    public static readonly int[,] Tris =
    {
        {0,3,2,1},   // back
        {4,5,6,7},   // front
        {3,7,6,2},   // top
        {0,1,5,4},   // bottom
        {4,7,3,0},   // left
        {1,2,6,5}    // right
    };

    // Directions used for neighbor checks (back, front, top, bottom, left, right)
    public static readonly Vector3[] FaceChecks =
    {
        new( 0, 0,-1), new( 0, 0, 1), new( 0, 1, 0),
        new( 0,-1, 0), new(-1, 0, 0), new( 1, 0, 0)
    };
}