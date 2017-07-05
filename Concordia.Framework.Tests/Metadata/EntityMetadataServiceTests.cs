using NUnit.Framework;
using Concordia.Framework.Metadata;
using Concordia.Framework.Tests.DataSources;

namespace Concordia.Framework.Tests.Metadata
{
    [TestFixture]
    public class EntityMetadataServiceTests
    {
        [SetUp]
        public void Setup()
        {
            DependencyResolver.Clear();
        }

        [Test]
        public void GetEntityMetadata_Works()
        {
            // act
            var metadata = new EntityMetadataService().GetEntityMetadata(typeof(Artist));

            // assert
            Assert.That(metadata.Fields.Count, Is.EqualTo(12));
            Assert.That(metadata.ListFields.Count, Is.EqualTo(1));
            Assert.That(metadata.VirtualFields.Count, Is.EqualTo(1));
            Assert.That(metadata.VirtualListFields.Count, Is.EqualTo(1));
        }
    }
}
