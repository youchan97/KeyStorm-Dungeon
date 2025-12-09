using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region InputSystem
    PlayerInput playerInput;
    #endregion

    #region InputActions
    InputAction moveAction;
    InputAction shootAction;
    InputAction bombAction;
    #endregion

    #region events
    public Action OnMove;
    public Action OnShoot;
    public Action OnBomb;
    #endregion

    #region Move관련
    Vector2 moveVec;
    #endregion

    #region Property
    public Vector2 MoveVec { get => moveVec; private set => moveVec = value; }
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
        playerInput = new PlayerInput();
        moveAction = playerInput.Player.Move;
        shootAction = playerInput.Player.Shot;
        bombAction = playerInput.Player.Bomb;
    }

    void InitAction()
    {
        moveAction.performed += MovePerformed;
        moveAction.canceled -= MoveCanceled;
        shootAction.performed += ShootPerformed;
        bombAction.performed += BombPerformed;
    }

    void RemoveAction()
    {
        moveAction.performed -= MovePerformed;
        shootAction.performed-= ShootPerformed;
        bombAction.performed -= BombPerformed;
    }

    public void EnableInput()
    {
        moveAction.Enable();
        shootAction.Enable();
        bombAction.Enable();
    }

    public void DisableInput()
    {
        moveAction.Disable();
        shootAction.Disable();
        bombAction.Disable();
    }

    void MovePerformed(InputAction.CallbackContext context)
    {
        moveVec = context.ReadValue<Vector2>();
        OnMove?.Invoke();
    }

    void MoveCanceled(InputAction.CallbackContext context)
    {
        moveVec = Vector2.zero;
    }

    void ShootPerformed(InputAction.CallbackContext context)
    {
        OnShoot?.Invoke();
    }

    void BombPerformed(InputAction.CallbackContext context)
    {
        OnBomb?.Invoke();
    }
}
