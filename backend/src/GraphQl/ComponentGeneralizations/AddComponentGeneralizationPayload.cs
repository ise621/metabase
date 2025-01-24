using System.Collections.Generic;
using Metabase.Data;
using Metabase.GraphQl.Components;

namespace Metabase.GraphQl.ComponentGeneralizations;

public sealed class AddComponentGeneralizationPayload
{
    public AddComponentGeneralizationPayload(
        ComponentConcretizationAndGeneralization association
    )
    {
        GeneralizationOfEdge = new ComponentGeneralizationOfEdge(association);
        ConcretizationOfEdge = new ComponentConcretizationOfEdge(association);
    }

    public AddComponentGeneralizationPayload(
        IReadOnlyCollection<AddComponentGeneralizationError> errors
    )
    {
        Errors = errors;
    }

    public AddComponentGeneralizationPayload(
        AddComponentGeneralizationError error
    )
        : this([error])
    {
    }

    public ComponentGeneralizationOfEdge? GeneralizationOfEdge { get; }
    public ComponentConcretizationOfEdge? ConcretizationOfEdge { get; }
    public IReadOnlyCollection<AddComponentGeneralizationError>? Errors { get; }
}