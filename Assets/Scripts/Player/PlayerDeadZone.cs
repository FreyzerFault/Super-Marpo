using UnityEngine;

namespace Player
{
    public class PlayerDeadZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) 
                PlayerController.Instance.Die(false);
        }
    }
}
