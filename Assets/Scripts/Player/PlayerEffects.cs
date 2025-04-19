using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

namespace Player
{
    public class PlayerEffects: MonoBehaviour
    {
        private PlayerController _playerController;
        private Animator _animator;
        private SpriteRenderer _sr;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _animator = GetComponent<Animator>();
            _sr = GetComponentInChildren<SpriteRenderer>();
        }
        
        private void Start()
        {
            _playerController.OnJumpPerformed += HandleJump;
            _playerController.OnBounce += HandleBounce;
            _playerController.OnLand += HandleLand;
            _playerController.OnDamage += HandleDamage;
            _playerController.OnDeath += HandleDeath;
        }


        private void Update()
        {
            if (_playerController.IsRunning && _playerController.IsOnGround)
                StartWalkingParticles();
            else
                StopWalkingParticles();

            switch (_playerController.IsInvincible)
            {
                case true when !invincibilityParticles.isPlaying:
                    invincibilityParticles.Play();
                    break;
                case false when invincibilityParticles.isPlaying:
                    invincibilityParticles.Stop();
                    break;
            }
        }


        private void HandleJump()
        {
            jumpAudio.Play();
            
            PLayHitGroundParticles();
            StopWalkingParticles();
        }
        
        private void HandleBounce() => bounceAudio.Play();

        private void HandleLand() => PLayHitGroundParticles();

        private void HandleDamage()
        {
            hitAudio.Play();
            
            StartCoroutine(FlickerSpriteInvulnerabilityCoroutine());
        }

        private void HandleDeath() => AudioManager.Instance.PlayMusic(deathMusic);
        

        #region VFX
        
        [SerializeField] private float flickerTickDuration = 0.5f;
        
        private IEnumerator FlickerSpriteInvulnerabilityCoroutine()
        {
            while (_playerController.IsInvulnerable)
            {
                _sr.color = new Color(1, 1, 1, 0.5f);
                yield return new WaitForSeconds(flickerTickDuration * 0.5f);
                _sr.color = new Color(1, 1, 1, 1);
                yield return new WaitForSeconds(flickerTickDuration * 0.5f);
            }
        }

        #endregion
        

        #region AUDIO

        [SerializeField] private AudioSource jumpAudio;
        [SerializeField] private AudioSource bounceAudio;
        [SerializeField] private AudioSource hitAudio;
        
        [SerializeField] private AudioClip deathMusic;

        #endregion
        
        
        #region PARTICLES
        
            #region WALK PARTICLES

            [Header("VFX")]
            private Vector3 _lastGroundPosition;
            [SerializeField] private ParticleSystem walkParticles;

            /// Start the walking particles system and parent them to the player again
            private void StartWalkingParticles()
            {
                // Parent back to the player
                walkParticles.transform.parent = transform;
                walkParticles.transform.localPosition = Vector3.zero;

                StartCoroutine(RestartEmissionAfterAFrameCoroutine());
            }

            private IEnumerator RestartEmissionAfterAFrameCoroutine()
            {
                // Wait a frame to avoid the particles from being emitted in the wrong position
                yield return null;
                
                ParticleSystem.EmissionModule emission = walkParticles.emission;
                emission.enabled = true;
            }

            /// End the walking particles system and unParent it so they don't follow you in the air
            private void StopWalkingParticles()
            {
                // Unparent the particles from the player to the position they are in so they stay on place
                ParticleSystem.EmissionModule emission = walkParticles.emission;
                emission.enabled = false;
                walkParticles.transform.parent = null;
            }

            #endregion

        
            #region LAND & JUMP GROUND PARTICLES
            
            [SerializeField] private ParticleSystem groundParticlesPrefab;
            private List<ParticleSystem> _hitParticlesPool = new();
            
            private void PLayHitGroundParticles()
            {
                RaycastHit2D hit = _playerController.HitGroundUnderneath;
                if (hit)
                    PlayParticlesPooled(groundParticlesPrefab, ref _hitParticlesPool, 
                        hit.point - (Vector2)hit.transform.position,
                        Quaternion.LookRotation(Vector3.forward, hit.normal), hit.transform);
            }

            /// Busca en el pool de particulas alguna que no este emitiendo y la recicla ejecutandola de nuevo
            private static void PlayParticlesPooled(ParticleSystem prefab, ref List<ParticleSystem> pool,
                Vector3 position = default, Quaternion rotation = default, Transform parent = null)
            {
                ParticleSystem pooledParticles = pool.FirstOrDefault(p => !p.isEmitting);
                if (pooledParticles == null)
                    pool.Add(pooledParticles = Instantiate(prefab, parent));
                
                pooledParticles.transform.parent = parent;
                pooledParticles.transform.SetLocalPositionAndRotation(position, rotation);
                pooledParticles.gameObject.SetActive(true);
                pooledParticles.Play();
            }
            
            #endregion


            #region INVINCIBILITY PARTICLES

            [SerializeField] private ParticleSystem invincibilityParticles;

            #endregion
        
        #endregion


        #region ANIMATIONS
        
        private static readonly int MusicBounceID = Animator.StringToHash("MusicBounce");

        public void ToggleBounceAnimation(bool on) => _animator.SetBool(MusicBounceID, on);
        public void StopBounceAnimation() => ToggleBounceAnimation(false);
        public void PlayBounceAnimation() => ToggleBounceAnimation(true);

        #endregion
    }
}
