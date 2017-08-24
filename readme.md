# Concordia.Framework [![Build Status](https://travis-ci.org/ThuCommix/Concordia.Framework.svg?branch=master)](https://travis-ci.org/ThuCommix/Concordia.Framework)

Concordia.Framework is an object relational mapper (ORM) for .NET languages. It follows the model first pattern in which you declare your entity metadata via xml files. Through this metadata C# classes are generated which are annotated with attributes to fetch the right tables and columns during runtime.

### Supported databases

1. MSSQL
2. MySQL
3. PostgreSQL (currently not tested)
4. SQLite

### Getting started

1. Create a new project and add Concordia.Framework via nuget
2. Select a data provider (also available via nuget) and add it to your project
3. Create an entity via xml as described below

```
PM> Install-Package Concordia.Framework
```
```
PM> Install-Package Concordia.Framework.SQLite
```

[Metadata for Person.xml](https://gist.github.com/ThuCommix/fbd987fd81d7544ac8252008a243916c "Person.xml")
[Metadata for Address.xml](https://gist.github.com/ThuCommix/7dc00f0c5fc6e76536970c8db7c93a3c "Address.xml")

4. Create a data provider with the appropriate connection string and initialize a new repository class with a session created by the SessionFactory.

```csharp
    var connectionFactory = new SQLiteConnectionFactory();
    connectionFactory.DataSource = "persons.s3db";

    var sessionFactory = new SessionFactory(connectionFactory);

    var session = sessionFactory.GetCurrentSession();
    var repository = new Repository(session);
```

5. Run the ConcordiaFrameworkCli with a) InputFolder and b) OutputFolder as command line arguments, this will create the C# classes for you (It's located in the Tools folder of the repository and also in the release zip)

6. Create the table in the database (Recreate is an extension method and actually calls Delete and then Create)

```csharp
    session.GetTable<Person>().Recreate();
    session.GetTable<Address>().Recreate();
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
    public class PersonEntityListener : EntityListener<Person>
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

### Querying data
Getting an entity by it's id. is a usefull key feature but in the most cases we don't have any idea of the id and instead rely on querying the database.
There are two different ways of querying data, the first one is just using the IRepository.GetList<T>() where you can define a kind of predicate to query or
using the Query class itself.

```csharp
    // using IRepository.GetList<T>()
    var customers = repository.GetList<Person>(x => x.FirstName == "Peter" && x.Age >= 18);
    var customers2 = repository.GetList<Person>(x => x.Name.StartsWith("Mueller"));

    // using the Query class
    var query = Query.CreateQuery<Person>();
    var group = query.CreateQueryConditionGroup()

    group.CreateQueryCondition<Person>(x => x.FirstName == "Peter" && x.Age >= 18);
    group.CreateQueryCondition<Person>(x => x.Name.StartsWith("Mueller"));

    var customers3 = repository.ExecuteQuery(query);

    var query2 = Query.CreateQuery<Person>();
    var group1 = query2.CreateQueryConditionGroup();
    var group2 = query2.CreateQueryConditionGroup(QueryJunction.Or);

    // can also be merge into one condition using the c# OR operator
    group1.CreateQueryCondition<Person>(x => x.FirstName == "Peter");
    group2.CreateQueryCondition<Person>(x => x.FirstName == "Klaus");

    var customers4 = repository.ExecuteQuery(query);
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

### StatelessSession
When using the default session type and insert or read huge amount of entities from the database, all of them are cached in the session cache. This will of course eventually
hit a memory limit when not creating new sessions in a long processing chain. For this case a stateless session can be used.
A stateless session can be configured when overriding the ISessionContext and does not have any caching mechanism build in. It also flushes directly aftere calling SaveOrUpdate
to prevent entity reference caching. With this session type you can read millions of entities without blowing up the session.