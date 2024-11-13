using System.Collections.Generic;
using Metabase.Data;
using Metabase.GraphQl.Components;

namespace Metabase.GraphQl.ComponentVariants;

public sealed class AddComponentVariantPayload
{
    public AddComponentVariantPayload(
        ComponentVariant componentVariant,
        ComponentVariant reverseComponentVariant
    )
    {
        VariantOfEdge = new ComponentVariantOfEdge(componentVariant);
        ReverseVariantOfEdge = new ComponentVariantOfEdge(reverseComponentVariant);
    }

    public AddComponentVariantPayload(
        IReadOnlyCollection<AddComponentVariantError> errors
    )
    {
        Errors = errors;
    }

    public AddComponentVariantPayload(
        AddComponentVariantError error
    )
        : this([error])
    {
    }

    public ComponentVariantOfEdge? VariantOfEdge { get; }
    public ComponentVariantOfEdge? ReverseVariantOfEdge { get; }
    public IReadOnlyCollection<AddComponentVariantError>? Errors { get; }
}