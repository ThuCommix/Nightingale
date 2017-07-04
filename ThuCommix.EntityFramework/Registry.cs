using ThuCommix.EntityFramework.Metadata;
using ThuCommix.EntityFramework.Queries.Tokens;

namespace ThuCommix.EntityFramework
{
    internal static class Registry
    {
        internal static void Initialize()
        {
            DependencyResolver.Register<IEntityMetadataService>(new EntityMetadataService());
            DependencyResolver.Register<IEntityMetadataResolver>(new EntityMetadataResolver());
            DependencyResolver.Register<IEntityService>(new EntityService());
            DependencyResolver.Register<ISqlTokenComposerService>(new SqlTokenComposerService());
        }
    }
}
