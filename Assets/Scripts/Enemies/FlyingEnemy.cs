using Data;
using UnityEngine;

namespace Enemies
{
    public class FlyingEnemy: JumpableEnemy
    {
        private void Start()
        {
            type = EnemyType.Flying;
            moveDir = Vector2.up;
        }
        
        // NOTHING SPECIAL. All is in the Animator
    }
}
