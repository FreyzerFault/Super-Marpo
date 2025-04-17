using System.Collections;
using Data;
using UnityEngine;

namespace Objects
{
    public class CollectableObject : MonoBehaviour
    {
        public const float PlayerMaxDistanceCollider = 2f;
        
        public CollectableType type;

        private Collider2D _collider;
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _audio = GetComponent<AudioSource>();
            _particleSystem = GetComponentInChildren<ParticleSystem>();
            _collider = GetComponent<Collider2D>();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Collect();
            _collider.enabled = false;
        }


        public void Collect()
        {
            DataManager.Instance.RegisterCollectable(type);
            
            TriggerAnimation();
            StartCoroutine(DestroyCoroutine());
        }

        
        #region VISUALS

        private static readonly int CollectID = Animator.StringToHash("Collect");
        
        private Animator _animator;
        private AudioSource _audio;
        private ParticleSystem _particleSystem;
        
        private void TriggerAnimation()
        {
            GetComponent<Collider2D>().enabled = false;
            _animator.SetTrigger(CollectID);
            _audio.Play();
            _particleSystem.Play();
        }

        private IEnumerator DestroyCoroutine(float waitSeconds = 1f)
        {
            yield return new WaitForSeconds(waitSeconds);
            Destroy(gameObject);
        }

        #endregion
    }
}
