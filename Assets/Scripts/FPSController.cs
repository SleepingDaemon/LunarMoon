using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    public static bool FPSMode = true;

    [Header("Move and Jump")]
    [SerializeField] private float MoveSpeed = 3f; // movement speed
    [SerializeField] private float SprintSpeed = 5f;
    [SerializeField] private float SpeedChangeRate = 10.0f;
    [SerializeField] private float bunnyHopForce = 3.0f; // force of bunny hop
    [SerializeField] private float jumpForce = 3.0f; // force of jump
    [SerializeField] private float moonGravityFactor = 1 / 6f; // ratio of moon's gravity to Earth's gravity
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float _rayCastDistance = 0.5f;
    [SerializeField] private bool isGrounded = false; // keep track of if the player is on the ground or not
    [SerializeField] private float JumpTimeout = 0.50f;
    [SerializeField] private float FallTimeout = 0.15f;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCameraBase fpsCam;
    [SerializeField] private CinemachineVirtualCameraBase tpsCam;
    [SerializeField] private GameObject CinemachineCameraTarget;
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private Transform _fpsCamAnchor;
    [SerializeField] private Transform _tpsCamAnchor;
    [SerializeField] private float _topClampFPS = -40f;
    [SerializeField] private float _bottomClampFPS = 70f;
    [SerializeField] private float _topClampTPS = 70f;
    [SerializeField] private float _bottomClampTPS = -30f;
    [SerializeField] private bool LockCameraPosition = false;
    [SerializeField] private float CameraAngleOverride = 0.0f;

    [Header("Ground")]
    [SerializeField] private float GroundedOffset = -0.14f;
    [SerializeField] private float GroundedRadius = 0.28f;

    // player
    private float _speed;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // cinemachine
    private const float _threshold = 0.01f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private Rigidbody _rb;
    private InputManager _input;
    private PlayerInput _playerInput;
    private Animator _animator;
    private bool _hasAnimator = false;
    private float _xRot;
    private float _yRot;
    private float mult = 0.01f;
    private float _mouseX;
    private float _mouseY;
    private Vector2 _mousePos;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }

    void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        _playerInput = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody>();
        _input = GetComponent<InputManager>();
        _animator = GetComponent<Animator>();

        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);
        _mousePos = Mouse.current.delta.ReadValue();

        if (FPSMode)
        {
            fpsCam.enabled = true;
            tpsCam.enabled = false;
        }
        else
        {
            fpsCam.enabled = false;
            tpsCam.enabled = true;
        }

        CameraFPSRotation();
        Move();
        HandleJump();
        GroundCheck();
    }

    private void FixedUpdate()
    {
        _rb.AddForce(moonGravityFactor * Physics.gravity, ForceMode.Acceleration);
    }

    private void LateUpdate()
    {
        CameraTPSRotation();
    }

    private void CameraFPSRotation()
    {
        if (FPSMode)
        {
            _mouseX = _mousePos.x;
            _mouseY = _mousePos.y;

            fpsCam.transform.position = _fpsCamAnchor.position;

            _yRot += _mouseX * _mouseSensitivity * mult;
            _xRot -= _mouseY * _mouseSensitivity * mult;
            _xRot = Mathf.Clamp(_xRot, _topClampFPS, _bottomClampFPS);

            fpsCam.transform.localRotation = Quaternion.Euler(_xRot, 0, 0);
            _rb.rotation = Quaternion.Euler(0, _yRot, 0);
        }
    }

    private void CameraTPSRotation()
    {
        if (!FPSMode)
        {
            // if there is an input and camera position is not fixed
            if (_mousePos.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.smoothDeltaTime;

                _cinemachineTargetYaw += _mousePos.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _mousePos.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClampTPS, _topClampTPS);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void Move()
    {
        Debug.DrawRay(transform.position, Vector3.down * _rayCastDistance, Color.red);

        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _input.Sprint ? SprintSpeed : MoveSpeed;

        // if there is no input, set the target speed to 0
        if (_input.Move == Vector2.zero) targetSpeed = 0.0f;

        // calculate movement as a 3D vector
        float horizontal = _input.Move.x;
        float vertical = _input.Move.y;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.Move.magnitude : 1f;

        float currentHorizontalSpeed = new Vector3(horizontal, 0, vertical).magnitude;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(horizontal, 0.0f, vertical).normalized;

        Vector3 forward = fpsCam.transform.forward;
        Vector3 right = fpsCam.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = (forward * inputDirection.z + right * inputDirection.x);

        // move character in the direction of movement vector
        Vector3 targetPosition = transform.position + direction * targetSpeed * Time.deltaTime;
        _rb.MovePosition(targetPosition);

        if(inputDirection != Vector3.zero && isGrounded && _input.Sprint)
        {
            _rb.AddForce(Vector3.up * bunnyHopForce, ForceMode.Impulse);
        }
    }

    private void HandleJump()
    {
        if (isGrounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // handle jumping
            if (_input.Jump && _jumpTimeoutDelta <= 0.0f)
            {
                _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }

            _rb.velocity = Vector3.zero;
        }
        else
        {
            _input.Jump = false;
        }
    }

    bool IsGrounded()
    {
        // check if the character is on the ground
        return Physics.Raycast(transform.position, Vector3.down, _rayCastDistance, groundMask);
    }

    private void GroundCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, groundMask,
            QueryTriggerInteraction.Ignore);
    }

    public void OnPOV(InputValue value)
    {
        if (value.isPressed)
            FPSMode = !FPSMode;
    }
}

