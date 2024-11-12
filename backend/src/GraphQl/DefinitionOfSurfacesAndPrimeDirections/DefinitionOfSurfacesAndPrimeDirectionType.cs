using HotChocolate.Types;
using Metabase.Data;
using Metabase.GraphQl.References;

namespace Metabase.GraphQl.DefinitionOfSurfacesAndPrimeDirections;

public sealed class DefinitionOfSurfacesAndPrimeDirectionType
    : ObjectType<DefinitionOfSurfacesAndPrimeDirection>
{
    protected override void Configure(
        IObjectTypeDescriptor<DefinitionOfSurfacesAndPrimeDirection> descriptor
    )
    {
        base.Configure(descriptor);
        descriptor
            .Field(t => t.Exists)
            .Ignore();
        descriptor
            .Field(t => t.Reference)
            .Type<ReferenceType>()
            .Resolve(context => context
                .Parent<DefinitionOfSurfacesAndPrimeDirection>()
                .Reference?
                .TheReference
            );
    }
}