using HotChocolate.Data.Filters;
using Metabase.Data;

namespace Metabase.GraphQl.DescriptionOrReferences;

public sealed class DescriptionOrReferenceFilterType
    : FilterInputType<DescriptionOrReference>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<DescriptionOrReference> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Description);
    }
}