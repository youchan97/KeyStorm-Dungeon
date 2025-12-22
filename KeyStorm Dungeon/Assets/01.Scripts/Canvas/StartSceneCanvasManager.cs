using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ConstValue;

public class StartSceneCanvasManager : MonoBehaviour
{
    public void GameStartButton()
    {
        LoadingManager.LoadScene(GameScene);
    }
}
