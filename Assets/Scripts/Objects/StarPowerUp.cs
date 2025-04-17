using System.Linq;
using Player;
using UnityEngine;

namespace Objects
{
    public class StarPowerUp: PowerUp
    {
        [SerializeField] private float bounceForce = 5f;
        [SerializeField] private float moveSpeed = 20f;
        private float _moveDir = 1f;

        private void Start()
        {
            Bounce();
        }

        private void FixedUpdate()
        {
            rb.velocity = new Vector2(_moveDir * moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
        }

        protected override void HandleCollisions(Collision2D other)
        {
            if (other.contacts.Any(c => Mathf.Abs(c.point.x - transform.position.x) > 0.1f))
                _moveDir = -_moveDir;
            
            if (other.relativeVelocity.y > 0.01f)
                Bounce();
        }

        protected override void HandlePlayerHit()
        {
            base.HandlePlayerHit();
            PlayerController.Instance.StartInvincibility();
        }

        private void Bounce() => rb.velocity = new Vector2(rb.velocity.x, bounceForce);
    }
}
