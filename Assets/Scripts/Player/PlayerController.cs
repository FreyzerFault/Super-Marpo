using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Player
{
    public class PlayerController : Singleton<PlayerController>
    {
        public static int WalkableLayer => LayerMask.GetMask("Ground", "Platform", "Moving Environment");
        public static int PlatformLayer => LayerMask.GetMask("Platform");
        private static bool GamePaused => GameManager.Instance.IsPaused;

        [SerializeField] private GameObject colliders;
    
        [Header("Player Movement")]
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float jumpForce = 1f;
        [SerializeField] private Vector2 groundDetectionBox = new(0.1f, 0.02f);
        [SerializeField] private float jumpTimeWindow = 0.2f; // Time to jump after pressing the button
    
    
        private Rigidbody2D _rb;
        private SpriteRenderer _sr;
    
        private float _horizontalInput;
    
        // Jump Buffer stays active for a short time after pressing the jump button
        private bool _jumpBufferInput;
        private float _jumpTimePressed;
    
        private bool _isOnWalkable;
        private bool _lastFrameInWalkable;
    
        [Header("Special Mechanics")]
        [SerializeField] private float hitDelaySeconds = 1f; // Time to be invincible after getting hit
        [SerializeField] private float invincibleTime = 10f; // Time to be invincible after getting hit
    
        public bool HasShield { get; private set; }
        public bool IsInvulnerable { get; private set; }
        public bool IsInvincible { get; private set; }

        public event Action OnJumpPerformed;
        public event Action OnBounce;
        public event Action OnLand;
        public event Action OnDeath;
    
        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponentInChildren<Rigidbody2D>();
            _sr = GetComponentInChildren<SpriteRenderer>();
            _animator = GetComponent<Animator>();
        }

        private void Update() => UpdateAnimator();

        private void FixedUpdate()
        {
            if (GamePaused) return;
        
            _isOnWalkable = IsOnWalkable;
        
            UpdateBufferJump(Time.fixedDeltaTime);
        
            Move(_horizontalInput, Time.fixedDeltaTime);
        
            if (_jumpBufferInput && _isOnWalkable)
                Jump(jumpForce);

            _lastFrameInWalkable = IsOnWalkable;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            bool isGround = other.otherCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
            if (_rb.velocity.y < 0.1f && isGround) OnLand?.Invoke();
        }


        #region MOVEMENT

        #region HORIZONTAL

        /// Apply Horizontal Velocity and flip the sprite
        public void Move(float horizontalMove, float deltaTime)
        {
            _rb.velocity = new Vector2(horizontalMove * moveSpeed * deltaTime, _rb.velocity.y);
            
            if (horizontalMove != 0) _sr.flipX = horizontalMove > 0;
        }

        #endregion


        #region JUMP

        /// Apply a Vertical Force to Jump
        public void Jump(float force)
        {
            OnJumpPerformed?.Invoke();
        
            // Reset the jump buffer
            _jumpBufferInput = false;
            _jumpTimePressed = 0;
        
            _rb.velocity = Vector2.up * force;
        }
        
        /// If JUMP is pressed, jump with the full force
        public void Bounce()
        {
            OnBounce?.Invoke();
            _rb.velocity = Vector2.up * (_jumpBufferInput ? jumpForce : jumpForce * 0.5f);
        }

        /// The buffer timer increase till the time window is reached, then buffer is reset
        private void UpdateBufferJump(float deltaTime)
        {
            if (!_jumpBufferInput) return;

            // Timer for Jump Buffer
            _jumpTimePressed += deltaTime;
        
            // Time Window for Jumping expired => Reset Jump Buffer
            if (_jumpTimePressed > jumpTimeWindow)
            {
                _jumpBufferInput = false;
                _jumpTimePressed = 0;
            }
        }

        #endregion


        #region FALL OFF PLATFORMS

        /// If a platform is under the Player, fall down, disabling its collider for brief a moment
        private void TryFallFromPlatform()
        {
            if (GamePaused) return;
        
            RaycastHit2D hit = RayCastDown(PlatformLayer, groundDetectionBox.y * 5);
            if (!hit) return;
        
            StartCoroutine(DeactivatePlatform(hit.collider));
        }
    
        private IEnumerator DeactivatePlatform(Collider2D platformCol, float waitSeconds = 0.15f)
        {
            platformCol.isTrigger = true;
            yield return new WaitForSeconds(waitSeconds);
            platformCol.isTrigger = false;
        }

        #endregion

        #endregion


        #region SHIELD

        public void ToggleShield(bool shield) => HasShield = shield;

        #endregion

    
        #region INVINCIBILITY

        public void StartInvincibility()
        {
            StopCoroutine(nameof(InvincibilityCoroutine));
        
            IsInvincible = true;
            HasShield = true;
            _sr.color = Color.yellow;
        
            StartCoroutine(nameof(InvincibilityCoroutine));
        }
    
        private IEnumerator InvincibilityCoroutine()
        {
            IsInvincible = true;
            _sr.color = Color.yellow;
        
            yield return new WaitForSeconds(invincibleTime);
        
            IsInvincible = false;
            _sr.color = Color.white;
        }

        #endregion

    
        #region DAMAGE
    
        public void GetHit(Vector2 hitForce)
        {
            if (IsInvulnerable || IsInvincible) return;
        
            if (HasShield)
            {
                HasShield = false;

                StartCoroutine(InvulnerableFrames(hitDelaySeconds));
                _rb.AddForce(hitForce);
            }
            else
            {
                Die();
            }
        }

        private void Die()
        {
            _animator.SetTrigger(DieID);
            colliders.SetActive(false);
            OnDeath?.Invoke();
            enabled = false;
        }

        private IEnumerator InvulnerableFrames(float seconds)
        {
            _sr.color = new Color(1,1,1,0.5f);
            IsInvulnerable = true;
            StartCoroutine(nameof(FlickerSprite));
            yield return new WaitForSeconds(seconds);
            IsInvulnerable = false;
            _sr.color = Color.white;
        }

        private IEnumerator FlickerSprite(float flickerTimeTime = 0.2f)
        {
            while (IsInvulnerable)
            {
                _sr.color = new Color(1, 1, 1, 0.5f);
                yield return new WaitForSeconds(flickerTimeTime / 2);
                _sr.color = new Color(1, 1, 1, 1);
                yield return new WaitForSeconds(flickerTimeTime / 2);
            }
        }

        #endregion

    
        #region CHECKING ENVIRONMENT
    
        public bool IsOnWalkable => BoxCheckDown(WalkableLayer, groundDetectionBox);
        public bool IsOnGround =>  BoxCheckDown(LayerMask.GetMask("Ground"), groundDetectionBox + Vector2.down * groundDetectionBox.y * 2);

        public RaycastHit2D RayCastDown(int layer, float dist) =>
            Physics2D.Raycast(transform.position, Vector2.down, dist, layer);
    
        public bool BoxCheckDown(int layer, Vector2 boxSize) =>
            Physics2D.OverlapBox((Vector2)transform.position + Vector2.down * groundDetectionBox.y / 2,
                boxSize, 0, layer);
    
        public RaycastHit2D HitGroundUnderneath =>
            Physics2D.BoxCast(transform.position, groundDetectionBox,
                0, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));

        #endregion

    
        #region INPUTS
    
        private void OnMove(InputValue value) => _horizontalInput = value.Get<float>();
        private void OnJump() => _jumpBufferInput = true;
        private void OnDown() => TryFallFromPlatform();
        private void OnPause() => GameManager.Instance.TogglePause();

        #endregion

    
        #region ANIMATION

        private static readonly int WalkSpeedID = Animator.StringToHash("WalkSpeed");
        private static readonly int IsGroundedID = Animator.StringToHash("IsGrounded");
        private static readonly int DieID = Animator.StringToHash("Die");

        private Animator _animator;
        
        private void UpdateAnimator()
        {
            _animator.SetFloat(WalkSpeedID, Mathf.Abs(_horizontalInput));
            _animator.SetBool(IsGroundedID, IsOnWalkable);
        }

        #endregion
    
   
        #region DEBUG

        private void OnDrawGizmos()
        {
            Gizmos.color = _isOnWalkable ? Color.red : Color.yellow;
            Vector3[] quadPoints = {
                transform.position + Vector3.left * groundDetectionBox.x,
                transform.position + Vector3.right * groundDetectionBox.x,
                transform.position + Vector3.right * groundDetectionBox.x + Vector3.down * groundDetectionBox.y,
                transform.position + Vector3.left * groundDetectionBox.x + Vector3.down * groundDetectionBox.y,
            };
            Gizmos.DrawLineStrip(quadPoints, true);
        
            if (_rb == null) return;
        
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position + new Vector3(_rb.centerOfMass.x, _rb.centerOfMass.y, 0), _rb.velocity);
        }

        #endregion
    }
}
