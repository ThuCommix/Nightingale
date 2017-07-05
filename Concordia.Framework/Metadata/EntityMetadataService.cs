using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Concordia.Framework.Entities;

namespace Concordia.Framework.Metadata
{
    public class EntityMetadataService : IEntityMetadataService
    {
        /// <summary>
        /// Parses the EntityMetadata from xml.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Returns the EntityMetadata.</returns>
        public EntityMetadata FromXml(string filePath)
        {
            if(string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if(!new FileInfo(filePath).Exists)
                throw new FileNotFoundException();

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return FromStream(fileStream);
            }
        }

        /// <summary>
        /// Parses the EntityMetadata from xml async.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Returns the EntityMetadata.</returns>
        public async Task<EntityMetadata> FromXmlAsync(string filePath)
        {
            return await Task.Run(() => FromXml(filePath));
        }

        /// <summary>
        /// Parses the EntityMetadata from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns the EntityMetadata.</returns>
        public EntityMetadata FromStream(Stream stream)
        {
            if(stream == null)
                throw new ArgumentNullException();

            if(!stream.CanRead)
                throw new InvalidOperationException("The stream is not readable.");

            return (EntityMetadata)new XmlSerializer(typeof(EntityMetadata)).Deserialize(stream);
        }

        /// <summary>
        /// Parses the EntityMetadata from stream async.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns the EntityMetadata.</returns>
        public async Task<EntityMetadata> FromStreamAsync(Stream stream)
        {
            return await Task.Run(() => FromStream(stream));
        }

		/// <summary>
		/// Validates the xml.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="validationIssues">The validation issues.</param>
		/// <returns><c>true</c>, if xml was validated, <c>false</c> otherwise.</returns>
		public bool ValidateXml(Stream stream, List<string> validationIssues)
		{
			var settings = new XmlReaderSettings();
			settings.ValidationType = ValidationType.Schema;

			var myAssembly = Assembly.GetExecutingAssembly();
			using (var schemaStream = myAssembly.GetManifestResourceStream("Concordia.Framework.Entity.xsd"))
			{
				using (var schemaReader = XmlReader.Create(schemaStream))
				{
					settings.Schemas.Add(null, schemaReader);
				}
			}

			settings.ValidationEventHandler += (s, args) => 
			{ 
				if(args.Severity == XmlSeverityType.Error)
				{
					validationIssues.Add(args.Message);
				}
			};

			var entity = XmlReader.Create(stream, settings);

			while (entity.Read()) { }

			return validationIssues.Count == 0;
		}

		/// <summary>
		/// Validates the xml async.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="validationIssues">The validation issues.</param>
		/// <returns><c>true</c>, if xml was validated, <c>false</c> otherwise.</returns>
		public async Task<bool> ValidateXmlAsync(Stream stream, List<string> validationIssues)
		{
			return await Task.Run(() => ValidateXml(stream, validationIssues));
		}

		/// <summary>
		/// Saves the entity metadata to xml.
		/// </summary>
		/// <param name="filePath">The filepath.</param>
		/// <param name="entityMetadata">The entity metadata.</param>
		public void ToXml(string filePath, EntityMetadata entityMetadata)
		{
			if (filePath == null)
				throw new ArgumentNullException(nameof(filePath));

			if (entityMetadata == null)
				throw new ArgumentNullException(nameof(entityMetadata));

			if (!new FileInfo(filePath).Exists)
				throw new FileNotFoundException();

			using(var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			{
				ToXml(stream, entityMetadata);
			}
		}

		/// <summary>
		/// Saves the entity metadata to xml async.
		/// </summary>
		/// <param name="filePath">The filepath.</param>
		/// <param name="entityMetadata">The entity metadata.</param>
		public async Task ToXmlAsync(string filePath, EntityMetadata entityMetadata)
		{
			await Task.Run(() => ToXml(filePath, entityMetadata));
		}

		/// <summary>
		/// Saves the entity metadata to xml.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="entityMetadata">The entity metadata.</param>
		public void ToXml(Stream stream, EntityMetadata entityMetadata)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			if (entityMetadata == null)
				throw new ArgumentNullException(nameof(entityMetadata));

			if (!stream.CanWrite)
				throw new InvalidOperationException("The stream must be writeable.");

			new XmlSerializer(typeof(EntityMetadata)).Serialize(stream, entityMetadata);
		}

		/// <summary>
		/// Saves the entity metadata to xml async.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="entityMetadata">The entity metadata.</param>
		public async Task ToXmlAsync(Stream stream, EntityMetadata entityMetadata)
		{
			await Task.Run(() => ToXml(stream, entityMetadata));
		}

        /// <summary>
        /// Gets the entity metadata.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The entity metadata.</returns>
        public EntityMetadata GetEntityMetadata(Type entityType)
        {
            var entityMetadata = new EntityMetadata();

            entityMetadata.Name = entityType.Name;
			entityMetadata.Table = entityType.GetCustomAttribute<TableAttribute>().Table;
            entityMetadata.Namespace = entityType.Namespace;
            entityMetadata.Description = entityType.GetCustomAttribute<DescriptionAttribute>().Description;

            foreach (var property in ReflectionHelper.GetProperties(entityType))
            {
                if (property.GetCustomAttribute<MappedAttribute>() == null)
                {
                    if(property.GetCustomAttribute<VirtualPropertyAttribute>() == null)
                        continue;

                    if (typeof(IList).IsAssignableFrom(property.PropertyType))
                    {
                        var virtualListFieldMetadata = new VirtualListFieldMetadata();
                        virtualListFieldMetadata.Name = property.GetCustomAttribute<FieldTypeAttribute>().FieldType;
                        virtualListFieldMetadata.Description = property.GetCustomAttribute<DescriptionAttribute>().Description;
                        virtualListFieldMetadata.Expression = property.GetCustomAttribute<ExpressionAttribute>().Expression;
                        virtualListFieldMetadata.FieldType = property.PropertyType.Name;

                        entityMetadata.VirtualListFields.Add(virtualListFieldMetadata);
                    }
                    else
                    {
                        var virtualFieldMetadata = new VirtualFieldMetadata();
                        virtualFieldMetadata.Name = property.GetCustomAttribute<FieldTypeAttribute>().FieldType;
                        virtualFieldMetadata.Description = property.GetCustomAttribute<DescriptionAttribute>().Description;
                        virtualFieldMetadata.Expression = property.GetCustomAttribute<ExpressionAttribute>().Expression;
                        virtualFieldMetadata.FieldType = property.PropertyType.Name;

                        entityMetadata.VirtualFields.Add(virtualFieldMetadata);
                    }

                    continue;
                }

                var referenceFieldAttribute = property.GetCustomAttribute<ReferenceFieldAttribute>();

                if (referenceFieldAttribute == null)
                {
                    var fieldMetadata = new FieldMetadata();
                    fieldMetadata.Name = property.Name;
                    fieldMetadata.FieldType = property.GetCustomAttribute<FieldTypeAttribute>().FieldType;
                    fieldMetadata.Mandatory = property.GetCustomAttribute<MandatoryAttribute>() != null;
                    fieldMetadata.Unique = property.GetCustomAttribute<UniqueAttribute>() != null;
                    fieldMetadata.MaxLength = property.GetCustomAttribute<MaxLengthAttribute>()?.MaxLength ?? 0;
                    fieldMetadata.Cascade = property.GetCustomAttribute<CascadeAttribute>().Cascade;
                    fieldMetadata.Description = property.GetCustomAttribute<DescriptionAttribute>().Description;
                    fieldMetadata.ForeignKey = property.GetCustomAttribute<ForeignKeyAttribute>()?.Name;
					fieldMetadata.DateOnly = property.GetCustomAttribute<DateOnlyAttribute>() != null;
                    fieldMetadata.EagerLoad = property.GetCustomAttribute<EagerLoadAttribute>() != null;
                    fieldMetadata.Enum = property.GetCustomAttribute<EnumAttribute>() != null;

                    var decimalAttribute = property.GetCustomAttribute<DecimalAttribute>();
                    fieldMetadata.DecimalPrecision = decimalAttribute?.Precision ?? 0;
                    fieldMetadata.DecimalScale = decimalAttribute?.Scale ?? 0;

                    entityMetadata.Fields.Add(fieldMetadata);
                }
                else
                {
                    var listFieldMetadata = new ListFieldMetadata();
                    listFieldMetadata.Name = property.Name;
                    listFieldMetadata.FieldType = property.GetCustomAttribute<FieldTypeAttribute>().FieldType;
                    listFieldMetadata.ReferenceField = referenceFieldAttribute.ReferenceField;
                    listFieldMetadata.Cascade = property.GetCustomAttribute<CascadeAttribute>().Cascade;
                    listFieldMetadata.Description = property.GetCustomAttribute<DescriptionAttribute>().Description;
                    listFieldMetadata.EagerLoad = property.GetCustomAttribute<EagerLoadAttribute>() != null;

                    entityMetadata.ListFields.Add(listFieldMetadata);
                }
            }

            return entityMetadata;
        }

        /// <summary>
        /// Gets the entity metadata async.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The entity metadata.</returns>
        public async Task<EntityMetadata> GetEntityMetadataAsync(Type entityType)
        {
            return await Task.Run(() => GetEntityMetadata(entityType));
        }

        /// <summary>
        /// Gets the entity metadata.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The entity metadata.</returns>
        public EntityMetadata GetEntityMetadata(Entity entity)
		{
			return GetEntityMetadata(entity.GetType());
		}

		/// <summary>
		/// Gets the entity metadata async.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>The entity metadata.</returns>
		public async Task<EntityMetadata> GetEntityMetadataAsync(Entity entity)
		{
			return await Task.Run(() => GetEntityMetadata(entity));
		}

		/// <summary>
		/// Generates the entity.
		/// </summary>
		/// <param name="entityStream">The stream of the entity xml.</param>
		/// <param name="destinationStream">The destination stream.</param>
		public void GenerateEntity(Stream entityStream, Stream destinationStream)
		{
			if (entityStream == null)
				throw new ArgumentNullException(nameof(entityStream));

			if (destinationStream == null)
				throw new ArgumentNullException(nameof(destinationStream));

			if (!destinationStream.CanWrite)
				throw new InvalidOperationException("The destination stream must be writeable.");

			if (!entityStream.CanRead)
				throw new InvalidOperationException("The entity stream must be readable.");

			var argsList = new XsltArgumentList();

			var transform = new XslCompiledTransform(true);
			var xslReader = XmlReader.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream("Concordia.Framework.Entity.xsl"));
			var xmlReader = XmlReader.Create(entityStream);

			transform.Load(xslReader);
			transform.Transform(xmlReader, argsList, destinationStream);

			xmlReader.Dispose();
			xslReader.Dispose();
		}

		/// <summary>
		/// Generates the entity async.
		/// </summary>
		/// <param name="entityStream">The stream of the entity xml.</param>
		/// <param name="destinationStream">The destination stream.</param>
		public async Task GenerateEntityAsync(Stream entityStream, Stream destinationStream)
		{
			await Task.Run(() => GenerateEntity(entityStream, destinationStream));
		}

		/// <summary>
		/// Generates the entity.
		/// </summary>
		/// <param name="entityFilePath">The entity file path.</param>
		/// <param name="destinationFilePath">The destination file path.</param>
		public void GenerateEntity(string entityFilePath, string destinationFilePath)
		{
			if (entityFilePath == null)
				throw new ArgumentNullException(nameof(entityFilePath));

			if (destinationFilePath == null)
				throw new ArgumentNullException(nameof(destinationFilePath));

			if (!new FileInfo(entityFilePath).Exists)
				throw new FileNotFoundException("The file path was not found.", entityFilePath);

			using (var entityStream = new FileStream(entityFilePath, FileMode.Open, FileAccess.Read))
			{
				using (var destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write))
				{
					GenerateEntity(entityStream, destinationStream);
				}
			}
		}

		/// <summary>
		/// Generates the entity async.
		/// </summary>
		/// <param name="entityFilePath">The entity file path.</param>
		/// <param name="destinationFilePath">The destination file path.</param>
		public async Task GenerateEntityAsync(string entityFilePath, string destinationFilePath)
		{
			if (entityFilePath == null)
				throw new ArgumentNullException(nameof(entityFilePath));

			if (destinationFilePath == null)
				throw new ArgumentNullException(nameof(destinationFilePath));

			if (!new FileInfo(entityFilePath).Exists)
				throw new FileNotFoundException("The file path was not found.", entityFilePath);

			using(var entityStream = new FileStream(entityFilePath, FileMode.Open, FileAccess.Read))
			{
				using(var destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write))
				{
					await GenerateEntityAsync(entityStream, destinationStream);
				}
			}
		}
	}
}
