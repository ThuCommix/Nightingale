namespace Nightingale.Sessions
{
    public enum PersistenceBehavior
    {
        /// <summary>
        /// Entities are not added to the persistence context.
        /// </summary>
        None,

        /// <summary>
        /// New entities are added to the persistence context, but are not overriden
        /// which means a query will discard new data in favor to the data in the persistence context.
        /// </summary>
        AddNewOnly,

        /// <summary>
        /// New entities are added to the persistence context. 
        /// A query will update the entity and discards it's previous changes.
        /// </summary>
        OverrideChanges
    }
}
