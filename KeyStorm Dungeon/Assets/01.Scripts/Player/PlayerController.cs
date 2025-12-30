using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region InputActions
    InputAction moveAction;
    InputAction shootAction;
    InputAction bombAction;
    InputAction useActiveItemAction;
    #endregion

    #region events
    public Action OnMove;
    public Action OnShoot;
    public Action OnBomb;
    public Action OnUseActiveItem;
    #endregion

    #region Move관련
    Vector2 moveVec;
    #endregion

    #region Shoot관련
    string keyName;
    #endregion

    #region Property
    public PlayerInput PlayerInput { get ; private set ; }
    public Vector2 MoveVec { get => moveVec; private set => moveVec = value; }
    public string KeyName { get => keyName; private set => keyName = value; }
    #endregion

    private void Awake()
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
        PlayerInput = new PlayerInput();
        moveAction = PlayerInput.Player.Move;
        shootAction = PlayerInput.Player.Shot;
        bombAction = PlayerInput.Player.Bomb;
        useActiveItemAction = PlayerInput.Player.UseActiveItem;
    }

    void InitAction()
    {
        moveAction.performed += MovePerformed;
        moveAction.canceled += MoveCanceled;
        shootAction.performed += ShootPerformed;
        bombAction.performed += BombPerformed;
        useActiveItemAction.performed += UseActiveItemPerformed;
    }

    void RemoveAction()
    {
        moveAction.performed -= MovePerformed;
        moveAction.canceled -= MoveCanceled;
        shootAction.performed-= ShootPerformed;
        bombAction.performed -= BombPerformed;
        useActiveItemAction.performed -= UseActiveItemPerformed;
    }

    public void EnableInput()
    {
        moveAction.Enable();
        shootAction.Enable();
        bombAction.Enable();
        useActiveItemAction.Enable();
    }

    public void DisableInput()
    {
        moveAction.Disable();
        shootAction.Disable();
        bombAction.Disable();
        useActiveItemAction.Disable();
    }

    void MovePerformed(InputAction.CallbackContext context)
    {
        moveVec = context.ReadValue<Vector2>();
        OnMove?.Invoke();
    }

    void MoveCanceled(InputAction.CallbackContext context)
    {
        moveVec = Vector2.zero;
        OnMove?.Invoke();
    }

    void ShootPerformed(InputAction.CallbackContext context)
    {
        keyName = context.control.displayName;
        OnShoot?.Invoke();
    }

    void BombPerformed(InputAction.CallbackContext context)
    {
        OnBomb?.Invoke();
    }

    void UseActiveItemPerformed(InputAction.CallbackContext context)
    {
        OnUseActiveItem?.Invoke();
    }
}
