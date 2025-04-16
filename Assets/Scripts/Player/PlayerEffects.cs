using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class PlayerEffects: MonoBehaviour
    {
        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
        }
        
        private void Start()
        {
            _playerController.OnJumpPerformed += HandleJump;
            _playerController.OnBounce += HandleBounce;
            _playerController.OnLand += HandleLand;
            _playerController.OnFallOff += HandleFallOff;
        }
        
        private void HandleJump()
        {
            jumpAudio.Play();
            
            PLayHitGroundParticles();
            StopWalkingParticles();
        }
        
        private void HandleBounce() => bounceAudio.Play();

        private void HandleFallOff() => StopWalkingParticles();

        private void HandleLand()
        {
            if (_playerController.IsOnGround) 
                StartWalkingParticles();
            PLayHitGroundParticles();
        }


        #region AUDIO

        [SerializeField] private AudioSource jumpAudio;
        [SerializeField] private AudioSource bounceAudio;

        #endregion

        

        
        #region PARTICLES
        
            #region WALK PARTICLES

            [Header("VFX")]
            private Vector3 _lastGroundPosition;
            [SerializeField] private ParticleSystem walkParticles;

            /// Start the walking particles system and parent them to the player again
            private void StartWalkingParticles()
            {
                walkParticles.Play();
                walkParticles.transform.parent = transform;
                walkParticles.transform.localPosition = Vector3.zero;
            }

            /// End the walking particles system and unParent it so they don't follow you in the air
            private void StopWalkingParticles()
            {
                walkParticles.Stop();
                walkParticles.transform.parent = null;
            }

            #endregion

        
            #region LAND & JUMP GROUND PARTICLES
            
            [SerializeField] private ParticleSystem groundParticlesPrefab;
            private List<ParticleSystem> _hitParticlesPool = new();
            
            private void PLayHitGroundParticles()
            {
                RaycastHit2D hit = _playerController.RayCastDown(PlayerController.WalkableLayer, 1);
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
        
        #endregion
    }
}
