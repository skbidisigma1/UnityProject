using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [Header("References")]
    public Material      BlockMaterial;
    public Transform     PlayerPrefab;

    [Header("Settings")]
    [Min(1)] public int  RenderDistance = 4;          // in chunks (Manhattan)

    readonly Dictionary<Vector2Int, Chunk> chunks = new();
    Transform player;

    #region Bootstrap
    void Start() => StartCoroutine(Bootstrap());

    IEnumerator Bootstrap()
    {
        // generate initial square
        for (int x = -RenderDistance; x <= RenderDistance; x++)
        for (int z = -RenderDistance; z <= RenderDistance; z++)
        {
            MakeChunk(new Vector2Int(x, z));
            yield return null;                        // spread load
        }
        foreach (var c in chunks.Values) c.BuildMeshes();

        player = Instantiate(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        player.GetComponent<BlockInteraction>().World = this;
    }
    #endregion

    /* ---------- public block API ---------- */

    public BlockType GetBlock(Vector3Int world)
    {
        Vector2Int c = new(
            Mathf.FloorToInt((float)world.x / VoxelData.ChunkWidth),
            Mathf.FloorToInt((float)world.z / VoxelData.ChunkWidth));

        if (!chunks.TryGetValue(c, out var chunk)) return BlockType.Air;

        int lx = Mod(world.x, VoxelData.ChunkWidth);
        int lz = Mod(world.z, VoxelData.ChunkWidth);

        int ly = world.y - VoxelData.WorldBottomY;
        if (ly is < 0 or >= VoxelData.ChunkHeight) return BlockType.Air;

        int sc    = ly / VoxelData.SubChunkHeight;
        int lySub = ly % VoxelData.SubChunkHeight;

        return (BlockType)chunk.Subs[sc].Blocks[lx, lySub, lz];
    }

    public void SetBlock(Vector3Int world, BlockType t)
    {
        Vector2Int c = new(
            Mathf.FloorToInt((float)world.x / VoxelData.ChunkWidth),
            Mathf.FloorToInt((float)world.z / VoxelData.ChunkWidth));
        if (!chunks.TryGetValue(c, out var chunk)) return;

        int lx = Mod(world.x, VoxelData.ChunkWidth);
        int lz = Mod(world.z, VoxelData.ChunkWidth);

        int ly = world.y - VoxelData.WorldBottomY;
        if (ly is < 0 or >= VoxelData.ChunkHeight) return;

        int sc    = ly / VoxelData.SubChunkHeight;
        int lySub = ly % VoxelData.SubChunkHeight;

        chunk.Subs[sc].Blocks[lx, lySub, lz] = (byte)t;
        chunk.Subs[sc].BuildMesh();
    }

    /* ---------- helpers ---------- */

    static int Mod(int value, int m) => (value % m + m) % m;   // positive modulo

    public Vector2Int WorldToChunk(Vector3 pos) =>
        new(Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth),
            Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth));

    void MakeChunk(Vector2Int coord)
    {
        var c = new Chunk(this, coord);
        chunks.Add(coord, c);
    }

    /* Update() & chunk unload logic unchanged */
}
