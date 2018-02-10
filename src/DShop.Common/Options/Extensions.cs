using Microsoft.Extensions.Configuration;

namespace DShop.Common.Options
{
    internal static class Extensions
    {
        internal static TModel GetOptions<TModel>(this IConfiguration configuration, string sectionName) where TModel : new()
        {
            var model = new TModel();
            configuration.GetSection(sectionName).Bind(model);
            return model;
        }
    }
}