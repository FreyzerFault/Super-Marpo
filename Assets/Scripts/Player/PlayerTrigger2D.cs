using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class PlayerTrigger: MonoBehaviour
    {
        public UnityEvent onTriggerEnter;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("TRIGGER");
            if (other.CompareTag("Player") || other.transform.parent.CompareTag("Player"))
            {
                Debug.Log("TRIGGER PLAYER");
                HandlePlayerTrigger();
            }
        }

        protected virtual void HandlePlayerTrigger() => onTriggerEnter?.Invoke();
    }
}
