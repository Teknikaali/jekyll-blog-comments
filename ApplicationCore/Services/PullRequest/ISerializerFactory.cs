using YamlDotNet.Serialization;

namespace ApplicationCore
{
    /// <summary>
    /// Enables creating a YAML serializer for the comment.
    /// </summary>
    public interface ISerializerFactory
    {
        /// <summary>
        /// Create a YAML serializer for the comment
        /// </summary>
        /// <returns>The YAML serializer.</returns>
        ISerializer BuildSerializer();
    }
}
