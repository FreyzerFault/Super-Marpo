using System;
using System.Collections;
using Data;
using Player;
using UnityEngine;
using Debug = Debugging.Debug;


namespace Enemies
{
    public abstract class Enemy : MonoBehaviour
    {
        [HideInInspector] public EnemyType type;
        
        private EnemyData Data => DataManager.Instance.GetEnemyData(type);
        protected float Speed => Data.speed;
        protected float Damage => Data.damage;
        protected float PushForce => Data.pushForce;
        
        protected Vector2 moveDir = Vector2.left;
        protected bool canMove = false;
        protected bool isDead;
        
        protected new Collider2D collider;
        protected Rigidbody2D rb;
        protected SpriteRenderer sr;

        private const float MaxDistanceToPlayerToActivate = 3f;

        // For Lazy Loading the Enemy
        protected bool IsNearPlayer => 
            Vector2.Distance(Player.transform.position, transform.position) < MaxDistanceToPlayerToActivate;

        protected PlayerController Player => PlayerController.Instance;
        
        protected virtual void Awake()
        {
            collider = GetComponent<Collider2D>();
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponentInChildren<SpriteRenderer>();
            _animator = GetComponent<Animator>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player")) 
                HandlePlayerCollision(collision);
            else
                HandleEnvironmentCollision(collision);
        }

        protected virtual void HandlePlayerCollision(Collision2D collision)
        {
            if (Player.IsInvincible)
                Die();
            else
                HitPlayer();
        }

        protected virtual void HandleEnvironmentCollision(Collision2D collision) { }


        #region INTERACTIONS

        public void HitPlayer()
        {
            Vector2 hitDir = ((Player.transform.position - transform.position) * Vector2.right + Vector2.up).normalized;
            Vector2 hitForce = hitDir.normalized * PushForce;
            Player.GetHit(hitForce);
        }
        
        public void Die()
        {
            isDead = true;
            DataManager.Instance.RegisterKill(type);
            
            TriggerDeathAnimation();
        }

        #endregion

        
        #region VISUALS

        private static readonly int DeathID = Animator.StringToHash("Death");
        
        private Animator _animator;

        private void TriggerDeathAnimation()
        {
            rb.bodyType = RigidbodyType2D.Static;
            GetComponentInChildren<Collider2D>().enabled = false;
            _animator.SetTrigger(DeathID);
        }

        private IEnumerator DestroyCoroutine(float waitSeconds = 1f)
        {
            yield return new WaitForSeconds(waitSeconds);
            Destroy(gameObject);
        }

        public void DestroyWhenEndParticles()
        {
            Debug.Log("DESTROY CALLED");
            ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
            StartCoroutine(DestroyOnConditionCoroutine(() => !particles.isPlaying));
        }

        public IEnumerator DestroyOnConditionCoroutine(Func<bool> condition)
        {
            yield return new WaitUntil(condition);
            Destroy(gameObject);
        }

        public void Destroy() => Destroy(gameObject);

        #endregion
    }
}
