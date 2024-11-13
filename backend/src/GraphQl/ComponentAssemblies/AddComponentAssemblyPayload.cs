using System.Collections.Generic;
using Metabase.Data;
using Metabase.GraphQl.Components;

namespace Metabase.GraphQl.ComponentAssemblies;

public sealed class AddComponentAssemblyPayload
{
    public AddComponentAssemblyPayload(
        ComponentAssembly componentAssembly
    )
    {
        AssembledOfEdge = new ComponentAssembledOfEdge(componentAssembly);
        PartOfEdge = new ComponentPartOfEdge(componentAssembly);
    }

    public AddComponentAssemblyPayload(
        IReadOnlyCollection<AddComponentAssemblyError> errors
    )
    {
        Errors = errors;
    }

    public AddComponentAssemblyPayload(
        AddComponentAssemblyError error
    )
        : this([error])
    {
    }

    public ComponentAssembledOfEdge? AssembledOfEdge { get; }
    public ComponentPartOfEdge? PartOfEdge { get; }
    public IReadOnlyCollection<AddComponentAssemblyError>? Errors { get; }
}