using ThuCommix.EntityFramework;

namespace ConsoleApp1
{
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
}
