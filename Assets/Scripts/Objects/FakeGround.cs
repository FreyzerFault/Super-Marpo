using DG.Tweening;
using UnityEngine;
using Debug = Debugging.Debug;

namespace Objects
{
    public class FakeGround: MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 0.5f;
        
        private Renderer _renderer;

        private void Awake() => _renderer = GetComponent<Renderer>();

        private void Start()
        {
            _renderer.material.DOFade(1, 0f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) Fade();
        }

        private void Fade() => _renderer.material.DOFade(0.1f, fadeDuration);
    }
}
