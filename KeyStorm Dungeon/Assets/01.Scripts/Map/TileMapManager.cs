using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager
{
    private readonly Vector2Int[] directions = new Vector2Int[]
    { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};

    Tilemap groundTileMap;
    Tilemap wallTileMap;
    TileBase groundTile;
    RuleTile wallTile;

    HashSet<Vector2Int> groundTiles = new HashSet<Vector2Int>();


    public TileMapManager(Tilemap ground, Tilemap wallTileMap, TileBase groundTile, RuleTile wallTile)
    {
        this.groundTileMap = ground;
        this.wallTileMap = wallTileMap;
        this.groundTile = groundTile;
        this.wallTile = wallTile;
    }

    public void DrawRooms(BspNode node)
    {
        if(node.IsLeafNode && node.roomTiles != null)
        {
            foreach(Vector2Int roomTile in node.roomTiles)
            {
                Vector3Int pos = new Vector3Int(roomTile.x, roomTile.y, 0);
                groundTileMap.SetTile(pos, groundTile);
            }

            return;
        }

        if (node.left != null) DrawRooms(node.left);
        if (node.right != null) DrawRooms(node.right);
    }

    public void DrawCorridors(List<(Vector2Int, Vector2Int)> corridors)
    {
        foreach (var corridor in corridors)
            DrawLine(corridor.Item1, corridor.Item2);
    }

    void DrawLine(Vector2Int start, Vector2Int end)
    {
        /*int minX = Mathf.Min(vecOne.x, vecTwo.x);
        int maxX = Mathf.Max(vecOne.x, vecTwo.x);
        int minY = Mathf.Min(vecOne.y, vecTwo.y);
        int maxY = Mathf.Max(vecOne.y, vecTwo.y);

        for(int i = minX; i < maxX; i++)
        {
            Vector3Int pos = new Vector3Int(i, vecOne.y, 0);
            corridorTileMap.SetTile(pos, corridorTile);
        }

        for (int i = minY; i < maxY; i++)
        {
            Vector3Int pos = new Vector3Int(vecTwo.x, i, 0);
            corridorTileMap.SetTile(pos, corridorTile);
        }*/
        int x = start.x;
        int y = start.y;

        AddGroundTile(new Vector2Int(x,y));

        while (x != end.x)
        {
            x += (end.x > x) ? 1 : -1;
            AddGroundTile(new Vector2Int(x, y));
        }

        while (y != end.y)
        {
            y += (end.y > y) ? 1 : -1;
            AddGroundTile(new Vector2Int(x, y));
        }
    }

    void AddGroundTile(Vector2Int pos)
    {
        groundTiles.Add(pos);
        groundTileMap.SetTile((Vector3Int)pos, groundTile);
    }

    public void GenerateWalls()
    {
        wallTileMap.ClearAllTiles();
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

        foreach(Vector2Int ground in groundTiles)
        {
            for(int i = 0; i < directions.Length; i++)
            {
                Vector2Int checkWallPos = ground + directions[i];

                if (!groundTiles.Contains(checkWallPos))
                    wallPositions.Add(checkWallPos);
            }
        }

        foreach (Vector2Int wallpos in wallPositions)
            wallTileMap.SetTile((Vector3Int)wallpos, wallTile);
    }
}
