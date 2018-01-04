using System.Linq;
using Nightingale.Metadata;
using Xunit;

namespace Nightingale.Tests.Metadata
{
    public class EntityMetadataResolverTests
    {
        public EntityMetadataResolverTests()
        {
            DependencyResolver.Clear();
            TestHelper.SetupEntityMetadataServices();
        }

        [Fact]
        public void EntityMetadataResolver_Has_All_Entity_Metadata()
        {
            // act
            var result = new EntityMetadataResolver().EntityMetadata;

            // assert
            Assert.Equal(5, result.Count());
            Assert.Contains(result, x => x.Name == "Artist");
            Assert.Contains(result, x => x.Name == "ArtistStatisticValues");
            Assert.Contains(result, x => x.Name == "Author");
            Assert.Contains(result, x => x.Name == "Book");
            Assert.Contains(result, x => x.Name == "Song");
        }
    }
}
