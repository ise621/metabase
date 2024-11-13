using System.Collections.Generic;
using Metabase.Data;
using Metabase.GraphQl.Components;
using Metabase.GraphQl.Institutions;

namespace Metabase.GraphQl.ComponentManufacturers;

public sealed class AddComponentManufacturerPayload
{
    public AddComponentManufacturerPayload(
        ComponentManufacturer componentManufacturer
    )
    {
        ManufacturedComponentEdge = new InstitutionManufacturedComponentEdge(componentManufacturer);
        ComponentManufacturerEdge = new ComponentManufacturerEdge(componentManufacturer);
    }

    public AddComponentManufacturerPayload(
        IReadOnlyCollection<AddComponentManufacturerError> errors
    )
    {
        Errors = errors;
    }

    public AddComponentManufacturerPayload(
        AddComponentManufacturerError error
    )
        : this([error])
    {
    }

    public InstitutionManufacturedComponentEdge? ManufacturedComponentEdge { get; }
    public ComponentManufacturerEdge? ComponentManufacturerEdge { get; }
    public IReadOnlyCollection<AddComponentManufacturerError>? Errors { get; }
}