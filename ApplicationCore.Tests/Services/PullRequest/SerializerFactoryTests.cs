using Xunit;

namespace ApplicationCore.Tests.Services
{
    public class SerializerFactoryTests
    {
        [Fact]
        public void CreatesSerializer()
        {
            Assert.NotNull(new SerializerFactory().BuildSerializer());
        }
    }
}
