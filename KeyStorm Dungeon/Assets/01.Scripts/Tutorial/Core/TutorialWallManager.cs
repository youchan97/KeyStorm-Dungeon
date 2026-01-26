using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWallManager : MonoBehaviour
{
    public static TutorialWallManager Instance { get; private set; }

    [Header("투명벽 리스트")]
    [SerializeField] private List<TutorialWall> walls = new List<TutorialWall>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        CloseAllWalls();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void CloseAllWalls()
    {
        foreach (var wall in walls)
        {
            if (wall != null)
            {
                wall.EnableWall();
            }
        }
    }

    public void OpenWall(int index)
    {
        foreach (var wall in walls)
        {
            if (wall != null && wall.WallIndex == index)
            {
                wall.DisableWall();
                return;
            }
        }
    }

    public void CloseWall(int index)
    {
        foreach (var wall in walls)
        {
            if (wall != null && wall.WallIndex == index)
            {
                wall.EnableWall();
                return;
            }
        }
    }

    public void OpenAllWalls()
    {
        foreach (var wall in walls)
        {
            if (wall != null)
            {
                wall.DisableWall();
            }
        }
    }
}
