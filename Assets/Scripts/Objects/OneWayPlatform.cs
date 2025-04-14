using UnityEngine;

namespace Objects
{
    public class OneWayPlatform : MonoBehaviour
    {
        private GameObject _player;
        private Collider _collider;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _collider = GetComponent<Collider>();
        }

        private void Update()
        {
            _collider.enabled = (_player.transform.position - transform.position).y > 0;
        }
    }
}
