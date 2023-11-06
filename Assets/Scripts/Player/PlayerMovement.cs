using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public LayerMask whatIsPlayer;
    [SerializeField] public float movementFactor;
    [SerializeField] public float airMovementFactor;
    [SerializeField] public float chargeFactor;
    [SerializeField] public float jumpFactor;
    [SerializeField] public float chargeRecoveryTime;

    LayerMask excludePlayer;
    private Rigidbody _rb;
    private float _distanseToBottom = 0.5228f;
    private PlayerInput _playerInput;
    private bool _isCharging = false;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.Player.Jump.performed += context => Jump();
        _playerInput.Player.Charge.performed += context => StartCoroutine(ChargeAsync()); ;
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        excludePlayer = ~(1 << whatIsPlayer);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private bool IsGrounded()
    {
        //Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.up), Color.blue);
        return Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), _distanseToBottom + 0.01f, excludePlayer);
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            _rb.AddRelativeForce(jumpFactor * Vector3.up, ForceMode.Acceleration);
        }
    }

    private void Move()
    {
        Vector3 inputVelocity = GetInputMovementVector();

        if (IsGrounded())
        {
            inputVelocity *= movementFactor;
        }
        else
        {
            inputVelocity *= airMovementFactor;
        }
        inputVelocity = transform.TransformDirection(inputVelocity);

        _rb.MovePosition(_rb.position + inputVelocity * Time.fixedDeltaTime);
        _rb.AddRelativeForce(inputVelocity * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    private IEnumerator ChargeAsync()
    {
        if (!_isCharging)
        {
            Vector3 inputVelocity = Vector3.Normalize(GetInputMovementVector());
            _rb.AddRelativeForce(chargeFactor * inputVelocity, ForceMode.Acceleration);
            _isCharging = true;
            yield return new WaitForSeconds(chargeRecoveryTime);
            _isCharging = false;
        }
    }

    private Vector3 GetInputMovementVector()
    {
        Vector3 inputVector = _playerInput.Player.Move.ReadValue<Vector2>();
        inputVector = new Vector3(inputVector.x, 0f, inputVector.y);
        return inputVector;
    }
}

