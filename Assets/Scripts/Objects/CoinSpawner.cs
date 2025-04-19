using System.Collections;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Objects
{
    public class CoinSpawner : MonoBehaviour
    {
        private PlayerController Player => PlayerController.Instance;
        
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private BoxCollider2D fallArea;
        
        [Tooltip("From where the coins will be shot, towards the RIGHT Vector. Rotate to change direction")]
        [SerializeField] private Transform[] spawnPoints;

        [SerializeField] private Vector2 spawnFrequencyRange;
        [SerializeField] private Vector2Int shootCoinRange;
        [SerializeField] private Vector2 shootForceRange;
        
        [SerializeField] private float shootAngle;

        
        private float RandomSpawnFrequency => Random.Range(spawnFrequencyRange.x, spawnFrequencyRange.y);
        private int RandomNumCoins => Random.Range(shootCoinRange.x, shootCoinRange.y);
        private float RandomShootForce => Random.Range(shootForceRange.x, shootForceRange.y);
        
        private float RandomShotAngle => Random.Range(-shootAngle, shootAngle);
        

        private void Start()
        {
            if (spawnPoints is null or { Length: 0 })
            {
                Debug.LogError("No spawn points assigned to CoinSpawner.");
                return;
            }
            
            StartCoroutine(ShootCoinCoroutine());
        }

        private void OnValidate()
        {
            spawnFrequencyRange.x = Mathf.Max(0.01f, spawnFrequencyRange.x);
            spawnFrequencyRange.y = Mathf.Max(spawnFrequencyRange.x, spawnFrequencyRange.y);
            spawnFrequencyRange.x = Mathf.Min(spawnFrequencyRange.x, spawnFrequencyRange.y);
            
            shootCoinRange.x = Mathf.Max(0, shootCoinRange.x);
            shootCoinRange.y = Mathf.Max(shootCoinRange.x, shootCoinRange.y);
            shootCoinRange.x = Mathf.Min(shootCoinRange.x, shootCoinRange.y);
            
            shootForceRange.x = Mathf.Max(0, shootForceRange.x);
            shootForceRange.y = Mathf.Max(shootForceRange.x, shootForceRange.y);
            shootForceRange.x = Mathf.Min(shootForceRange.x, shootForceRange.y);
        }

        private IEnumerator ShootCoinCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1 / RandomSpawnFrequency);
                ShootCoins(spawnPoints[Random.Range(0, spawnPoints.Length)], RandomNumCoins);
            }
        }

        private void ShootCoins(Transform spawnPoint, int numCoins = 10)
        {
            for (var i = 0; i < numCoins; i++)
            {
                GameObject coin = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity);

                Vector3 shootVector = spawnPoint.right;
                float midShootAngle = Vector3.Angle(Vector3.right, spawnPoint.right);
                Quaternion shootRotation = Quaternion.AngleAxis(RandomShotAngle, Vector3.forward);

                shootVector = shootRotation * shootVector;

                Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
                rb.AddForce(shootVector * RandomShootForce);
            }
        }


#if UNITY_EDITOR
        #region DEBUG
        
        private void OnDrawGizmos()
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(spawnPoint.position, spawnPoint.right * 2);
                
                Color color = Color.yellow;
                color.a = 0.5f;
                Handles.color = color;
                Handles.DrawSolidArc(spawnPoint.position, spawnPoint.forward, spawnPoint.right, shootAngle, 2f);
                Handles.DrawSolidArc(spawnPoint.position, spawnPoint.forward, spawnPoint.right, -shootAngle, 2f);
            }
        }

        #endregion
#endif
    }
}
