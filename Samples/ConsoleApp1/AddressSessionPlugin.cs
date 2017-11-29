using Nightingale.Sessions;

namespace ConsoleApp1
{
    public class AddressSessionPlugin : SessionPluginBase<Address>
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
