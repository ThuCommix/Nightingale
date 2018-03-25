namespace Nightingale.Tests.DataSources
{
    public class CustomerModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public CustomerModel()
        {
            
        }

        public CustomerModel(int id)
        {
            Id = id;
        }
    }
}
