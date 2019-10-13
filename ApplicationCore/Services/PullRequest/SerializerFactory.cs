using YamlDotNet.Serialization;

namespace ApplicationCore
{
    public class SerializerFactory : ISerializerFactory
    {
        public ISerializer BuildSerializer()
        {
            return new SerializerBuilder().Build();
        }
    }
}
