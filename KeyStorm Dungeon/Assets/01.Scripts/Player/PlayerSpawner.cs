using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private readonly Vector2 startPos = new Vector2(-2.5f, -7.5f);

    [SerializeField] GameObject playerObj;
    [SerializeField] AttackPoolManager attackPoolManager;
    [SerializeField] CinemachineVirtualCamera vCam;
    [SerializeField] MinimapCameraManager minimapCameraManager;
    [SerializeField] GameSceneUI gameSceneUI;

    public void SpawnPlayer()
    {
        Player player = Instantiate(playerObj).GetComponent<Player>();
        player.gameObject.transform.position = startPos;
        SettingPlayer(player);
    }

    void SettingPlayer(Player player)
    {
        player.InitAttackPoolManager(attackPoolManager);
        gameSceneUI.InitPlayerData(player);
        CameraSetting(player);
    }

    void CameraSetting(Player player)
    {
        vCam.LookAt = player.transform;
        vCam.Follow = player.transform;
        minimapCameraManager.SetTarget(player.transform);
    }
}
