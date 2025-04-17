using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Enemies
{
    public class JumpableEnemy: Enemy
    {
        protected const float AngleOfHead = 90f;
        
        /// If collision is in the head, bounce the player and die
        /// If not, hit the player
        protected override void HandlePlayerCollision(Collision2D collision)
        {
            //Player may fall downwards to hit the head
            if (collision.relativeVelocity.y < 0)
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    // Check if the contact point is in the head cone with the head angle
                    Vector2 dir = (contact.point - (Vector2)transform.position).normalized;
                    float angle = Vector2.Angle(Vector2.up, dir);
                    if (angle > AngleOfHead / 2)
                        continue;
                    
                    Die();
                    Player.Bounce();
                    return;
                }
            
            // Collision is NOT in the head
            base.HandlePlayerCollision(collision);
        }
        
        
#if UNITY_EDITOR
        #region DEBUG
        
        protected virtual void OnDrawGizmos()
        {
            Handles.color = Color.blue;
            Handles.DrawWireArc(transform.position, Vector3.back, Quaternion.AngleAxis(AngleOfHead / 2, Vector3.forward) * Vector2.up, AngleOfHead, 0.2f);
        }

        #endregion
#endif
    }
}
