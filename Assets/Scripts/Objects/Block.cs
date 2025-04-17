using System;
using Data;
using UnityEngine;

namespace Objects
{
    public class Block : MonoBehaviour
    {
        public enum BlockType { Coin, PowerUp, Breakable }

        public BlockType type;
        public GameObject powerUp;
        
        private static readonly int HitID = Animator.StringToHash("Hit");
        private static readonly int BreakID = Animator.StringToHash("Break");
        private static readonly int CoinID = Animator.StringToHash("Coin");
        private static readonly int PowerUpID = Animator.StringToHash("PowerUp");
        private Animator _animator;

        private Collider2D _hitCollider;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _hitCollider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) 
                HandlePlayerHit();
        }

        private void HandlePlayerHit()
        {
            _hitCollider.enabled = false;

            _animator.SetTrigger(type == BlockType.Breakable ? BreakID : HitID);

            switch (type)
            {
                case BlockType.Coin:
                    DataManager.Instance.RegisterCollectable(CollectableType.Coin);
                    _animator.SetTrigger(CoinID);
                    break;
                case BlockType.PowerUp:
                    _animator.SetTrigger(PowerUpID);
                    break;
                case BlockType.Breakable:
                    break;
            }
        }

        public void Destroy() => Destroy(gameObject);

        
        #region DEBUG

        private void OnDrawGizmos()
        {
            switch (type)
            {
                case BlockType.Coin:
                    Gizmos.DrawIcon(transform.position + Vector3.right * 0.05f, "coin");
                    break;
                case BlockType.PowerUp:
                    Gizmos.DrawIcon(transform.position + Vector3.right * 0.05f, "star");
                    break;
                case BlockType.Breakable:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
