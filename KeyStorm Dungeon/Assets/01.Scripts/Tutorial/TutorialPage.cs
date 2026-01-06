using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TutorialPage
{
    [Header("페이지 정보")]
    public string title;                    

    [Header("예시 화면")]
    public Sprite exampleImage;             

    [Header("설명")]
    [TextArea(5, 10)]
    public string description;              

    [Header("페이지 번호")]
    public int pageNumber;                  
}
