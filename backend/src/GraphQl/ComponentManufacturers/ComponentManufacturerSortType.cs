using HotChocolate.Data.Sorting;
using Metabase.Data;

namespace Metabase.GraphQl.ComponentManufacturers;

public sealed class ComponentManufacturerSortType
    : SortInputType<ComponentManufacturer>
{
    protected override void Configure(
        ISortInputTypeDescriptor<ComponentManufacturer> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Component);
        descriptor.Field(x => x.Institution);
        descriptor.Field(x => x.Pending);
    }
}