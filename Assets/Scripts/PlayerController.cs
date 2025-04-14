using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Animator _animator;
    
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private float groundDetectionDistance = 0.02f;
    [SerializeField] private float jumpWindow = 0.2f; // Time to jump after pressing the button
    private float _jumpTimePressed = 0f;
    
    private float _horizontalInput;
    private bool _jumpInputBuffered;
    private bool IsGrounded => Physics2D.Raycast(transform.position, Vector2.down, groundDetectionDistance, LayerMask.GetMask("Map"));

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        Move();
        TryJump();
    }

    private void Move()
    {
        if (_horizontalInput != 0)
            _rb.velocity = new Vector2(_horizontalInput * moveSpeed * Time.fixedDeltaTime, _rb.velocity.y);
    }
    
    private void TryJump()
    {
        if (!_jumpInputBuffered) return;

        // Time Window for Jumping expired => Reset Jump Buffer
        if (_jumpTimePressed > jumpWindow)
        {
            _jumpInputBuffered = false;
            _jumpTimePressed = 0;
            return;
        }
        
        bool isGrounded = IsGrounded;
        
        // Jump pressed and IsGrounded => Jump and Reset Jump Buffer
        if (isGrounded)
        {
            Jump();
            _jumpInputBuffered = false;
            _jumpTimePressed = 0;
            return;
        }
        
        // Timer for Jump Buffer
        _jumpTimePressed += Time.fixedDeltaTime;
    }

    private void Jump() => _rb.velocity = Vector2.up * jumpForce;

    #region INPUTS
    
    private void OnMove(InputValue value) => _horizontalInput = value.Get<float>();
    private void OnJump() => _jumpInputBuffered = true;

    #endregion

    #region ANIMATION

    private static readonly int WalkSpeedID = Animator.StringToHash("WalkSpeed");
    private static readonly int IsGroundedID = Animator.StringToHash("IsGrounded");
    
    private void UpdateAnimator()
    {
        _animator.SetFloat(WalkSpeedID, Mathf.Abs(_horizontalInput));
        _animator.SetBool(IsGroundedID, IsGrounded);
    }

    #endregion


    #region DEBUG

    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.down * groundDetectionDistance);
        
        if (_rb == null) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position + new Vector3(_rb.centerOfMass.x, _rb.centerOfMass.y, 0), _rb.velocity);
    }

    #endregion
}
