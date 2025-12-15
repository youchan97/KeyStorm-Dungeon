using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager
{
    private readonly Vector2Int[] directions = new Vector2Int[]
    { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};

    private readonly Vector2Int[] diagonalDirections = new Vector2Int[]
    { new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)};

    Tilemap groundTileMap;
    Tilemap wallTileMap;
    TileBase groundTile;
    RuleTile wallTile;

    int corridorWidth;

    HashSet<Vector2Int> groundTiles = new HashSet<Vector2Int>();


    public TileMapManager(Tilemap ground, TileBase groundTile, int corridorWidth)
    {
        this.groundTileMap = ground;
        this.groundTile = groundTile;
        this.corridorWidth = corridorWidth;
    }

    public void DrawRooms(BspNode node)
    {
        if(node.IsLeafNode && node.roomTiles != null)
        {
            foreach(Vector2Int roomTile in node.roomTiles)
            {
                Vector3Int pos = new Vector3Int(roomTile.x, roomTile.y, 0);
                groundTileMap.SetTile(pos, groundTile);
                groundTiles.Add(roomTile);
            }

            return;
        }

        if (node.left != null) DrawRooms(node.left);
        if (node.right != null) DrawRooms(node.right);
    }

    public void DrawCorridors(List<(Vector2Int, Vector2Int)> corridors)
    {
        foreach (var corridor in corridors)
            DrawLine(corridor.Item1, corridor.Item2, corridorWidth);
    }

    void DrawLine(Vector2Int start, Vector2Int end, int width)
    {
        int x = start.x;
        int y = start.y;

        DrawWall(new Vector2Int(x,y), corridorWidth, true);

        while (x != end.x)
        {
            x += (end.x > x) ? 1 : -1;
            DrawWall(new Vector2Int(x, y), corridorWidth, true);
        }

        while (y != end.y)
        {
            y += (end.y > y) ? 1 : -1;
            DrawWall(new Vector2Int(x, y), corridorWidth, false);
        }
    }

    void AddGroundTile(Vector2Int pos)
    {
        groundTiles.Add(pos);
        groundTileMap.SetTile((Vector3Int)pos, groundTile);
    }

    void DrawWall(Vector2Int vec, int width, bool isHorizontal)
    {
        if(width<= 1)
        {
            AddGroundTile(vec);
            return;
        }

        AddGroundTile(vec);

        int halfWidth = width / 2;

        for (int i = 1; i <= halfWidth; i++)
        {
            Vector2Int perVec = isHorizontal ? new Vector2Int(0, i) : new Vector2Int(i, 0);
            AddGroundTile(vec + perVec);
        }
        for (int i = 1; i <= (width - 1) / 2; i++)
        {
            Vector2Int perVec = isHorizontal ? new Vector2Int(0,-i) : new Vector2Int(-i, 0);
            AddGroundTile(vec + perVec);
        }

    }
}
