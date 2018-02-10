using Microsoft.AspNetCore.Hosting;

namespace DShop.Common.Builders
{
    public interface IServiceBuilder
    {
        IIoCServiceBuilder WithPort(int port);
        IWebHost Build();
    }
}
