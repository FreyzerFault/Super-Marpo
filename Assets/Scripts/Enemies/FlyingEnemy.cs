using System;
using Data;
using UnityEngine;
using Utils;

namespace Enemies
{
    public class FlyingEnemy: JumpableEnemy
    {
        [SerializeField] private LoopMoveTween loopMoveTween;

        protected override void Awake()
        {
            base.Awake();
            type = EnemyType.Flying;
            moveDir = Vector2.up;
        }

        private void Start() => loopMoveTween.Initialize();

        private void OnDestroy() => loopMoveTween.Destroy();


#if UNITY_EDITOR
        #region DEBUG

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            loopMoveTween.OnDrawGizmos();
        }

        #endregion
#endif
    }
}
