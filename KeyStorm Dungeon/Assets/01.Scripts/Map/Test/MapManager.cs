using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    BspTreeGenerator bspTreeGenerator;
    RoomGenerator roomGenerator;
    CorridorGenerator corridorGenerator;
    TileMapManager tileMapManager;
    BspNode rootNode;

    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int depth;
    [SerializeField] int minSplit;

    [SerializeField] int minRoomSize;
    [SerializeField] int padding;

    [SerializeField] Tilemap groundTileMap;
    [SerializeField] TileBase groundTile;
    [SerializeField] int corridorWidth;


    private void Start()
    {
        SetBspTree();
        SetRandomWalk();
        SetCorridors();
        SetTileMap();
    }

    void SetBspTree()
    {
        bspTreeGenerator = new BspTreeGenerator(minSplit);
        RectInt rect = new RectInt(0, 0, width, height);
        rootNode = bspTreeGenerator.GenerateTree(rect, depth);
    }

    void SetRandomWalk()
    {
        roomGenerator = new RoomGenerator(minRoomSize, padding);
        roomGenerator.GenerateRooms(rootNode);
    }

    void SetCorridors()
    {
        corridorGenerator = new CorridorGenerator();
        corridorGenerator.ConnectRoom(rootNode);
    }

    void SetTileMap()
    {
        tileMapManager = new TileMapManager(groundTileMap, groundTile, corridorWidth);
        tileMapManager.DrawRooms(rootNode);
        tileMapManager.DrawCorridors(corridorGenerator.corridors);
    }
}
