using Microsoft.EntityFrameworkCore;

namespace Metabase.Data;

[Owned]
public sealed class DefinitionOfSurfacesAndPrimeDirection
{
    // Constructor for EF Core because navigation properties cannot be set using a constructor.
    private DefinitionOfSurfacesAndPrimeDirection()
    {
    }

    public DefinitionOfSurfacesAndPrimeDirection(
        Reference reference
    )
    {
        Reference = reference;
    }

    public DefinitionOfSurfacesAndPrimeDirection(
        string description
    )
    {
        Description = description;
    }

    public DefinitionOfSurfacesAndPrimeDirection(
        Reference reference,
        string description
    )
    {
        Reference = reference;
        Description = description;
    }

    // Note that by construction either `Reference` or `Description` is non-null.
    // Reference, being an owned type, is included by default as told on https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities#querying-owned-types
    public Reference? Reference { get; private set; }
    public string? Description { get; private set; }

    // To evade the error
    // ---
    // Entity type `DefinitionOfSurfacesAndPrimeDirection` is an optional
    // dependent using table sharing and containing other dependents without any
    // required non shared property to identify whether the entity exists. If
    // all nullable properties contain a null value in database then an object
    // instance won't be created in the query causing nested dependent's values
    // to be lost. Add a required property to create instances with null values
    // for other properties or mark the incoming navigation as required to
    // always create an instance.
    // ---
    // I introduce this non-null property.
    public bool Exists { get; private set; } = true;
}