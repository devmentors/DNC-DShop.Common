namespace DShop.Common.Builders
{
    public interface IDatabaseServiceBuilder
    {
        IBusServiceBuilder WithMongoDb(string settingsSectionName);
        IBusServiceBuilder WithNoDatabase();
    }
}