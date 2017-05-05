# ThuCommix.EntityFramework

ThuCommix.EntityFramework is an object relational mapper (ORM) for .NET languages. It follows the model first pattern in which you declare your entity metadata via xml files. Through this metadata C# classes are generated which are annotated with attributes to fetch the right tables and columns during runtime.

### Supported databases

1. MSSQL
2. MySQL
3. PostgreSQL (currently not tested)
4. SQLite

### Getting started

1. Create a new project and add ThuCommix.Framework via nuget
2. Select a data provider (also available via nuget) and add it to your project
3. Create an entity via xml as described below

```
PM> Install-Package ThuCommix.EntityFramework
```
```
PM> Install-Package ThuCommix.EntityFramework.SQLite
```

(This also installs the dependencies of SQLite, you can delete all but "System.Data.SQLite")

[Metadata for Person.xml](https://gist.github.com/ThuCommix/fbd987fd81d7544ac8252008a243916c "Person.xml")
[Metadata for Address.xml](https://gist.github.com/ThuCommix/7dc00f0c5fc6e76536970c8db7c93a3c "Address.xml")

4. Create a data provider with the appropriate connection string and initialize a new repository class with a session created by the SessionFactory.

```csharp
    var dataProvider = new SQLiteDataProvider("Data Source=persons.s3db;Version=3");
    var repository = new Repository(SessionFactory.OpenSession(dataProvider));
```

5. Run the EntityGenerator.exe with a) InputFolder and b) OutputFolder as command line arguments, this will create the C# classes for you (It's located in the Tools folder of the repository and also in the release zip)

6. Create the table in the database (Recreate is an extension method and actually calls Delete and then Create)

```csharp
    dataProvider.GetTable<Person>().Recreate();
    dataProvider.GetTable<Address>().Recreate();
```

7. Fill your newly created entities with data

```csharp
    var person = new Person();
    person.FirstName = "Max";
    person.Name = "Mustermann";
    person.Age = 21;
    
    var address = new Address();
    address.ValidFrom = DateTime.Today;
    address.Zip = "0815";
    address.Town = "SampleTown";
    address.Street = "Samplestreet 75a";
    
    person.Addresses.Add(address);
```

8. Start a new transaction and save the person instance

```csharp
    using (repository.BeginTransaction())
    {
        repository.Save(person);
        repository.Commit();
    }
```

9. Load the person with the repository

```csharp
    var person = repository.GetById<Person>(1);
```

### Intercepting entity saving and deletion with EntityListener

When saving or deleting entities via the Repository class the entity listeners run to validate said actions. It's also handy to add some extra logic which should run for every entity of the specified type. In this example the IsLegalAge field is set based on the specified age of the person. If you return false the repository will throw an exception stating that the entity listener returned false.

```csharp
    public class PersonEntityService : EntityListener<Person>
    {
        protected override bool OnDelete(Person entity)
        {
            return true;
        }

        protected override bool OnSave(Person entity)
        {
            entity.IsLegalAge = entity.Age >= 21;

            return true;
        }
    }
```

### Session behaviour
The session, by default, only writes data to the database when a commit is happening, this behaviour can be changed when changing the FlushMode:
1. Always - Writes data directly when calling Save or Delete or ExecuteQuery
2. Intelligent - Writes data before calling ExecuteQuery to keep data in sync
3. Commit - Writes data when calling Commit
4. Manual - Writes data only when calling Flush on either the Repository or the Session itself

Also the delete behaviour is set to soft by default which means that the data is not actually deleted in the database but the Deleted field is set to true which causes that the entity is not fetched anymore. This can be changed when changing the DeletionMode on the session:
1. None - Never execute delete statements or setting the Deleted flag
2. Soft - Sets the Deleted flag but don't delete the entity in the database
3. Hard - Deletes the entity from the database

### StatefulSession vs StatelessSession
When using the SessionFactory, which is recommend, and no type arguments are given, the SessionFactory will return a StatefullSession which enables first level caching and entity persistence during the session. 
This sounds great, why should I use the StatelessSession you may ask. When inserting a huge amount of data the cache of the StatefulSession would increase rapidly and holding all those entity references in your RAM. A StatelessSession does not cache nor it cares about evicted entities, so this should be used when inserting many entities.