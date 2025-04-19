using System;
using System.Collections.Generic;

namespace Data
{
    public abstract class EntityData<T>: IEquatable<EntityData<T>> where T : Enum
    {
        public T type;

        protected EntityData(T type) => this.type = type;


        #region EQUALITY

        public bool Equals(EntityData<T> other) =>
            other is not null &&
            (ReferenceEquals(this, other) || EqualityComparer<T>.Default.Equals(type, other.type));

        public override bool Equals(object obj) =>
            obj is not null &&
            (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((EntityData<T>)obj));

        public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(type);
        
        #endregion
    }
}
