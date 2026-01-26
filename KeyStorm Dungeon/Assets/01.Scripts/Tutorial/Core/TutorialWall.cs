using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWall : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private int wallIndex;  

    private BoxCollider2D col;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>();
        }
        col.isTrigger = false;  
    }

    public int WallIndex => wallIndex;

    public void EnableWall()
    {
        col.enabled = true;
    }

    public void DisableWall()
    {
        col.enabled = false;
    }
}
