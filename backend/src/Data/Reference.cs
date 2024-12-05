using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Metabase.Data;

[Owned]
public sealed class Reference
{
    // Constructor for EF Core because navigation properties cannot be set using a constructor.
    private Reference()
    {
    }

    public Reference(Standard standard)
    {
        Standard = standard;
    }

    public Reference(Publication publication)
    {
        Publication = publication;
    }

    // Standard, being an owned type, is included by default as told on https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities#querying-owned-types
    public Standard? Standard { get; private set; }

    // Publication, being an owned type, is included by default as told on https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities#querying-owned-types
    public Publication? Publication { get; private set; }

    // Note that using `!` is safe here as by construction either `Standard` or `Publication` is non-null and never both.
    [NotMapped] public IReference TheReference => Standard is not null ? Standard! : Publication!;

    // To evade the error
    // ---
    // Entity type `Reference` is an optional dependent using table sharing and
    // containing other dependents without any required non shared property to
    // identify whether the entity exists. If all nullable properties contain a
    // null value in database then an object instance won't be created in the
    // query causing nested dependent's values to be lost. Add a required
    // property to create instances with null values for other properties or
    // mark the incoming navigation as required to always create an instance.
    // ---
    // I introduce this non-null property.
    [Required] public bool Exists { get; private set; } = true;
}