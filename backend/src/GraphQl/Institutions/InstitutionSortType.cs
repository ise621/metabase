using HotChocolate.Data.Sorting;
using Metabase.Data;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionSortType
    : SortInputType<Institution>
{
    protected override void Configure(
        ISortInputTypeDescriptor<Institution> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Abbreviation);
        descriptor.Field(x => x.Description);
        descriptor.Field(x => x.WebsiteLocator);
        descriptor.Field(x => x.State);
        descriptor.Field(x => x.Manager);
    }
}