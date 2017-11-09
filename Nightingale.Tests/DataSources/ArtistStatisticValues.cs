using Nightingale.Entities;

namespace Nightingale.Tests.DataSources
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

        private Artist _AnotherArtist;

        public Artist AnotherArtist
        {
            get { return _AnotherArtist; }
            set
            {
                _AnotherArtist = value;
                FK_AnotherArtist_ID = value.Id;
            }
        }

        [Description("")]
        [Mapped]
        [FieldType("Artist")]
        [ForeignKey("SecondArtist")]
        [Cascade(Cascade.Save)]
        public int? FK_SecondArtist_ID { get; protected set; }

        private Artist _SecondArtist;

        public Artist SecondArtist
        {
            get { return _SecondArtist; }
            set
            {
                _SecondArtist = value;
                FK_SecondArtist_ID = value?.Id;
            }
        }

        [Description("")]
        [Mapped]
        [Enum]
        [Mandatory]
        [FieldType("StatusCode")]
        [Cascade(Cascade.None)]
        public StatusCode StatusCode { get; set; }

        protected override void EagerLoadProperties()
        {
        }
    }
}
