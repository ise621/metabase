using HotChocolate.Data.Filters;
using Metabase.Data;

namespace Metabase.GraphQl.DefinitionOfSurfacesAndPrimeDirections;

public sealed class DefinitionOfSurfacesAndPrimeDirectionFilterType
    : FilterInputType<Method>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<Method> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.Description);
    }
}