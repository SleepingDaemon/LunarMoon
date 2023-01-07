using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerInput PlayerInput;

    public Vector2 Move { get; set; }
    public Vector2 Look { get; set; }
    public bool Switch { get; set; }
    public bool FlashLight { get; set; }

    private InputActionMap _currentMap = null;
    private InputAction _moveAction = null;
    private InputAction _lookAction = null;
    private InputAction _switchAction = null;
    private InputAction _flashLightAction = null;

    private void OnEnable() => _currentMap.Enable();

    private void OnDisable() => _currentMap.Disable();

    private void Awake()
    {
        _currentMap = PlayerInput.currentActionMap;
        _moveAction = _currentMap.FindAction("Move");
        _lookAction = _currentMap.FindAction("Look");
        _switchAction = _currentMap.FindAction("Switch");
        _flashLightAction = _currentMap.FindAction("FlashLight");

        _moveAction.performed += OnMove;
        _lookAction.performed += OnLook;
        _switchAction.performed += OnSwitch;
        _flashLightAction.performed += OnFlashLight;

        _moveAction.canceled += OnMove;
        _lookAction.canceled += OnLook;
        _switchAction.canceled += OnSwitch;
        _flashLightAction.canceled += OnFlashLight;
    }

    private void OnLook(InputAction.CallbackContext context) => Look = context.ReadValue<Vector2>();

    private void OnMove(InputAction.CallbackContext context) => Move = context.ReadValue<Vector2>();
    private void OnSwitch(InputAction.CallbackContext context) => Switch = context.ReadValueAsButton();
    private void OnFlashLight(InputAction.CallbackContext context) => FlashLight = context.ReadValueAsButton();
}
