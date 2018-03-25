using System;
using System.Linq;
using Nightingale;
using Nightingale.Logging;
using Nightingale.Sessions;
using Nightingale.SQLite;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionFactory = new SQLiteConnectionFactory { DataSource = "persons.s3db" };
            var sessionFactory = new SessionFactory(connectionFactory) { Logger = new TraceLogger(LogLevel.Debug) };

            // register entity listeners
            sessionFactory.Interceptors.Add(new PersonSessionInterceptor());
            sessionFactory.Interceptors.Add(new AddressSessionInterceptor());

            var session = sessionFactory.OpenSession();

            // create tables if not available
            using (var connection = connectionFactory.CreateConnection())
            {
                connection.CreateTable<Person>(true);
                connection.CreateTable<Address>(true);
            }

            // create entities
            var person = new Person
            {
                FirstName = "Max",
                Name = "Mustermann",
                Age = 21
            };

            var address = new Address
            {
                ValidFrom = DateTime.Today,
                Zip = "0815",
                Town = "SampleTown",
                Street = "Samplestreet 75a",
                Type = AddressType.Business
            };

            person.Addresses.Add(address);

            using (var transaction = session.BeginTransaction())
            {
                session.Save(person);
                session.SaveChanges();

                transaction.Commit();
            }

            session.Dispose();

            // creating a new session so that the entities aren't cached anymore
            session = sessionFactory.OpenSession();

            // Possible to discard changes which are made in this session.
            var loadedPerson = session.Get<Person>(1);
            loadedPerson.Name = "Bernd";
            loadedPerson.Age = 0;
            loadedPerson.Addresses.RemoveAt(0);
            loadedPerson.Addresses.Add(new Address());

            session.DiscardChanges();

            Console.WriteLine($"Person: {loadedPerson.FullName}, IsLegalAge: {loadedPerson.IsLegalAge}");

            foreach (var addr in loadedPerson.ValidAddresses)
                Console.WriteLine($"{addr.Street}, Type={addr.Type}");

            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    // try to remove the only address from person
                    var addr = loadedPerson.Addresses[0];
                    session.Delete(addr);
                    session.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"The address could not be deleted. {e.Message}");
                }
            }

            session.Dispose();

            using (session = sessionFactory.OpenSession())
            {
                var customer = session.Create<Person>();
                customer.FirstName = "Bernd";
                customer.Name = "Oklo";

                // save not required because session.Create is used for entity creation
                session.SaveChanges();

                var fetchedCustomer = session.Query<Person>().FirstOrDefault(x => x.Age == null);
                fetchedCustomer.Age = 17;

                session.Save(fetchedCustomer);
                session.SaveChanges();

            }

            // custom select
            using (session = sessionFactory.OpenSession())
            {
                var customerInfos = session.Query<Person>().Where(x => x.Age >= 18).Select(x => new { Name = x.Name, CustomerId = x.Id }).ToList();
            }

            Console.WriteLine("Press any key to continue ..");
            Console.ReadLine();
        }
    }
}
