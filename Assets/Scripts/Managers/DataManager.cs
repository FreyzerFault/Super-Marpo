using System;
using Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Managers
{
    public class DataManager : SingletonPersistent<DataManager>
    {
        [SerializeField] private GameDataSO gameData;
        
        public int Coins => gameData.Coins;
        public int Gems => gameData.Gems;
        public int Score => gameData.TotalScore;

        public event Action<CollectableType> OnCollectableAdded;
        public event Action<EnemyType> OnEnemyKilled;
        
        public EnemyData GetEnemyData(EnemyType type) => gameData.enemies[type];

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        public void RegisterCollectable(CollectableType type)
        {
            gameData.RegisterCollectable(type);
            OnCollectableAdded?.Invoke(type);
        }
        
        public void RegisterKill(EnemyType type)
        {
            gameData.RegisterKill(type);
            OnEnemyKilled?.Invoke(type);
        }
        
        
        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Game") 
                gameData.Reset();
        }
    }
}
