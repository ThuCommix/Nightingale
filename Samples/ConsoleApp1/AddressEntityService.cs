using Concordia.Framework;

namespace ConsoleApp1
{
    public class AddressEntityService : EntityListener<Address>
    {
        protected override bool OnDelete(Address entity)
        {
            if (entity.Person.Addresses.Count <= 1)
                return false;

            return true;
        }

        protected override bool OnSave(Address entity)
        {
            entity.IsValid = entity.ValidTo == null;

            return true;
        }
    }
}
