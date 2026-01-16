using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private readonly Vector2 startPos = new Vector2(-2.5f, -7.5f);

    [SerializeField] GameObject playerObj;
    [SerializeField] AttackPoolManager attackPoolManager;
    [SerializeField] EffectPoolManager effectPoolManager;
    [SerializeField] CinemachineVirtualCamera vCam;
    [SerializeField] MinimapCameraManager minimapCameraManager;
    [SerializeField] GameSceneUI gameSceneUI;
    [SerializeField] TutorialManager tutorialManager;
    [SerializeField] CameraManager cameraManager;

    public void SpawnPlayer()
    {
        Player player = Instantiate(playerObj).GetComponent<Player>();
        player.gameObject.transform.position = startPos;
        SettingPlayer(player);
    }

    void SettingPlayer(Player player)
    {
        player.InitAttackPoolManager(attackPoolManager);
        player.EffectPoolManager = effectPoolManager;
        player.GameSceneUI = gameSceneUI;
        //tutorialManager.SetTutorial(player.PlayerController);
        CameraSetting(player);
        gameSceneUI.InitPlayerData(player);
    }

    void CameraSetting(Player player)
    {
        vCam.LookAt = player.transform;
        vCam.Follow = player.transform;
        minimapCameraManager.SetTarget(player);
        cameraManager.SetTarget(player);
    }
}
