using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InHelmetController : MonoBehaviour
{
    [SerializeField] private Transform _cameraRoot;
    [SerializeField] private Transform _helmet;
    [SerializeField] private float _upperLimit = -40f;
    [SerializeField] private float _bottomLimit = 70f;
    [SerializeField] private float _leftLimit = -40f;
    [SerializeField] private float _rightLimit = 70f;
    [SerializeField] private float _mouseSensitivity = 21.9f;

    private InputManager _input;
    private float _xRot;
    private float _yRot;

    private void Awake() => _input = GetComponent<InputManager>();

    private void Start()
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    private void LateUpdate() => CamMovement();

    private void CamMovement()
    {

        var mouseX = _input.Look.x;
        var mouseY = _input.Look.y;

        _xRot -= mouseY * _mouseSensitivity * Time.smoothDeltaTime;
        _yRot -= mouseX * _mouseSensitivity * Time.smoothDeltaTime;
        _xRot = Mathf.Clamp(_xRot, _upperLimit, _bottomLimit);
        _yRot = Mathf.Clamp(_yRot, _leftLimit, _rightLimit);

        _helmet.localRotation = Quaternion.Euler(_xRot, _yRot, 0);
    }
}
