using Data;
using UnityEngine;

namespace Enemies
{
    public class GroundEnemy: JumpableEnemy
    {
        protected override void Awake()
        {
            base.Awake();
            type = EnemyType.Ground;
            moveDir = Vector2.left;
        }

        
        #region MOVEMENT

        private void FixedUpdate()
        {
            if (isDead || !IsNearPlayer) return;

            Move(moveDir, Time.fixedDeltaTime);
        }

        private void Move(Vector2 move, float deltaTime)
        {
            rb.velocity = new Vector2(move.x * Speed * deltaTime, rb.velocity.y);
            // sr.flipX = move.x > 0;
            transform.rotation = Quaternion.Euler(0, move.x > 0 ? 180 : 0, 0);
        }

        #endregion


        #region COLLISIONS

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                moveDir = -moveDir;
        }

        #endregion
    }
}
