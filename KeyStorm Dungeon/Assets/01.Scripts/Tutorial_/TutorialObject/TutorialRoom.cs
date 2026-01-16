using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRoom : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private List<TutorialDoor> doors = new List<TutorialDoor>();
    [SerializeField] private List<TutorialEnemy> enemies = new List<TutorialEnemy>();
    [SerializeField] private bool closeDoorsOnEnter = true;

    private bool roomCleared = false;
    private bool playerInside = false;

    void Start()
    {
        // 시작 시 문 열기
        foreach (var door in doors)
        {
            if (door != null)
            {
                door.OpenDoor();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerInside)
        {
            playerInside = true;
            Debug.Log("[TutorialRoom] 플레이어 진입");

            if (closeDoorsOnEnter)
            {
                CloseAllDoors();
            }
        }
    }

    void Update()
    {
        if (!roomCleared && playerInside)
        {
            CheckRoomCleared();
        }
    }

    void CheckRoomCleared()
    {
        // 모든 적이 처치되었는지 확인
        enemies.RemoveAll(enemy => enemy == null);

        if (enemies.Count == 0)
        {
            OnRoomCleared();
        }
    }

    void CloseAllDoors()
    {
        foreach (var door in doors)
        {
            if (door != null)
            {
                door.CloseDoor();
            }
        }

        Debug.Log("[TutorialRoom] 모든 문이 닫혔습니다");
    }

    void OnRoomCleared()
    {
        roomCleared = true;

        Debug.Log("[TutorialRoom] 방 클리어!");

        // 문 열기
        foreach (var door in doors)
        {
            if (door != null)
            {
                door.OpenDoor();
            }
        }
    }
}
