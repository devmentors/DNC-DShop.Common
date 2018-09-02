using System;

namespace DShop.Common.Types
{
    public class AggregateId : IEquatable<AggregateId>
    {
        public Guid Value { get; }

        public AggregateId()
        {
            Value = Guid.NewGuid();
        }

        public AggregateId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("Aggregate ID can not be empty.", nameof(value));
            }

            Value = value;
        }

        public bool Equals(AggregateId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AggregateId) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static implicit operator Guid(AggregateId id)
            => id.Value;

        public static implicit operator AggregateId(Guid id)
            => new AggregateId(id);
    }
}