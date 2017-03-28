using System;
using ThuCommix.EntityFramework.Entities;

namespace ThuCommix.EntityFramework.Tests.DataSources
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

        public Artist AnotherArtist { get; set; }

        [Description("")]
        [Mapped]
        [FieldType("Artist")]
        [Cascade(Cascade.Save)]
        [ForeignKey("AnotherArtist")]
        public int FK_AnotherArtist_ID { get; protected set; }

        protected override void EagerLoadProperties()
        {
        }
    }
}
