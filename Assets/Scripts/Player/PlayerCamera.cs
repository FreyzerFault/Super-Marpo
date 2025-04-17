using Cinemachine;
using UnityEngine;

namespace Player
{
    public class PlayerCamera: MonoBehaviour
    {
        private CinemachineVirtualCamera _cam;

        private void Awake()
        {
            _cam = GetComponent<CinemachineVirtualCamera>();
        }

        private void Start()
        {
            PlayerController.Instance.OnDeath += HandlePlayerDeath;
        }

        private void HandlePlayerDeath() => UnstickToPlayer();

        private void UnstickToPlayer() => _cam.Follow = null;
    }
}
