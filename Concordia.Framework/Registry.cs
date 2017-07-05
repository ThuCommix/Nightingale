using Concordia.Framework.Metadata;
using Concordia.Framework.Queries.Tokens;

namespace Concordia.Framework
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
