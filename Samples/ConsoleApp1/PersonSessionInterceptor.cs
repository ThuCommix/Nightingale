using Nightingale.Sessions;

namespace ConsoleApp1
{
    public class PersonSessionInterceptor : SessionInterceptor<Person>
    {
        protected override bool Save(Person entity)
        {
            entity.IsLegalAge = entity.Age >= 21;

            return true;
        }
    }
}
