using HotChocolate.Data.Sorting;
using Metabase.Data;

namespace Metabase.GraphQl.Components;

public sealed class ComponentSortType
    : SortInputType<Component>
{
    protected override void Configure(
        ISortInputTypeDescriptor<Component> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Abbreviation);
        descriptor.Field(x => x.Description);
        // TODO Allow sorting by Availability. How? See https://chillicream.com/docs/hotchocolate/fetching-data/sorting/#customization
    }
}