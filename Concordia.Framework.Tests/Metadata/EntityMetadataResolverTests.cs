using System.Linq;
using NUnit.Framework;
using Concordia.Framework.Metadata;

namespace Concordia.Framework.Tests.Metadata
{
    [TestFixture]
    public class EntityMetadataResolverTests
    {
        [SetUp]
        public void Setup()
        {
            DependencyResolver.Clear();
            TestHelper.SetupEntityMetadataServices();
        }

        [Test]
        public void EntityMetadataResolver_Has_All_Entity_Metadata()
        {
            // act
            var result = new EntityMetadataResolver().EntityMetadata;

            // assert
            Assert.That(result.Count(), Is.EqualTo(3));
            Assert.That(result.Any(x => x.Name == "Artist"), Is.True);
            Assert.That(result.Any(x => x.Name == "ArtistStatisticValues"), Is.True);
        }
    }
}
