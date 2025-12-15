using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BspNode
{
    public RectInt rect;
    public BspNode left;
    public BspNode right;

    public RectInt? roomRect;

    public HashSet<Vector2Int> roomTiles;

    public BspNode(RectInt rect)
    {
        this.rect = rect;
    }

    public bool IsLeafNode => (left == null) && (right == null);
    
    public Vector2Int GetCenter()
    {
        if(roomRect.HasValue)
        {
            RectInt hasRect = roomRect.Value;
            int centerX = hasRect.x + (hasRect.width / 2);
            int centerY = hasRect.y + (hasRect.height / 2);
            Vector2Int center = new Vector2Int(centerX, centerY);
            return center;
        }

        if(roomTiles != null && roomTiles.Count > 0)
        {
            int sumX = 0;
            int sumY = 0;

            foreach(Vector2Int roomTile in roomTiles)
            {
                sumX += roomTile.x;
                sumY += roomTile.y;
            }

            int roomCenterX = sumX / roomTiles.Count;
            int roomCenterY = sumY / roomTiles.Count;
            return new Vector2Int(roomCenterX, roomCenterY);
        }

        int leafCenterX = rect.x + rect.width / 2;
        int leafCenterY = rect.y + rect.height / 2;
        return new Vector2Int(leafCenterX, leafCenterY);

    }
}
