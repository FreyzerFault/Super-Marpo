using System.Linq;
using AYellowpaper.SerializedCollections;
using Managers;
using UnityEngine;
using static Data.CollectableType;
using static Data.EnemyType;

namespace Data
{
    [CreateAssetMenu(fileName = "Game Data", menuName = "Game Data", order = 1)]
    public class GameDataSO : ScriptableObject
    {
        public int initialScore = 2000;

        public void Reset()
        {
            foreach (CollectableData data in collectables.Values) data.Reset();
            foreach (EnemyData data in enemies.Values) data.Reset();
        }
        
        
        #region COLLECTABLES
        
        [Header("Collectables")]
        [SerializedDictionary("Collectable", "Data")]
        public SerializedDictionary<CollectableType, CollectableData> collectables = new() 
        {
            { Coin, new CollectableData ( Coin, 100 ) },
            { Gem, new CollectableData ( Gem, 2000 ) }
        };
        
        public int Coins => collectables[Coin].PickCount;
        public int Gems => collectables[Gem].PickCount;

        public void RegisterCollectable(CollectableType type) => collectables[type].RegisterPickUp();

        #endregion
        

        #region ENEMIES KILLED

        [Header("Enemies")]
        [SerializedDictionary("Enemy", "Data")]
        public SerializedDictionary<EnemyType, EnemyData> enemies = new() 
        {
            { Ground, new EnemyData ( Ground, 500 ) },
            { Flying, new EnemyData ( Flying, 800 ) }
        };
        
        public void RegisterKill(EnemyType type) => enemies[type].RegisterKill();

        #endregion
        
        
        #region SCORE

        private int CollectablesScore => collectables.Aggregate(0, (sum, pair) => pair.Value.TotalScore + sum);
        private int EnemiesScore => enemies.Aggregate(0, (sum, pair) => pair.Value.TotalScore + sum);
        
        private int TimeScoreLost => -Mathf.FloorToInt(GameManager.Instance.GameTime);
        
        public int TotalScore => initialScore + CollectablesScore + EnemiesScore + TimeScoreLost;

        #endregion
    }
}
