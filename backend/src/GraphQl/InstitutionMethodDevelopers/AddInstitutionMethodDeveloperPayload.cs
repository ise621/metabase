using System.Collections.Generic;
using Metabase.Data;
using Metabase.GraphQl.Institutions;
using Metabase.GraphQl.Methods;

namespace Metabase.GraphQl.InstitutionMethodDevelopers;

public sealed class AddInstitutionMethodDeveloperPayload
{
    public AddInstitutionMethodDeveloperPayload(
        InstitutionMethodDeveloper institutionMethodDeveloper
    )
    {
        DevelopedMethodEdge = new InstitutionDevelopedMethodEdge(institutionMethodDeveloper);
        MethodDeveloperEdge = new InstitutionMethodDeveloperEdge(institutionMethodDeveloper);
    }

    public AddInstitutionMethodDeveloperPayload(
        IReadOnlyCollection<AddInstitutionMethodDeveloperError> errors
    )
    {
        Errors = errors;
    }

    public AddInstitutionMethodDeveloperPayload(
        AddInstitutionMethodDeveloperError error
    )
        : this([error])
    {
    }

    public InstitutionDevelopedMethodEdge? DevelopedMethodEdge { get; }
    public InstitutionMethodDeveloperEdge? MethodDeveloperEdge { get; }
    public IReadOnlyCollection<AddInstitutionMethodDeveloperError>? Errors { get; }
}