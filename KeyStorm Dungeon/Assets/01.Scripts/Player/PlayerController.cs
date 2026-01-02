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
    InputAction pauseAction;
    #endregion

    #region events
    public Action OnMove;
    public Action OnShoot;
    public Action OnBomb;
    public Action OnUseActiveItem;
    public Action OnPause;
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
        AllEnable();
        Room.OnGameCleared += AllDisable;
    }

    private void OnDisable()
    {
        RemoveAction();
        AllDisable();
        Room.OnGameCleared -= AllDisable;
    }

    void InitInput()
    {
        PlayerInput = new PlayerInput();
        moveAction = PlayerInput.Player.Move;
        shootAction = PlayerInput.Player.Shot;
        bombAction = PlayerInput.Player.Bomb;
        useActiveItemAction = PlayerInput.Player.UseActiveItem;
        pauseAction = PlayerInput.Player.Pause;
    }

    void InitAction()
    {
        moveAction.performed += MovePerformed;
        moveAction.canceled += MoveCanceled;
        shootAction.performed += ShootPerformed;
        bombAction.performed += BombPerformed;
        useActiveItemAction.performed += UseActiveItemPerformed;
        pauseAction.performed += PausePerformed;
    }

    void RemoveAction()
    {
        moveAction.performed -= MovePerformed;
        moveAction.canceled -= MoveCanceled;
        shootAction.performed-= ShootPerformed;
        bombAction.performed -= BombPerformed;
        useActiveItemAction.performed -= UseActiveItemPerformed;
        pauseAction.performed -= PausePerformed;
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

    public void EnablePause() => pauseAction.Enable();

    public void DisablePause() => pauseAction.Disable();

    public void AllEnable()
    {
        EnableInput();
        EnablePause();
    }

    public void AllDisable()
    {
        DisableInput();
        DisablePause();
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

    void PausePerformed(InputAction.CallbackContext context)
    {
        OnPause?.Invoke();
    }
}
