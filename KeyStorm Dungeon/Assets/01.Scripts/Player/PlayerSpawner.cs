using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject playerObj;
    [SerializeField] AttackPoolManager attackPoolManager;
    [SerializeField] CinemachineVirtualCamera vCam;

    public void SpawnPlayer(Vector3 startPos)
    {
        Player player = Instantiate(playerObj).GetComponent<Player>();
        player.gameObject.transform.position = startPos;
        SettingPlayer(player);
    }

    void SettingPlayer(Player player)
    {
        player.InitAttackPoolManager(attackPoolManager);
        vCam.LookAt = player.transform;
        vCam.Follow = player.transform;
    }
}
