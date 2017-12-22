using System;
using System.Collections.Generic;
using System.Text;
using Nightingale.Entities;

namespace Nightingale.Tests.DataSources
{
    [Table("")]
    [Description("")]
    public class Author : Entity
    {
        /// <summary>
        /// Loads all properties wich are marked with eager loading.
        /// </summary>
        protected override void EagerLoadProperties()
        {
            throw new NotImplementedException();
        }
    }
}
