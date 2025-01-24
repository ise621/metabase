using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Metabase.Data;

public sealed class DataFormat
    : Entity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public DataFormat()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        // Parameterless constructor is needed by HotChocolate's `UseProjection`
    }

    public DataFormat(
        string name,
        string? extension,
        string description,
        string mediaType,
        Uri? schemaLocator
    )
    {
        Name = name;
        Extension = extension;
        Description = description;
        MediaType = mediaType;
        SchemaLocator = schemaLocator;
    }

    [Required][MinLength(1)] public string Name { get; private set; }

    [MinLength(1)] public string? Extension { get; private set; }

    [Required][MinLength(1)] public string Description { get; private set; }

    [Required][MinLength(1)] public string MediaType { get; private set; }

    [Url] public Uri? SchemaLocator { get; private set; }

    // Standard, being an owned type, is included by default as told on https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities#querying-owned-types
    public Standard? Standard { get; set; }

    // Publication, being an owned type, is included by default as told on https://docs.microsoft.com/en-us/ef/core/modeling/owned-entities#querying-owned-types
    public Publication? Publication { get; set; }

    // TODO Make sure that either `Standard` or `Publication` is set but never both!
    [NotMapped] public IReference? Reference => Standard is not null ? Standard : Publication;

    public Guid ManagerId { get; set; }

    [InverseProperty(nameof(Institution.ManagedDataFormats))]
    public Institution? Manager { get; set; }

    public void Update(
        string name,
        string? extension,
        string description,
        string mediaType,
        Uri? schemaLocator
    )
    {
        Name = name;
        Extension = extension;
        Description = description;
        MediaType = mediaType;
        SchemaLocator = schemaLocator;
    }
}