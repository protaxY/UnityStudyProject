using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float movementFactor;
    [SerializeField] public float airMovementFactor;
    [SerializeField] public float chargeFactor;
    [SerializeField] public float jumpFactor;

    private Rigidbody _rb;
    private Vector3 _inputVelocity;
    private Vector3 _previousInputVelocity;
    private Vector3 _velocity;

    private float _distanseToBottom = 0.5228f;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.Player.Jump.performed += context => Jump();
        //_playerInput.Player.Move.performed += context => Move(context);
        //_playerInput.Player.Charge.performed += context => Charge(context);
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
        _velocity = _rb.velocity;
        //_rb.velocity = Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void FixedUpdate()
    {
        //Move();
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, _distanseToBottom + 0.1f);
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            _rb.AddRelativeForce(jumpFactor * Vector3.up);
            Debug.Log("here2");
        }
        Debug.Log("here");
    }

    private void Move()
    {
        _inputVelocity = _playerInput.Player.Move.ReadValue<Vector2>();
        _inputVelocity = new Vector3(_inputVelocity.x, 0f, _inputVelocity.y);

        if (IsGrounded())
        {
            _inputVelocity *= movementFactor;
        }
        else
        {
            _inputVelocity *= airMovementFactor;
        }
        _inputVelocity = transform.TransformDirection(_inputVelocity);
        Debug.Log(_inputVelocity);
        _rb.velocity -= _previousInputVelocity;
        _rb.velocity += _inputVelocity;

        _previousInputVelocity = _inputVelocity;
    }
}
