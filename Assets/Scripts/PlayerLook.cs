using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    public static bool FPSMode = true;

    [SerializeField] private CinemachineVirtualCameraBase fpsCam;
    [SerializeField] private CinemachineVirtualCameraBase tpsCam;
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private Transform _fpsCamAnchor;
    [SerializeField] private Transform _tpsCamAnchor;
    [SerializeField] private float _upperLimit = -40f;
    [SerializeField] private float _bottomLimit = 70f;


    private InputManager _input;
    private float _xRot;
    private float _yRot;
    private float mult = 0.01f;
    private float _mouseX;
    private float _mouseY;
    private Vector2 _mousePosFPS;

    private void Start() => _input = GetComponent<InputManager>();

    private void Update()
    {
        _mousePosFPS = Mouse.current.delta.ReadValue();

        OnCamMovement();

        if (FPSMode)
        {
            fpsCam.transform.localRotation = Quaternion.Euler(_xRot, 0, 0);
            transform.rotation = Quaternion.Euler(0, _yRot, 0);
        }
    }

    private void OnCamMovement()
    {
        if (FPSMode)
        {
            fpsCam.enabled = true;
            tpsCam.enabled = false;

            _mouseX = _mousePosFPS.x;
            _mouseY = _mousePosFPS.y;

            fpsCam.transform.position = _fpsCamAnchor.position;

            _yRot += _mouseX * _mouseSensitivity * mult;
            _xRot -= _mouseY * _mouseSensitivity * mult;

            //_xRot -= mouseY * _mouseSensitivity * Time.smoothDeltaTime;
            _xRot = Mathf.Clamp(_xRot, _upperLimit, _bottomLimit);
        }
        else if(!FPSMode)
        {
            fpsCam.enabled = false;
            tpsCam.enabled = true;

            //tpsCam.transform.position = _tpsCamAnchor.position;
        }
    }

    public void OnPOV(InputValue value)
    {
        if(value.isPressed)
            FPSMode = !FPSMode;
    }
}
