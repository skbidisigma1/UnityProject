// Assets/Scripts/World/SubChunk.cs
using System.Collections.Generic;
using UnityEngine;

public class SubChunk
{
    public readonly Chunk Parent;
    public readonly int   IndexY;                   // 0…23
    public readonly byte[,,] Blocks = new byte[VoxelData.ChunkWidth,
                                               VoxelData.SubChunkHeight,
                                               VoxelData.ChunkWidth];

    GameObject   go;
    MeshFilter   mf;
    MeshCollider mc;

    public SubChunk(Chunk parent, int indexY)
    {
        Parent = parent;
        IndexY = indexY;

        go = new GameObject($"SubChunk_{indexY}");
        go.transform.SetParent(parent.Root.transform, false);
        go.transform.localPosition = new Vector3(0, indexY * VoxelData.SubChunkHeight, 0);

        mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        mr.sharedMaterial = parent.World.BlockMaterial;
        mc = go.AddComponent<MeshCollider>();
    }

    // ------------ generation ------------

    public void PopulateFlat()
    {
        int globalYBase = IndexY * VoxelData.SubChunkHeight + VoxelData.WorldBottomY;

        for (int y = 0; y < VoxelData.SubChunkHeight; y++)
        {
            int worldY = globalYBase + y;

            BlockType t = worldY switch
            {
                0                  => BlockType.Grass,
                >= -3 and < 0      => BlockType.Dirt,
                < -3               => BlockType.Stone,
                _                  => BlockType.Air
            };

            if (t == BlockType.Air) continue;

            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            for (int z = 0; z < VoxelData.ChunkWidth; z++)
                Blocks[x, y, z] = (byte)t;
        }
    }

    // ------------ mesh ------------

    public void BuildMesh()
    {
        var verts = new List<Vector3>();
        var tris  = new List<int>();
        var uvs   = new List<Vector2>();

        // world-space base for this sub-chunk
        int chunkX = Parent.Coord.x * VoxelData.ChunkWidth;
        int chunkZ = Parent.Coord.y * VoxelData.ChunkWidth;
        int subY0  = IndexY * VoxelData.SubChunkHeight + VoxelData.WorldBottomY;

        for (int y = 0; y < VoxelData.SubChunkHeight; y++)
        for (int x = 0; x < VoxelData.ChunkWidth;   x++)
        for (int z = 0; z < VoxelData.ChunkWidth;   z++)
        {
            BlockType block = (BlockType)Blocks[x,y,z];
            if (!BlockUtilities.IsSolid(block)) continue;

            int worldX = chunkX + x;
            int worldY = subY0  + y;
            int worldZ = chunkZ + z;

            for (int face = 0; face < 6; face++)
            {
                // skip faces that have a solid neighbor
                Vector3Int nWorld = new(
                    worldX + (int)VoxelData.FaceChecks[face].x,
                    worldY + (int)VoxelData.FaceChecks[face].y,
                    worldZ + (int)VoxelData.FaceChecks[face].z);

                if (BlockUtilities.IsSolid(Parent.World.GetBlock(nWorld)))
                    continue;

                // build a full quad (4 verts) then 2 tris
                int vertStart = verts.Count;
                Vector3 offset = new Vector3(x, y, z);

                // add the 4 corner verts in CCW order
                verts.Add(VoxelData.Verts[VoxelData.Tris[face, 0]] + offset);
                verts.Add(VoxelData.Verts[VoxelData.Tris[face, 1]] + offset);
                verts.Add(VoxelData.Verts[VoxelData.Tris[face, 2]] + offset);
                verts.Add(VoxelData.Verts[VoxelData.Tris[face, 3]] + offset);

                // triangle 1
                tris.Add(vertStart + 0);
                tris.Add(vertStart + 1);
                tris.Add(vertStart + 2);
                // triangle 2
                tris.Add(vertStart + 0);
                tris.Add(vertStart + 2);
                tris.Add(vertStart + 3);

                // UVs for that face
                uvs.AddRange(BlockUtilities.FaceUVs(block, face));
            }
        }

        var mesh = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        mf.sharedMesh = mesh;
        mc.sharedMesh = mesh;
    }
}
