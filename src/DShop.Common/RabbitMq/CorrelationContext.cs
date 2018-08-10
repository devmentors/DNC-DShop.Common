using System;
using System.Linq;
using Newtonsoft.Json;

namespace DShop.Common.RabbitMq
{
    public class CorrelationContext : ICorrelationContext
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public Guid ResourceId { get; }
        public string Name { get; }
        public string Origin { get; }
        public string Resource { get; }
        public string Culture { get; }
        public DateTime CreatedAt { get; }

        public CorrelationContext()
        {
        }

        [JsonConstructor]
        private CorrelationContext(Guid id, Guid userId, Guid resourceId, string name,
            string origin, string culture, string resource)
        {
            Id = id;
            UserId = userId;
            ResourceId = resourceId;
            Name = string.IsNullOrWhiteSpace(name) ? string.Empty : GetName(name);
            Origin = string.IsNullOrWhiteSpace(origin) ? string.Empty : 
                origin.StartsWith("/") ? origin.Remove(0, 1) : origin;
            Culture = culture;
            Resource = resource;
            CreatedAt = DateTime.UtcNow;
        }

        public static ICorrelationContext Empty 
            => new CorrelationContext();

        public static ICorrelationContext From<T>(ICorrelationContext context)
            => Create<T>(context.Id, context.UserId, context.ResourceId, context.Origin, context.Culture, context.Resource);

        public static ICorrelationContext Create<T>(Guid id, Guid userId, Guid resourceId, string origin, string culture, string resource = "")
            => new CorrelationContext(id, userId, resourceId, typeof(T).Name, origin, culture, resource);

        private static string GetName(string name)
            => name.Underscore().ToLowerInvariant();
    }
}
