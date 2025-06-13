using UnityEngine;

public class BlockInteraction : MonoBehaviour
{
    public World     World;
    public float     Reach = 5f;

    readonly BlockType[] hotbar =
    {
        BlockType.Dirt, BlockType.Grass, BlockType.Stone,
        BlockType.OakLog, BlockType.OakLeaves
    };
    int selected;

    void Update()
    {
        // hot‑bar cycling
        if (Input.mouseScrollDelta.y != 0)
            selected = (selected - (int)Mathf.Sign(Input.mouseScrollDelta.y) + hotbar.Length) % hotbar.Length;
        for (int k = 0; k < hotbar.Length && k < 9; k++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + k)) selected = k;

        if (Input.GetMouseButtonDown(0))  BreakBlock();
        if (Input.GetMouseButtonDown(1))  PlaceBlock();
    }

    void BreakBlock()
    {
        if (Physics.Raycast(Camera.main.transform.position,
                Camera.main.transform.forward, out var hit, Reach))
        {
            Vector3Int pos = Vector3Int.RoundToInt(hit.point - hit.normal * 0.5f);
            World.SetBlock(pos, BlockType.Air);
        }
    }

    void PlaceBlock()
    {
        if (Physics.Raycast(Camera.main.transform.position,
                Camera.main.transform.forward, out var hit, Reach))
        {
            Vector3Int pos = Vector3Int.RoundToInt(hit.point + hit.normal * 0.5f);
            World.SetBlock(pos, hotbar[selected]);
        }
    }
}