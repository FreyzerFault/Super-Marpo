using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private Vector2 groundDetectionDistance = new(0.1f, 0.02f);
    [SerializeField] private float jumpWindow = 0.2f; // Time to jump after pressing the button
    private float _jumpTimePressed = 0f;
    
    
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _sr;
    
    private float _horizontalInput;
    private bool _jumpInputBuffered;
    private bool _isOnWalkable;
    private bool _lastFrameInWalkable;
    
    private static int WalkableLayer => LayerMask.GetMask("Walkable");
    private bool IsOnWalkable => BoxCheckDownLayer(WalkableLayer, groundDetectionDistance);
    private bool IsOnGround
    {
        get
        {
            RaycastHit2D hit = RayCastDownLayer(WalkableLayer, groundDetectionDistance.y * 2);
            return hit && hit.collider.CompareTag("Ground");
        }
    }

    private RaycastHit2D RayCastDownLayer(int layer, float dist) =>
        Physics2D.Raycast(transform.position, Vector2.down, dist, layer);
    
    private bool BoxCheckDownLayer(int layer, Vector2 boxSize) =>
        Physics2D.OverlapBox((Vector2)transform.position + Vector2.down * groundDetectionDistance.y / 2,
            boxSize, 0, layer);

    private void Awake()
    {
        _rb = GetComponentInChildren<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update() => UpdateAnimator();

    private void FixedUpdate()
    {
        if (!GameManager.Instance.IsPlaying) return;
        
        _isOnWalkable = IsOnWalkable;
        
        Move();
        TryJump();
        
        // Fall on Walkable object
        if (_lastFrameInWalkable != _isOnWalkable && _rb.velocity.y < 0.1f)
        {
            if (_isOnWalkable) OnLand();
            else OnFallOff();
        }

        _lastFrameInWalkable = IsOnWalkable;
    }

    private void Move()
    {
        _rb.velocity = new Vector2(_horizontalInput * moveSpeed * Time.fixedDeltaTime, _rb.velocity.y);
        
        if (_horizontalInput != 0) _sr.flipX = _horizontalInput > 0;
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
        
        bool isGrounded = IsOnWalkable;
        
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

    private void Jump()
    {
        _rb.velocity = Vector2.up * jumpForce;
        PLayHitGroundParticles();
        StopWalkingParticles();
    }

    private void OnLand()
    {
        if (IsOnGround)
        {
            StartWalkingParticles();
            PLayHitGroundParticles();
        }
    }
    
    private void OnFallOff() => StopWalkingParticles();


    private void FallFromPlatform()
    {
        if (!GameManager.Instance.IsPlaying) return;
        RaycastHit2D hit = RayCastDownLayer(WalkableLayer, groundDetectionDistance.y * 5);
        hit.collider.enabled = false;
        StartCoroutine(DeactivatePlatform(hit.collider));
    }

    private IEnumerator DeactivatePlatform(Collider2D platformCol, float waitSeconds = 0.15f)
    {
        platformCol.isTrigger = true;
        yield return new WaitForSeconds(waitSeconds);
        platformCol.isTrigger = false;
    }

    
    #region INPUTS
    
    private void OnMove(InputValue value) => _horizontalInput = value.Get<float>();
    private void OnJump() => _jumpInputBuffered = true;
    private void OnDown() => FallFromPlatform();
    private void OnPause() => GameManager.Instance.TogglePause();

    #endregion

    
    #region ANIMATION

    private static readonly int WalkSpeedID = Animator.StringToHash("WalkSpeed");
    private static readonly int IsGroundedID = Animator.StringToHash("IsGrounded");
    
    private void UpdateAnimator()
    {
        _animator.SetFloat(WalkSpeedID, Mathf.Abs(_horizontalInput));
        _animator.SetBool(IsGroundedID, IsOnWalkable);
    }

    #endregion

    
    #region PARTICLES
    
    #region WALK PARTICLES

    private Vector3 _lastGroundPosition;
    [SerializeField] private ParticleSystem walkParticles;

    /// Start the walking particles system and parent them to the player again
    private void StartWalkingParticles()
    {
        walkParticles.Play();
        walkParticles.transform.parent = transform;
        walkParticles.transform.localPosition = Vector3.zero;
    }

    /// End the walking particles system and unParent it so they don't follow you in the air
    private void StopWalkingParticles()
    {
        walkParticles.Stop();
        walkParticles.transform.parent = null;
    }

    #endregion

    
    #region LAND & JUMP GROUND PARTICLES
    
    [SerializeField] private ParticleSystem groundParticlesPrefab;
    private List<ParticleSystem> _hitParticlesPool = new();
    
    private void PLayHitGroundParticles()
    {
        RaycastHit2D hit = RayCastDownLayer(WalkableLayer, groundDetectionDistance.y * 10f);
        if (hit&& hit.collider.CompareTag($"Ground"))
            PlayParticlesPooled(groundParticlesPrefab, ref _hitParticlesPool, 
                hit.point - (Vector2)hit.transform.position,
                Quaternion.LookRotation(Vector3.forward, hit.normal), hit.transform);
    }

    /// Busca en el pool de particulas alguna que no este emitiendo y la recicla ejecutandola de nuevo
    private static void PlayParticlesPooled(ParticleSystem prefab, ref List<ParticleSystem> pool,
        Vector3 position = default, Quaternion rotation = default, Transform parent = null)
    {
        ParticleSystem pooledParticles = pool.FirstOrDefault(p => !p.isEmitting);
        if (pooledParticles == null)
            pool.Add(pooledParticles = Instantiate(prefab, parent));
        
        pooledParticles.transform.parent = parent;
        pooledParticles.transform.SetLocalPositionAndRotation(position, rotation);
        pooledParticles.Play();
    }
    
    #endregion
    
    #endregion

    
    #region DEBUG

    private void OnDrawGizmos()
    {
        Gizmos.color = _isOnWalkable ? Color.red : Color.yellow;
        Vector3[] quadPoints = {
            transform.position + Vector3.left * groundDetectionDistance.x,
            transform.position + Vector3.right * groundDetectionDistance.x,
            transform.position + Vector3.right * groundDetectionDistance.x + Vector3.down * groundDetectionDistance.y,
            transform.position + Vector3.left * groundDetectionDistance.x + Vector3.down * groundDetectionDistance.y,
        };
        Gizmos.DrawLineStrip(quadPoints, true);
        
        if (_rb == null) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position + new Vector3(_rb.centerOfMass.x, _rb.centerOfMass.y, 0), _rb.velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER ENTER");
    }

    #endregion
}
