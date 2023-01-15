using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    public PlayerInput PlayerInput { get { return _playerInput; } set { _playerInput = value; } }
    public InputActionMap CurrentMap { get; set; }

    public Vector2 Move;
    public Vector2 Look;
    public Vector2 LookFPS;
    public bool Sprint;
    public bool POV;
    public bool Flashlight;
    public bool Jump;

    private InputActionMap _currentMap = null;
    private InputAction _moveAction = null;
    private InputAction _lookAction = null;
    private InputAction _lookFPSAction = null;
    private InputAction _sprintAction = null;
    private InputAction _povAction = null;
    private InputAction _flashLightAction = null;
    private InputAction _jumpAction = null;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    private void OnEnable()
    {
        _currentMap.Enable();
    }

    private void OnDisable()
    {
        _currentMap.Disable();
    }

    private void Awake()
    {
        _currentMap = _playerInput.currentActionMap;
        _moveAction = _currentMap.FindAction("Move");
        _lookAction = _currentMap.FindAction("Look");
        _lookFPSAction = _currentMap.FindAction("LookFPS");
        _sprintAction = _currentMap.FindAction("Sprint");
        _povAction = _currentMap.FindAction("POV");
        _flashLightAction = _currentMap.FindAction("Flashlight");
        _jumpAction = _currentMap.FindAction("Jump");
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }
    public void LookInput(Vector2 newFloat)
    {
        Look = newFloat;
    }
    public void OnLookFPS(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookFPSInput(value.Get<Vector2>());
        }
    }
    public void LookFPSInput(Vector2 newFloat)
    {
        LookFPS = newFloat;
    }

    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }
    public void MoveInput(Vector2 newFloat)
    {
        Move = newFloat;
    }
    public void OnPOV(InputValue value)
    {
        POVInput(value.isPressed);
    }
    public void POVInput(bool newBool)
    {
        POV = newBool;
    }
    //public void OnFlashlight(InputValue value)
    //{
    //    FlashlightInput(value.isPressed);
    //}
    public void FlashlightInput(bool newBool)
    {
        Flashlight = newBool;
    }
    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }
    public void JumpInput(bool newBool)
    {
        Jump = newBool;
    }
    public void OnSprint(InputValue value) 
    {
        SprintInput(value.isPressed);
    }
    public void SprintInput(bool newBool) 
    {
        Sprint = newBool;
    }


    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
