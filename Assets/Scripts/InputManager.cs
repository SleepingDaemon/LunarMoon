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

    private InputActionMap _currentMap = null;
    private InputAction _moveAction = null;
    public InputAction _lookAction = null;

    private void OnEnable() => _currentMap.Enable();

    private void OnDisable() => _currentMap.Disable();

    private void Awake()
    {
        _currentMap = PlayerInput.currentActionMap;
        _moveAction = _currentMap.FindAction("Move");
        _lookAction = _currentMap.FindAction("Look");

        _moveAction.performed += OnMove;
        _lookAction.performed += OnLook;

        _moveAction.canceled += OnMove;
        _lookAction.canceled += OnLook;
    }

    private void OnLook(InputAction.CallbackContext context) => Look = context.ReadValue<Vector2>();

    private void OnMove(InputAction.CallbackContext context) => Move = context.ReadValue<Vector2>();
}
