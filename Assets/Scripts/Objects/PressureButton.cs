using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Objects
{
    public class PressureButton : MonoBehaviour
    {
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite inactiveSprite;
        [SerializeField] private AudioSource activeAudio;
        [SerializeField] private AudioSource inactiveAudio;
        
        public bool Active { get; private set; }

        public UnityEvent onActivate;
        public UnityEvent onDeactivate;

        private SpriteRenderer _sr;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                Activate();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                Deactivate();
        }

        private void Activate()
        {
            Active = true;
            activeAudio.Play();
            _sr.sprite = activeSprite;
            onActivate?.Invoke();
        }

        private void Deactivate()
        {
            Active = false;
            inactiveAudio.Play();
            _sr.sprite = inactiveSprite;
            onActivate?.Invoke();
        }
    }
}
