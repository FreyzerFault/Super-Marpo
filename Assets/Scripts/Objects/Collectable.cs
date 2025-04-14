using UnityEngine;

namespace Objects
{
    public class Collectable : MonoBehaviour
    {
        public enum CollectableType { Coin, Gem }
        
        public CollectableType collectableType;
        
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

        public void Collect() => ScoreManager.Instance.AddCollectable(collectableType);
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("COIN COLLECTED");
            _animator.SetTrigger(CollectID);
            _audio.Play();
            _particleSystem.Play();
            Collect();
        }
    }
}
