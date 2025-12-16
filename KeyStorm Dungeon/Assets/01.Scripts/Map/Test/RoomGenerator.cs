using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
{
    int minRoomSize;
    int padding;

    public RoomGenerator(int minRoomSize, int padding)
    {
        this.minRoomSize = minRoomSize;
        this.padding = padding;
    }

    public void GenerateRooms(BspNode node)
    {
        if(node.IsLeafNode)
        {
            CreateRoom(node);
            return;
        }

        if (node.left != null) GenerateRooms(node.left);
        if (node.right != null) GenerateRooms(node.right);
    }

    void CreateRoom(BspNode node)
    {
        RectInt leaf = node.rect;

        int totalMinSize = minRoomSize + (padding * 2);
        int rectPadding = padding * 2;

        if(leaf.width < totalMinSize || leaf.height < totalMinSize)
        {
            node.roomRect = null;
            node.roomTiles = null;
            return;
        }

        int randomWidth = Random.Range(minRoomSize, leaf.width - rectPadding + 1);
        int randomHeight = Random.Range(minRoomSize, leaf.height - rectPadding + 1);

        int leatCenterX = leaf.x + leaf.width / 2;
        int leatCenterY = leaf.y + leaf.height / 2;

        int roomX = Mathf.Clamp(leatCenterX - (randomWidth / 2), leaf.x + padding, leaf.xMax - padding - randomWidth);
        int roomY = Mathf.Clamp(leatCenterY - (randomHeight / 2), leaf.y + padding, leaf.yMax - padding - randomHeight);

        RectInt roomRect = new RectInt(roomX, roomY, randomWidth, randomHeight);
        node.roomRect = roomRect;

        HashSet<Vector2Int> tiles = new HashSet<Vector2Int>();

        for(int i = roomRect.xMin; i < roomRect.xMax; i++)
        {
            for(int j = roomRect.yMin; j < roomRect.yMax; j++)
            {
                Vector2Int vec = new Vector2Int(i, j);
                tiles.Add(vec);
            }
        }

        node.roomTiles = tiles;
    }
}

public class CorridorGenerator
{
    public List<(Vector2Int, Vector2Int)> corridors = new List<(Vector2Int, Vector2Int)>();

    public void ConnectRoom(BspNode node)
    {
        if (node.IsLeafNode) return;

        ConnectRoom(node.left);
        ConnectRoom(node.right);

        Vector2Int leftCenter = node.left.GetCenter();
        Vector2Int rightCenter = node.right.GetCenter();

        corridors.Add((leftCenter, rightCenter));
    }
}
