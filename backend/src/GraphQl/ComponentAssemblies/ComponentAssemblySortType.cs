using HotChocolate.Data.Sorting;
using Metabase.Data;

namespace Metabase.GraphQl.ComponentAssemblies;

public sealed class ComponentAssemblySortType
    : SortInputType<ComponentAssembly>
{
    protected override void Configure(
        ISortInputTypeDescriptor<ComponentAssembly> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.AssembledComponent);
        descriptor.Field(x => x.PartComponent);
        descriptor.Field(x => x.Index);
        descriptor.Field(x => x.PrimeSurface);
    }
}