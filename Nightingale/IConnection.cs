using System;
using System.Data;
using Nightingale.Entities;
using Nightingale.Queries;

namespace Nightingale
{
    /// <summary>
    /// Represents a database connection.
    /// </summary>
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// A value indicating whether the connection is open.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Opens the database connection.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the database connection.
        /// </summary>
        void Close();

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>Returns a db transaction instance.</returns>
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Rollbacks the current transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Rollsbacks to the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        void RollbackTo(string savePoint);

        /// <summary>
        /// Releases the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        void Release(string savePoint);

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Creates a save point with the specified name.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        void Save(string savePoint);

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Returns the count of the affected rows.</returns>
        int ExecuteNonQuery(IQuery query);

        /// <summary>
        /// Executes the scalar query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Returns the result as object.</returns>
        object ExecuteScalar(IQuery query);

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Returns an instance of the IDataReader.</returns>
        IDataReader ExecuteReader(IQuery query);

        /// <summary>
        /// Executes an insert and returns the id.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Returns the id.</returns>
        int ExecuteInsert(IQuery query);

        /// <summary>
        /// Gets the table object.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>Returns the table object.</returns>
        Table<T> GetTable<T>() where T : Entity;
    }
}
