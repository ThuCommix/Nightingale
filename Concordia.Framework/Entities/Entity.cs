using System;
using System.Linq;
using System.Linq.Expressions;
using Concordia.Framework.Metadata;
using Concordia.Framework.Queries;
using Concordia.Framework.Sessions;

namespace Concordia.Framework.Entities
{
    public abstract class Entity
    {
        private bool _deleted;
		private int _version;

        /// <summary>
        /// Gets the id.
        /// </summary>
        [Mapped]
        [PrimaryKey]
        [Cascade(Cascade.None)]
        [FieldType("int")]
        [Description("Gets the id.")]
        public int Id { get; internal set; }

        /// <summary>
        /// A value indicating whether the entity is marked as deleted.
        /// </summary>
        [Mapped]
        [Cascade(Cascade.None)]
        [FieldType("bool")]
        [Description("A value indicating whether the entity is marked as deleted.")]
        public bool Deleted
        {
            get { return _deleted; }
            internal set
            {
                PropertyChangeTracker.AddPropertyChangedItem(nameof(Deleted), _deleted, value);
                _deleted = value;
            }
        }

        /// <summary>
        /// A value indicating the version of the entity.
        /// </summary>
        [Mapped]
        [Cascade(Cascade.None)]
        [FieldType("int")]
        [Description("A value indicating the version of the entity.")]
		public int Version 
		{ 
			get { return _version; }
			internal set
			{
                PropertyChangeTracker.AddPropertyChangedItem(nameof(Version), _version, value);
				_version = value;
			}
		}

        /// <summary>
        /// A value indicating whether the entity is saved.
        /// </summary>
        public bool IsSaved => Id != 0;

        /// <summary>
        /// A value indicating whether the entity is not saved.
        /// </summary>
        public bool IsNotSaved => !IsSaved;

        /// <summary>
        /// Gets the property change tracker.
        /// </summary>
        public PropertyChangeTracker PropertyChangeTracker { get; }

        /// <summary>
        /// Gets the session.
        /// </summary>
        protected Session Session { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Concordia.Framework.Entities.Entity"/> is Evicted.
		/// </summary>
		internal bool Evicted { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Concordia.Framework.Entities.Entity"/> class.
        /// </summary>
        protected Entity()
		{
            PropertyChangeTracker = new PropertyChangeTracker(this);
		}

        /// <summary>
        /// Sets the session.
        /// </summary>
        /// <param name="session">The session.</param>
        internal void SetSession(Session session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            Session = session;
        }

        /// <summary>
        /// Loads all properties wich are marked with eager loading.
        /// </summary>
        internal void EagerLoadPropertiesInternal()
        {
            EagerLoadProperties();
        }

        /// <summary>
        /// Loads all properties wich are marked with eager loading.
        /// </summary>
        protected abstract void EagerLoadProperties();

        /// <summary>
        /// Validates the entity.
        /// </summary>
        /// <returns>Returns true if the entity is valid and ready to save.</returns>
        public virtual bool Validate()
        {
            var properties = ReflectionHelper.GetProperties(GetType());
            var metadata = DependencyResolver.GetInstance<IEntityMetadataResolver>().GetEntityMetadata(this);

            return
                metadata.Fields.Where(x => x.Mandatory)
                    .Select(fieldMetadata => properties.First(x => x.Name == fieldMetadata.Name))
                    .All(property => property.GetValue(this) != null);
        }

        /// <summary>
        /// Gets a query prepared with the deleted statement.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the prepared query.</returns>
        protected Query GetQuery<T>(Expression<Func<T, bool>> expression) where T : Entity
        {
            var query = Query.CreateQuery<T>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<T>(x => x.Deleted == false);

            return query;
        }
    }
}
