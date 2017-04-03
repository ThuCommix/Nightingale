using NUnit.Framework;
using ThuCommix.EntityFramework.Metadata;
using ThuCommix.EntityFramework.Tests.DataSources;

namespace ThuCommix.EntityFramework.Tests.Metadata
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
