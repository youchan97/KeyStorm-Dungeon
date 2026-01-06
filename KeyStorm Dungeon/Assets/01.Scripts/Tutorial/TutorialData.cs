using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialPages", menuName = "Tutorial/Tutorial Data", order = 0)]
public class TutorialData : ScriptableObject
{
    [Header("튜토리얼 페이지 목록")]
    public List<TutorialPage> pages = new List<TutorialPage>();

    [Header("설정")]
    [Tooltip("튜토리얼을 본적 있는지 확인하는 PlayerPrefs")]
    public string hasSeenKey = "Tutorial_HasSeen";
}
