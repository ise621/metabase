using Metabase.Data;
using Metabase.GraphQl.Components;

namespace Metabase.GraphQl.Institutions;

public sealed class InstitutionManufacturedComponentEdge(
    ComponentManufacturer association
    )
        : Edge<Component, ComponentByIdDataLoader>(association.ComponentId)
{
}