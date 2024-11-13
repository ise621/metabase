using HotChocolate.Data.Sorting;
using Metabase.Data;

namespace Metabase.GraphQl.DefinitionOfSurfacesAndPrimeDirections;

public sealed class DefinitionOfSurfacesAndPrimeDirectionSortType
    : SortInputType<Method>
{
    protected override void Configure(
        ISortInputTypeDescriptor<Method> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.Description);
    }
}