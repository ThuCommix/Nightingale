# Nightingale [![Build Status](https://travis-ci.org/ThuCommix/Nightingale.svg?branch=master)](https://travis-ci.org/ThuCommix/Nightingale)

Nightingale is an object relational mapper (ORM) for .NET languages. It follows the model first pattern in which you declare your entity metadata via xml files. Through this metadata C# classes are generated which are annotated with attributes to fetch the right tables and columns during runtime.

### Supported databases

1. MSSQL
2. MySQL
3. PostgreSQL (currently not tested)
4. SQLite

### Getting started

1. Create a new project and add Nightingale via nuget
2. Select a data provider (also available via nuget) and add it to your project
3. Create an entity via xml as described below

```
PM> Install-Package Nightingale
```
```
PM> Install-Package Nightingale.SQLite
```

[Metadata for Person.xml](https://gist.github.com/ThuCommix/fbd987fd81d7544ac8252008a243916c "Person.xml")
[Metadata for Address.xml](https://gist.github.com/ThuCommix/7dc00f0c5fc6e76536970c8db7c93a3c "Address.xml")

4. Create a connection factory with the appropriate connection string and initialize a new session created by the SessionFactory.

```csharp
    var connectionFactory = new SQLiteConnectionFactory();
    connectionFactory.DataSource = "persons.s3db";

    var sessionFactory = new SessionFactory(connectionFactory);

    var session = sessionFactory.OpenSession();
```

5. Run the ConcordiaFrameworkCli with a) InputFolder and b) OutputFolder as command line arguments, this will create the C# classes for you (It's located in the Tools folder of the repository and also in the release zip)

6. Create the table in the database (Recreate is an extension method and actually calls Delete and then Create)

```csharp
    using (var connection = connectionFactory.Open())
    {
        connection.GetTable<Person>().Recreate();
        connection.GetTable<Address>().Recreate();
    }
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
    using (var transaction = session.BeginTransaction())
    {
        session.Save(person);
        session.SaveChanges();
        transaction.Commit();
    }
```

Also nested transactions are possible, just call BeginTransaction from inside the using block. This
nested transaction will basicly create a save point which can be rolled back to when calling
Rollback or Dispose without calling commit. A nested transaction must be committed before disposing
otherwise the transaction will be rolled back.

9. Load the person with the session

```csharp
    var person = session.Get<Person>(1);
```

### Intercepting entity saving and deletion with SessionInterceptor

When saving or deleting entities via the Session class the session interceptors run to validate said actions. It's also handy to add some extra logic which should run for every entity of the specified type. In this example the IsLegalAge field is set based on the specified age of the person. If you return false the session will throw an exception stating that the session interceptor returned false.

```csharp
    public class PersonSessionInterceptor : SessionInterceptor<Person>
    {
        protected override bool Delete(Person entity)
        {
            return true;
        }

        protected override bool Save(Person entity)
        {
            entity.IsLegalAge = entity.Age >= 21;

            return true;
        }
    }
```

### Querying data
Getting an entity by it's id. is a usefull key feature but in the most cases we don't have any idea of the id and instead rely on querying the database.

```csharp
    // using the IQueryable interface
    var customers3 = repository.Query<Person>().Where(x => x.FirstName == "Peter" && x.Age >= 18).ToList();
    var customers4 = repository.Query<Person>().Where(x => x.Orders.Any(y => y.Status == OrderStatus.Pending)).OrderBy(x => x.Name).ThenBy(x => x.FirstName).ToList()
```