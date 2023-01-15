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
    private InputAction _mouse;
    private float _xRot;
    private float _yRot;
    private float mult = 0.01f;
    private float _mouseX;
    private float _mouseY;
    private Vector2 _mousePosFPS;

    public void OnLookFPS(InputValue value)
    {
        //if (FPSMode)
        //{
        //    Vector2 mouseDelta = context.ReadValue<Vector2>();

        //    fpsCam.transform.Rotate(Vector3.up * mouseDelta.x * _mouseSensitivity * Time.deltaTime);
        //    _xRot -= mouseDelta.y * _mouseSensitivity * Time.deltaTime;
        //    _xRot = Mathf.Clamp(_xRot, _upperLimit, _bottomLimit);
        //    fpsCam.transform.localRotation = Quaternion.Euler(_xRot, 0f, 0f);
        //    transform.rotation = Quaternion.Euler(0, _yRot, 0);
        //}
    }

    private void Start() => _input = GetComponent<InputManager>();

    private void Update()
    {
        //if (FPSMode)
        //{
        //    fpsCam.enabled = true;
        //    tpsCam.enabled = false;
        //}
        //else
        //{
        //    fpsCam.enabled = false;
        //    tpsCam.enabled = true;
        //}

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

            _mouseX = _input.LookFPS.x;
            _mouseY = _input.LookFPS.y;

            fpsCam.transform.position = _fpsCamAnchor.position;

            _yRot += _mouseX * _mouseSensitivity * mult;
            _xRot -= _mouseY * _mouseSensitivity * mult;

            //_xRot -= mouseY * _mouseSensitivity * Time.smoothDeltaTime;
            _xRot = Mathf.Clamp(_xRot, _upperLimit, _bottomLimit);
        }
        else if (!FPSMode)
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
