using System;

namespace Data
{
    public enum EnemyType { Ground, Flying }
    
    [Serializable]
    public class EnemyData: ScorableData<EnemyType>
    {
        public int ScoreOnKilled => score;
        public int KillCount => count;
        
        public float speed = 1f;
        public float damage = 1f;
        public float knockback = 5f;
        
        public void RegisterKill() => Add();

        public EnemyData(EnemyType type, int score, int count = 0) : base(type, score, count) { }
    }
}
