using HotChocolate.Data.Sorting;
using Metabase.Data;

namespace Metabase.GraphQl.InstitutionRepresentatives;

public sealed class InstitutionRepresentativeSortType
    : SortInputType<InstitutionRepresentative>
{
    protected override void Configure(
        ISortInputTypeDescriptor<InstitutionRepresentative> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Institution);
        descriptor.Field(x => x.User);
        descriptor.Field(x => x.Role);
        descriptor.Field(x => x.Pending);
    }
}