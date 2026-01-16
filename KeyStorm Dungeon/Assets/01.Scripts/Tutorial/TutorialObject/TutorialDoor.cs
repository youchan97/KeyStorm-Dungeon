using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private GameObject doorSprite;
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private bool isOpen = true;

    void Start()
    {
        if (isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    public void OpenDoor()
    {
        isOpen = true;

        if (doorSprite != null)
        {
            doorSprite.SetActive(false);
        }

        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }

        Debug.Log("[TutorialDoor] 문 열림");
    }

    public void CloseDoor()
    {
        isOpen = false;

        if (doorSprite != null)
        {
            doorSprite.SetActive(true);
        }

        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }

        Debug.Log("[TutorialDoor] 문 닫힘");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isOpen)
        {
            TutorialManager tutorialManager = FindObjectOfType<TutorialManager>();
            if (tutorialManager != null)
            {
                tutorialManager.OnRoomEntered();
            }
        }
    }
}
