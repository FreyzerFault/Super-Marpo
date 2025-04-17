using UnityEngine;

namespace Objects
{
    public abstract class PowerUp : MonoBehaviour
    {
        private static readonly int PlayerPickUpID = Animator.StringToHash("PlayerPickUp");
        
        protected Rigidbody2D rb;
        private Animator _animator;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void OnCollisionEnter2D(Collision2D other) => HandleCollisions(other);
        private void OnTriggerEnter2D(Collider2D other) => HandlePlayerHit();

        protected abstract void HandleCollisions(Collision2D other);

        protected virtual void HandlePlayerHit() => _animator.SetTrigger(PlayerPickUpID);

        public void Destroy() => Destroy(gameObject);
    }
}
