using System;
using ThuCommix.EntityFramework;
using ThuCommix.EntityFramework.Extensions;
using ThuCommix.EntityFramework.Sessions;
using ThuCommix.EntityFramework.SQLite;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataProvider = new SQLiteDataProvider("Data Source=persons.s3db;Version=3");
            var repository = new Repository(SessionFactory.OpenSession(dataProvider));

            // create tables if not available
            dataProvider.GetTable<Person>().Recreate();
            dataProvider.GetTable<Address>().Recreate();

            // register entity listeners
            repository.EntityListeners.Add(new PersonEntityService());
            repository.EntityListeners.Add(new AddressEntityService());

            // create entities

            var person = new Person();
            person.FirstName = "Max";
            person.Name = "Mustermann";
            person.Age = 21;

            var address = new Address();
            address.ValidFrom = DateTime.Today;
            address.Zip = "0815";
            address.Town = "SampleTown";
            address.Street = "Samplestreet 75a";
            address.Type = AddressType.Business;

            person.Addresses.Add(address);

            using (repository.BeginTransaction())
            {
                repository.Save(person);
                repository.Commit();
            }

            repository.Dispose();

            // creating a new session so that the entities aren't cached anymore
            dataProvider = new SQLiteDataProvider("Data Source=persons.s3db;Version=3");
            repository = new Repository(SessionFactory.OpenSession(dataProvider));

            // register entity listeners
            repository.EntityListeners.Add(new PersonEntityService());
            repository.EntityListeners.Add(new AddressEntityService());

            var loadedPerson = repository.GetById<Person>(1);

            Console.WriteLine($"Person: {loadedPerson.FullName}, IsLegalAge: {loadedPerson.IsLegalAge}");

            foreach (var addr in loadedPerson.ValidAddresses)
                Console.WriteLine($"{addr.Street}, Type={addr.Type}");

            using (repository.BeginTransaction())
            {
                try
                {
                    // try to remove the only address from person
                    var addr = loadedPerson.Addresses[0];
                    repository.Delete(addr);
                    repository.Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"The address could not be deleted. {e.Message}");
                }
            }

            Console.WriteLine("Press any key to continue ..");
            Console.ReadLine();
        }
    }
}
