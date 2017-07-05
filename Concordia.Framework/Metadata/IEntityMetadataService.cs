using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Concordia.Framework.Entities;

namespace Concordia.Framework.Metadata
{
    public interface IEntityMetadataService
    {
        /// <summary>
        /// Parses the EntityMetadata from xml.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Returns the EntityMetadata.</returns>
        EntityMetadata FromXml(string filePath);

        /// <summary>
        /// Parses the EntityMetadata from xml async.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Returns the EntityMetadata.</returns>
        Task<EntityMetadata> FromXmlAsync(string filePath);

        /// <summary>
        /// Parses the EntityMetadata from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns the EntityMetadata.</returns>
        EntityMetadata FromStream(Stream stream);

        /// <summary>
        /// Parses the EntityMetadata from stream async.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns the EntityMetadata.</returns>
        Task<EntityMetadata> FromStreamAsync(Stream stream);

		/// <summary>
		/// Saves the entity metadata to xml.
		/// </summary>
		/// <param name="filePath">The filepath.</param>
		/// <param name="entityMetadata">The entity metadata.</param>
		void ToXml(string filePath, EntityMetadata entityMetadata);

		/// <summary>
		/// Saves the entity metadata to xml async.
		/// </summary>
		/// <param name="filePath">The filepath.</param>
		/// <param name="entityMetadata">The entity metadata.</param>
		Task ToXmlAsync(string filePath, EntityMetadata entityMetadata);

		/// <summary>
		/// Saves the entity metadata to xml.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="entityMetadata">The entity metadata.</param>
		void ToXml(Stream stream, EntityMetadata entityMetadata);

		/// <summary>
		/// Saves the entity metadata to xml async.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="entityMetadata">The entity metadata.</param>
		Task ToXmlAsync(Stream stream, EntityMetadata entityMetadata);

		/// <summary>
		/// Validates the xml.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="validationIssues">The validation issues.</param>
		/// <returns><c>true</c>, if xml was validated, <c>false</c> otherwise.</returns>
		bool ValidateXml(Stream stream, List<string> validationIssues);

		/// <summary>
		/// Validates the xml async.
		/// </summary>
		/// <param name="stream">Stream.</param>
		/// <param name="validationIssues">The validation issues.</param>
		/// <returns><c>true</c>, if xml was validated, <c>false</c> otherwise.</returns>
		Task<bool> ValidateXmlAsync(Stream stream, List<string> validationIssues);

        /// <summary>
        /// Gets the entity metadata.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The entity metadata.</returns>
        EntityMetadata GetEntityMetadata(Type entityType);

        /// <summary>
        /// Gets the entity metadata async.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The entity metadata.</returns>
        Task<EntityMetadata> GetEntityMetadataAsync(Type entityType);

        /// <summary>
        /// Gets the entity metadata.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The entity metadata.</returns>
        EntityMetadata GetEntityMetadata(Entity entity);

		/// <summary>
		/// Gets the entity metadata async.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>The entity metadata.</returns>
		Task<EntityMetadata> GetEntityMetadataAsync(Entity entity);

		/// <summary>
		/// Generates the entity.
		/// </summary>
		/// <param name="entityStream">The stream of the entity xml.</param>
		/// <param name="destinationStream">The destination stream.</param>
		void GenerateEntity(Stream entityStream, Stream destinationStream);

		/// <summary>
		/// Generates the entity async.
		/// </summary>
		/// <param name="entityStream">The stream of the entity xml.</param>
		/// <param name="destinationStream">The destination stream.</param>
		Task GenerateEntityAsync(Stream entityStream, Stream destinationStream);

		/// <summary>
		/// Generates the entity.
		/// </summary>
		/// <param name="entityFilePath">The entity file path.</param>
		/// <param name="destinationFilePath">The destination file path.</param>
		void GenerateEntity(string entityFilePath, string destinationFilePath);

		/// <summary>
		/// Generates the entity async.
		/// </summary>
		/// <param name="entityFilePath">The entity file path.</param>
		/// <param name="destinationFilePath">The destination file path.</param>
		Task GenerateEntityAsync(string entityFilePath, string destinationFilePath);
    }
}
