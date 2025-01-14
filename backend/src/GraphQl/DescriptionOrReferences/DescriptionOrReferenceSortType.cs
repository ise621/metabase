using HotChocolate.Data.Sorting;
using Metabase.Data;

namespace Metabase.GraphQl.DescriptionOrReferences;

public sealed class DescriptionOrReferenceSortType
    : SortInputType<DescriptionOrReference>
{
    protected override void Configure(
        ISortInputTypeDescriptor<DescriptionOrReference> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Description);
    }
}