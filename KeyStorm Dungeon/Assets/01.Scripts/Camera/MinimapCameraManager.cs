using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraManager : MonoBehaviour
{
    Player player;

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPos = player.transform.position;
        targetPos.z = transform.position.z;
        transform.position = targetPos;
    }

    public void SetTarget(Player player)
    {
        this.player = player;
    }

    
}
