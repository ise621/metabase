using HotChocolate.Data.Filters;
using Metabase.Data;

namespace Metabase.GraphQl.Methods;

public sealed class MethodFilterType
    : FilterInputType<Method>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<Method> descriptor
    )
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(x => x.Id);
        descriptor.Field(x => x.Name);
        descriptor.Field(x => x.Description);
        descriptor.Field(x => x.CalculationLocator);
        descriptor.Field(x => x.Categories);
        descriptor.Field(x => x.InstitutionDevelopers);
        descriptor.Field(x => x.InstitutionDeveloperEdges);
        descriptor.Field(x => x.UserDevelopers);
        descriptor.Field(x => x.UserDeveloperEdges);
        descriptor.Field(x => x.Manager);
    }
}