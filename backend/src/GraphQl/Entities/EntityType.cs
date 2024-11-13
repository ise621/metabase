using System;
using GreenDonut;
using HotChocolate.Types;
using Metabase.Data;

namespace Metabase.GraphQl.Entities;

public abstract class EntityType<TEntity, TEntityByIdDataLoader>
    : ObjectType<TEntity>
    where TEntity : IEntity
    where TEntityByIdDataLoader : IDataLoader<Guid, TEntity?>
{
    protected override void Configure(
        IObjectTypeDescriptor<TEntity> descriptor
    )
    {
        descriptor
            .ImplementsNode()
            .IdField(t => t.Id)
            .ResolveNode((context, id) =>
                    context
                        .DataLoader<TEntityByIdDataLoader>()
                        .LoadAsync(id, context.RequestAborted)
                    ! // Notice the null-forgiving operator `!`. It's bad that we need to use it here.
            );
        descriptor
            .Field("uuid")
            .Type<NonNullType<UuidType>>()
            .Resolve(context =>
                context.Parent<TEntity>().Id
            );
        // TODO Do we want to expose this, require it as input, and use it to discover concurrent writes?
        descriptor
            .Field(t => t.Version)
            .Type<NonNullType<NonNegativeIntType>>()
            .Name("version")
            .Ignore();
    }
}