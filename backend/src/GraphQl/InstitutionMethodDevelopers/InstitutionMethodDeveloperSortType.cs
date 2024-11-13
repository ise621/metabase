using HotChocolate.Data.Sorting;
using Metabase.Data;

namespace Metabase.GraphQl.InstitutionMethodDevelopers;

public sealed class InstitutionMethodDeveloperSortType
    : SortInputType<InstitutionMethodDeveloper>
{
    protected override void Configure(
        ISortInputTypeDescriptor<InstitutionMethodDeveloper> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Method);
        descriptor.Field(x => x.Institution);
        descriptor.Field(x => x.Pending);
    }
}