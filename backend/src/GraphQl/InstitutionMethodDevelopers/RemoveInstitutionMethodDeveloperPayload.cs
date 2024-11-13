using System.Collections.Generic;
using Metabase.Data;
using Metabase.GraphQl.Institutions;
using Metabase.GraphQl.Methods;

namespace Metabase.GraphQl.InstitutionMethodDevelopers;

public sealed class RemoveInstitutionMethodDeveloperPayload
{
    public RemoveInstitutionMethodDeveloperPayload(
        InstitutionMethodDeveloper institutionMethodDeveloper
    )
    {
        DevelopedMethodEdge = new InstitutionDevelopedMethodEdge(institutionMethodDeveloper);
        MethodDeveloperEdge = new InstitutionMethodDeveloperEdge(institutionMethodDeveloper);
    }

    public RemoveInstitutionMethodDeveloperPayload(
        IReadOnlyCollection<RemoveInstitutionMethodDeveloperError> errors
    )
    {
        Errors = errors;
    }

    public RemoveInstitutionMethodDeveloperPayload(
        RemoveInstitutionMethodDeveloperError error
    )
        : this([error])
    {
    }

    public InstitutionDevelopedMethodEdge? DevelopedMethodEdge { get; }
    public InstitutionMethodDeveloperEdge? MethodDeveloperEdge { get; }
    public IReadOnlyCollection<RemoveInstitutionMethodDeveloperError>? Errors { get; }
}