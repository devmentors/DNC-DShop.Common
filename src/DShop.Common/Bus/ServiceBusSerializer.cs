using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DShop.Common.Bus
{
    internal sealed class ServiceBusSerializer : RawRabbit.Serialization.JsonSerializer
    {
        public ServiceBusSerializer() : base(new Newtonsoft.Json.JsonSerializer
        {
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            Formatting = Formatting.None,
            CheckAdditionalContent = true,
            ContractResolver = new DefaultContractResolver(),
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            NullValueHandling = NullValueHandling.Ignore
        })
        { }
    }
}