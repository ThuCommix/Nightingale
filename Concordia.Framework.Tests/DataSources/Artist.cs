using System;
using System.Collections.Generic;
using System.Linq;
using Concordia.Framework.Entities;

namespace Concordia.Framework.Tests.DataSources
{
    [Table("")]
    [Description("")]
    public class Artist : Entity
    {
        /// <summary>
		/// 
		/// </summary>
		[Description("")]
        [Mapped]
        [MaxLength(1)]
        [Mandatory]
        [FieldType("string")]
        [Cascade(Cascade.None)]

        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        [Mapped]
        [MaxLength(75)]
        [FieldType("string")]
        [Cascade(Cascade.None)]

        public string Alias { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        [Mapped]
        [DateOnly]
        [FieldType("DateTime")]
        [Cascade(Cascade.None)]

        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        [Mapped]
        [DateOnly]
        [FieldType("DateTime")]
        [Cascade(Cascade.None)]

        public DateTime? DeathDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        [Mapped]
        [MaxLength(255)]
        [FieldType("string")]
        [Cascade(Cascade.None)]

        public string WebLink { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        [Mapped]
        [FieldType("string")]
        [Cascade(Cascade.None)]

        public string Biography { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        [Mapped]
        [MaxLength(255)]
        [FieldType("string")]
        [Cascade(Cascade.None)]

        public string Note { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Description("")]
        [Mapped]
        [MaxLength(75)]
        [FieldType("string")]
        [Cascade(Cascade.None)]

        public string Label { get; set; }

        [Description("")]
        [Mapped]
        [FieldType("Artist")]
        [ForeignKey("AnotherArtist")]
        [Cascade(Cascade.Save)]
        public int? FK_AnotherArtist_ID { get; protected set; }

        private Artist _AnotherArtist;

        public Artist AnotherArtist
        {
            get { return _AnotherArtist; }
            set
            {
                _AnotherArtist = value;
                FK_AnotherArtist_ID = value?.Id;
            }
        }

        [Description("")]
        [Mapped]
        [FieldType("ArtistStatisticValues")]
        [Cascade(Cascade.Save)]
        [ReferenceField("FK_AnotherArtist_ID")]
        public EntityCollection<ArtistStatisticValues> StatisticValues { get; }

        [Description("")]
        [FieldType("string")]
        [VirtualProperty]
        [Expression("TestExpression")]
        public string FullName => $"{this.Name} ({this.Alias})";

        [Description("")]
        [FieldType("ArtistStatisticValues")]
        [VirtualProperty]
        [Expression("TestExpression")]
        public List<ArtistStatisticValues> StatisticValuesWithSecondArtist => this.StatisticValues.Where(x => x.FK_SecondArtist_ID != null).ToList();

        public Artist()
        {
            StatisticValues = new EntityCollection<ArtistStatisticValues>(this, "AnotherArtist");
        }

        protected override void EagerLoadProperties()
        {
        }
    }
}
