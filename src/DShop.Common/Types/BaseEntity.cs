using System;

namespace DShop.Common.Types
{
    public abstract class BaseEntity : IIdentifiable
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedDate { get; protected set; } 
        public DateTime UpdatedDate { get; protected set; }

        public BaseEntity(Guid id)
        {
            Id = id;
            CreatedDate = DateTime.UtcNow;
            SetUpdatedDate();
        }

        protected virtual void SetUpdatedDate()
            => UpdatedDate = DateTime.UtcNow;
    }
}