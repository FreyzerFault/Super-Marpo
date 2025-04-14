using System;
using System.Collections;
using UnityEngine;

namespace Objects
{
    public class Collectable : MonoBehaviour
    {
        public enum CollectableType { Coin, Gem }
        public CollectableType type ;
        
        private static readonly int CollectID = Animator.StringToHash("Collect");
        
        private Animator _animator;
        private AudioSource _audio;
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _audio = GetComponent<AudioSource>();
            _particleSystem = GetComponent<ParticleSystem>();
        }

        public void Collect()
        {
            ScoreManager.Instance.AddCollectable(type);
            
            GetComponent<Collider2D>().enabled = false;
            _animator.SetTrigger(CollectID);
            _audio.Play();
            _particleSystem.Play();
            
            StartCoroutine(DestroyCoroutine());
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                Collect();
        }

        private IEnumerator DestroyCoroutine(float waitSeconds = 1f)
        {
            yield return new WaitForSeconds(waitSeconds);
            Destroy(gameObject);
        }
    }
}
