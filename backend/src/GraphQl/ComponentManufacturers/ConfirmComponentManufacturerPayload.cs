using System.Collections.Generic;
using Metabase.Data;
using Metabase.GraphQl.Components;
using Metabase.GraphQl.Institutions;

namespace Metabase.GraphQl.ComponentManufacturers;

public sealed class ConfirmComponentManufacturerPayload
{
    public ConfirmComponentManufacturerPayload(
        ComponentManufacturer componentManufacturer
    )
    {
        ManufacturedComponentEdge = new InstitutionManufacturedComponentEdge(componentManufacturer);
        ComponentManufacturerEdge = new ComponentManufacturerEdge(componentManufacturer);
    }

    public ConfirmComponentManufacturerPayload(
        IReadOnlyCollection<ConfirmComponentManufacturerError> errors
    )
    {
        Errors = errors;
    }

    public ConfirmComponentManufacturerPayload(
        ConfirmComponentManufacturerError error
    )
        : this([error])
    {
    }

    public InstitutionManufacturedComponentEdge? ManufacturedComponentEdge { get; }
    public ComponentManufacturerEdge? ComponentManufacturerEdge { get; }
    public IReadOnlyCollection<ConfirmComponentManufacturerError>? Errors { get; }
}