using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class ScorableData<T>: EntityData<T> where T : Enum
    {
        [SerializeField] protected int score;
        [SerializeField] protected int count;
        
        public int TotalScore => score * count;

        protected void Add(int num = 1) => count += num;
        public void Reset() => count = 0;
        
        protected ScorableData(T type, int score, int count = 0) : base(type)
        {
            this.score = score;
            this.count = count;
        }
    }
}
