using System;

namespace Data
{
    public enum CollectableType { Coin, Gem }
    
    [Serializable]
    public class CollectableData: ScorableData<CollectableType>
    {
        public int ScoreOnPickup => score;
        public int PickCount => count;
        
        public void RegisterPickUp() => Add();
        
        public CollectableData(CollectableType type, int score, int count = 0) : base(type, score, count) { }
    }
}
