using System;

namespace DShop.Common.Types
{
    public interface IIdentifiable
    {
         Guid Id { get; }
    }
}