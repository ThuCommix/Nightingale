using System;
using Nightingale.Entities;

namespace Nightingale.Tests.DataSources
{
    [Table("")]
    [Description("")]
    public class Book : Entity
    {
        [Description("")]
        [Mapped]
        [Mandatory]
        [FieldType("Author")]
        [ForeignKey("Author")]
        [Cascade(Cascade.None)]
        public int FK_Author_ID { get; protected set; }

        private Author _Author;

        public Author Author
        {
            get { return _Author; }
            set
            {
                _Author = value;
                FK_Author_ID = value.Id;
            }
        }

        /// <summary>
        /// Loads all properties wich are marked with eager loading.
        /// </summary>
        protected override void EagerLoadProperties()
        {
            throw new NotImplementedException();
        }
    }
}
