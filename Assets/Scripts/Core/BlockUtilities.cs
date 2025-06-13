using UnityEngine;

public static class BlockUtilities
{
    public const int AtlasSizeInBlocks = 16;
    public static float UVSize => 1f / AtlasSizeInBlocks;

    public static bool IsSolid(BlockType t) => t != BlockType.Air;

    /// <summary>Returns the atlas tile index (0…255) for this face.</summary>
    public static int TileIndex(BlockType type, int faceIndex)
    {
        switch (type)
        {
            case BlockType.Dirt:      return 0;
            case BlockType.Grass:     return faceIndex switch
            {
                2 => 1,   // top
                3 => 0,   // bottom
                _ => 2    // sides
            };
            case BlockType.Stone:     return 3;
            case BlockType.OakLog:    return (faceIndex == 2 || faceIndex == 3) ? 4 : 5;
            case BlockType.OakLeaves: return 6;
            default:                  return 0;
        }
    }

    /// <summary>Computes UVs for a face, special-casing Z+ for grass sides.</summary>
    public static Vector2[] FaceUVs(BlockType type, int faceIndex)
    {
        int id = TileIndex(type, faceIndex);
        int x  = id % AtlasSizeInBlocks;
        int y  = id / AtlasSizeInBlocks;
        float uv = UVSize;
        Vector2 uv00 = new Vector2(x * uv, y * uv);
        Vector2 uv11 = uv00 + new Vector2(uv, uv);

        // base UVs: top-right, top-left, bottom-left, bottom-right
        var baseUVs = new[]
        {
            new Vector2(uv11.x, uv11.y),
            new Vector2(uv00.x, uv11.y),
            new Vector2(uv00.x, uv00.y),
            new Vector2(uv11.x, uv00.y)
        };

        // Special-case for grass block sides
        if (type == BlockType.Grass && faceIndex != 2 && faceIndex != 3)
        {
            if (faceIndex == 1) // Z+ (front)
            {
                // rotate 90° counterclockwise (adjust as needed)
                return new[]
                {
                    baseUVs[2],
                    baseUVs[3],
                    baseUVs[0],
                    baseUVs[1]
                };
            }
            else
            {
                // 180° flip for other sides
                return new[]
                {
                    baseUVs[3],
                    baseUVs[0],
                    baseUVs[1],
                    baseUVs[2]
                };
            }
        }
        return baseUVs;
    }
}
