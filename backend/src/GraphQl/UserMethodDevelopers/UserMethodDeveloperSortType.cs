using HotChocolate.Data.Sorting;
using Metabase.Data;

namespace Metabase.GraphQl.UserMethodDevelopers;

public sealed class UserMethodDeveloperSortType
    : SortInputType<UserMethodDeveloper>
{
    protected override void Configure(
        ISortInputTypeDescriptor<UserMethodDeveloper> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Method);
        descriptor.Field(x => x.User);
        descriptor.Field(x => x.Pending);
    }
}