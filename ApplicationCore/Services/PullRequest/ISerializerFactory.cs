using YamlDotNet.Serialization;

namespace ApplicationCore
{
    public interface ISerializerFactory
    {
        ISerializer BuildSerializer();
    }
}
