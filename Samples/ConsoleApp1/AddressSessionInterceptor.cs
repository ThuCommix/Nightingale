using Nightingale.Sessions;

namespace ConsoleApp1
{
    public class AddressSessionInterceptor : SessionInterceptor<Address>
    {
        protected override bool Delete(Address entity)
        {
            if (entity.Person.Addresses.Count <= 1)
                return false;

            return true;
        }

        protected override bool Save(Address entity)
        {
            entity.IsValid = entity.ValidTo == null;

            return true;
        }
    }
}
