using System;

namespace Data
{
    public class ScorableData<T>: EntityData<T> where T : Enum
    {
        protected readonly int score;
        protected int count;
        
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
