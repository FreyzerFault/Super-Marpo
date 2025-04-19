using System.Collections;
using DG.Tweening;
using Managers;
using Player;
using UnityEngine;

namespace Objects
{
    public class GoalFlag : MonoBehaviour
    {
        [SerializeField] private float stopDelay = 0.5f;
        [SerializeField] private float slideDuration = 2f;
        [SerializeField] private float walkSpeed = 1f;

        [SerializeField] private AudioSource slideAudio;
        [SerializeField] private AudioClip music;
        
        private PlayerController Player => PlayerController.Instance;
        private PlayerEffects PlayerEffects => PlayerController.Instance.GetComponent<PlayerEffects>();
        
        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                Player.activeInput = false;

                GameManager.Instance.State = GameManager.GameState.GoalReached;
                
                Rigidbody2D rb = Player.GetComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.velocity = Vector2.zero;

                StartCoroutine(FullSequence());
            }
        }

        private IEnumerator FullSequence()
        {
            yield return new WaitForSeconds(stopDelay);
            
            slideAudio.Play();
            
            yield return SlideDown(slideDuration).WaitForCompletion();
            
            AudioManager.Instance.PlayMusic(music);
            PlayerEffects?.StopBounceAnimation();
            
            yield return new WaitForSeconds(stopDelay);
            
            yield return StartCoroutine(WalkAway(walkSpeed));
        }

        private Tweener SlideDown(float duration) => 
            Player.transform.DOMoveY(_collider.bounds.min.y, duration);

        private IEnumerator WalkAway(float speed = 1f)
        {
            while (true)
            {
                yield return null;
                Player.Move(speed, Time.deltaTime);
            }
        }
    }
}
