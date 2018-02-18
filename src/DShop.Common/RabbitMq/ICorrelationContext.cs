using System;

namespace DShop.Common.RabbitMq
{
    public interface ICorrelationContext
    {
        Guid Id { get; }
        Guid UserId { get; }
        Guid ResourceId { get; }
        string Name { get; }
        string Origin { get; }
        string Resource { get; }
        string Culture { get; }
        DateTime CreatedAt { get; }
    }
}
