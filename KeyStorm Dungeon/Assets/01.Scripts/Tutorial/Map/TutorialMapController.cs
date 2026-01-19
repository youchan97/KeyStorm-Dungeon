using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMapController : MonoBehaviour
{
    [Header("방 참조")]
    [SerializeField] private TutorialRoom startRoom;
    [SerializeField] private TutorialRoom treasureRoom;
    [SerializeField] private TutorialRoom normalRoom;
    [SerializeField] private TutorialRoom shopRoom;
    [SerializeField] private TutorialRoom bossRoom;

    [Header("플레이어")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("포탈")]
    [SerializeField] private GameObject exitPortal;

    private void Start()
    {
        if (player != null && playerSpawnPoint != null)
            player.position = playerSpawnPoint.position;

        exitPortal?.SetActive(false);
    }

    public void ActivateExitPortal() => exitPortal?.SetActive(true);
}

public class TutorialRoom : MonoBehaviour
{
    [SerializeField] private TutorialRoomType roomType;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();
    [SerializeField] private bool spawnEnemiesOnEnter = true;

    private bool isCleared = false;
    private int remainingEnemies = 0;

    public TutorialRoomType RoomType => roomType;
    public Transform SpawnPoint => spawnPoint;

    public event Action OnRoomCleared;

    private void Start()
    {
        foreach (var enemy in enemies)
            enemy?.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        TutorialPlayerHook hook = FindObjectOfType<TutorialPlayerHook>();
        hook?.ReportRoomEnter(roomType);

        if (spawnEnemiesOnEnter && !isCleared)
            SpawnEnemies();
    }

    void SpawnEnemies()
    {
        remainingEnemies = 0;
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.SetActive(true);
                remainingEnemies++;
            }
        }
    }

    public void ReportEnemyKilled()
    {
        remainingEnemies--;
        if (remainingEnemies <= 0 && !isCleared)
        {
            isCleared = true;
            OnRoomCleared?.Invoke();
        }
    }
}

public class TutorialDoor : MonoBehaviour
{
    [SerializeField] private TutorialRoomType roomA;
    [SerializeField] private TutorialRoomType roomB;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private GameObject openVisual;
    [SerializeField] private GameObject closedVisual;
    [SerializeField] private Collider2D doorCollider;

    private void Start() => UpdateVisual();

    public void Open() { isOpen = true; UpdateVisual(); }
    public void Close() { isOpen = false; UpdateVisual(); }

    public bool ConnectsRooms(TutorialRoomType from, TutorialRoomType to)
        => (roomA == from && roomB == to) || (roomA == to && roomB == from);

    void UpdateVisual()
    {
        openVisual?.SetActive(isOpen);
        closedVisual?.SetActive(!isOpen);
        if (doorCollider != null) doorCollider.enabled = !isOpen;
    }
}
