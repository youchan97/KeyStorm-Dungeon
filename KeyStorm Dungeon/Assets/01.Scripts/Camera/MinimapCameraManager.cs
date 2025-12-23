using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCameraManager : MonoBehaviour
{
    Transform target;


    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position;
        targetPos.z = transform.position.z;
        transform.position = targetPos;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
