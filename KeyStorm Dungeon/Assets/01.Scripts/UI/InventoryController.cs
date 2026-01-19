using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    InventoryInput inventoryInput;

    InputAction selectAction;

    public Action<Vector2> OnSelect;

    private void OnEnable()
    {
        InitInput();
        InitAction();
        EnableInput();
    }

    private void OnDisable()
    {
        RemoveAction();
        DisableInput();
    }  

    void InitInput()
    {
        inventoryInput = new InventoryInput();
        selectAction = inventoryInput.Inventory.Select;
    }
    void InitAction()
    {
        selectAction.performed += SelectPerformed;
    }

    void RemoveAction()
    {
        selectAction.performed -= SelectPerformed;
    }

    public void EnableInput()
    {
        selectAction.Enable();
    }

    public void DisableInput()
    {
        selectAction.Disable();
    }

    void SelectPerformed(InputAction.CallbackContext context)
    {
        Vector2 vec = context.ReadValue<Vector2>();
        OnSelect?.Invoke(vec);
    }
}
