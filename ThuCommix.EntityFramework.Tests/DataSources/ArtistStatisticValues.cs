using ThuCommix.EntityFramework.Entities;

namespace ThuCommix.EntityFramework.Tests.DataSources
{
    [Table("")]
    [Description("")]
    public class ArtistStatisticValues : Entity
    {
        [Description("")]
        [Mapped]
        [Mandatory]
        [FieldType("Artist")]
        [ForeignKey("AnotherArtist")]
        [Cascade(Cascade.Save)]
        public int FK_AnotherArtist_ID { get; protected set; }

        public Artist AnotherArtist { get; set; }

        protected override void EagerLoadProperties()
        {
        }
    }
}
