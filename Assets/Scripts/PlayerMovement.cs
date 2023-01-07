using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    

    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody _rb;
    private InputManager _input;
    private Vector3 _moveDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _input = GetComponent<InputManager>();
    }

    private void Update()
    {
        _moveDirection = transform.forward * _input.Move.y + transform.right * _input.Move.x;
    }

    private void FixedUpdate()
    {
        OnMovement();
    }

    private void OnMovement()
    {
        _rb.AddForce(_moveDirection.normalized * 10f * moveSpeed, ForceMode.Acceleration);
    }
}
