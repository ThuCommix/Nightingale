using Concordia.Framework.Metadata;
using Concordia.Framework.Tests.DataSources;
using Xunit;

namespace Concordia.Framework.Tests.Metadata
{
    public class EntityMetadataServiceTests
    {
        public EntityMetadataServiceTests()
        {
            DependencyResolver.Clear();
        }

        [Fact]
        public void GetEntityMetadata_Works()
        {
            // act
            var metadata = new EntityMetadataService().GetEntityMetadata(typeof(Artist));

            // assert
            Assert.Equal(12, metadata.Fields.Count);
            Assert.Single(metadata.ListFields);
            Assert.Single(metadata.VirtualFields);
            Assert.Single(metadata.VirtualListFields);
        }
    }
}
