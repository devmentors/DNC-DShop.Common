using Autofac;
using Microsoft.Extensions.Configuration;

namespace DShop.Common.MailKit
{
    public static class Extensions
    {
        public static void AddMailKit(this ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                var options = configuration.GetOptions<MailKitOptions>("mailkit");
                
                return options;
            }).SingleInstance();
        }
    }
}
