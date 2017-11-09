using System;
using Nightingale.Entities;

namespace Nightingale.Tests.DataSources
{
    [Table("")]
    [Description("")]
    public class Song : Entity
    {
        [Description("")]
        [Mapped]
        [FieldType("Artist")]
        [ForeignKey("Artist")]
        [Cascade(Cascade.None)]
        public int? FK_Artist_ID { get; protected set; }

        private Artist _Artist;

        public Artist Artist
        {
            get { return _Artist; }
            set
            {
                _Artist = value;
                FK_Artist_ID = value?.Id;
            }
        }

        protected override void EagerLoadProperties()
        {
            throw new NotImplementedException();
        }
    }
}
