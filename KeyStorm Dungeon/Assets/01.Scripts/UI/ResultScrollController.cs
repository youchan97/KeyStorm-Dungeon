using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResultScrollController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ScrollRect scrollRect;
    private bool isPointerInside = false;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        if (scrollRect == null)
        {

        }
    }

    void Update()
    {
        if (isPointerInside && scrollRect != null)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(
                    scrollRect.verticalNormalizedPosition + scroll * 0.5f
                );
            }
        }
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
    }
}
