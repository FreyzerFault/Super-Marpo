using System;
using Player;
using UnityEngine;
using Utils;

namespace Objects
{
    /// <summary>
    /// This make the chosen platform move between two points.
    /// In a given duration.
    /// </summary>
    public class MovingPlatform : MonoBehaviour
    {
        public static PlayerController Player => PlayerController.Instance;
        
        public LoopMoveTween loopMoveTween;
        
        [Header("Sprites")]
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite inactiveSprite;

        public Transform Platform => loopMoveTween.movingObj;
        
        private SpriteRenderer _sr;

        private void Start()
        {
            _sr = GetComponentInChildren<SpriteRenderer>();
            UpdateSprite();
            
            loopMoveTween.Initialize();
        }

        private void OnDestroy() => loopMoveTween.Destroy();

        private void OnCollisionEnter2D(Collision2D other)
        {
            bool playerIsAbovePlatform = other.collider.transform.position.y - Platform.transform.position.y > 0;
            if (other.collider.CompareTag("Player") && loopMoveTween.IsActive && playerIsAbovePlatform)
                ParentPlayerToPlatform();
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            Transform player = other.collider.transform;
            if (other.collider.CompareTag("Player") && player.parent == player.transform)
                UnparentPlayer();
        }
        
        
        
        #region PARENTING

        private Transform _playerLastParent;
        
        /// This make the physics easier while Player is on Platform
        private void ParentPlayerToPlatform()
        {
            _playerLastParent = Player.transform.parent;
            Player.transform.parent = Platform.transform;
        }

        private void UnparentPlayer() => 
            Player.transform.parent = _playerLastParent;

        #endregion

        
        #region ACTIVATION

        public void Toggle(bool on)
        {
            if (on) loopMoveTween.Play();
            else loopMoveTween.Pause();
            
            UpdateSprite();
        }
        
        private void UpdateSprite() => _sr.sprite = loopMoveTween.IsActive ? activeSprite : inactiveSprite;

        #endregion

        
        #region DEBUG

        private void OnDrawGizmos() => loopMoveTween.OnDrawGizmos();

        #endregion
    }
}
