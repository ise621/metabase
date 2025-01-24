using System.Collections.Generic;
using Metabase.Data;
using Metabase.GraphQl.Institutions;
using Metabase.GraphQl.Methods;

namespace Metabase.GraphQl.InstitutionMethodDevelopers;

public sealed class ConfirmInstitutionMethodDeveloperPayload
{
    public ConfirmInstitutionMethodDeveloperPayload(
        InstitutionMethodDeveloper institutionMethodDeveloper
    )
    {
        DevelopedMethodEdge = new InstitutionDevelopedMethodEdge(institutionMethodDeveloper);
        MethodDeveloperEdge = new InstitutionMethodDeveloperEdge(institutionMethodDeveloper);
    }

    public ConfirmInstitutionMethodDeveloperPayload(
        IReadOnlyCollection<ConfirmInstitutionMethodDeveloperError> errors
    )
    {
        Errors = errors;
    }

    public ConfirmInstitutionMethodDeveloperPayload(
        ConfirmInstitutionMethodDeveloperError error
    )
        : this([error])
    {
    }

    public InstitutionDevelopedMethodEdge? DevelopedMethodEdge { get; }
    public InstitutionMethodDeveloperEdge? MethodDeveloperEdge { get; }
    public IReadOnlyCollection<ConfirmInstitutionMethodDeveloperError>? Errors { get; }
}