using ThuCommix.EntityFramework.Metadata;

namespace ThuCommix.EntityFramework
{
    internal static class Registry
    {
        static Registry()
        {
            DependencyResolver.Register<IEntityMetadataService>(new EntityMetadataService());
            DependencyResolver.Register<IEntityMetadataResolver>(new EntityMetadataResolver());
            DependencyResolver.Register<IEntityService>(new EntityService());
        }
    }
}
