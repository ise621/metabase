using HotChocolate.Data.Sorting;
using Metabase.Data;

namespace Metabase.GraphQl.Methods;

public sealed class MethodSortType
    : SortInputType<Method>
{
    protected override void Configure(
        ISortInputTypeDescriptor<Method> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Description);
        descriptor.Field(x => x.CalculationLocator);
        descriptor.Field(x => x.Manager);
    }
}