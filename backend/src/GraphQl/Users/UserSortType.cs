using HotChocolate.Data.Sorting;
using Metabase.Data;

namespace Metabase.GraphQl.Users;

public sealed class UserSortType
    : SortInputType<User>
{
    protected override void Configure(
        ISortInputTypeDescriptor<User> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id);
        // TODO The commented fiels below should be sortable by OpenId Connect Clients and application users with the proper scopes and rights. If they are filterable in general, it is a way to figure out that information even if it is not returned by GraphQL.
        // descriptor.Field(x => x.Name);
        // descriptor.Field(x => x.Email);
        // descriptor.Field(x => x.PostalAddress);
        // descriptor.Field(x => x.WebsiteLocator);
    }
}