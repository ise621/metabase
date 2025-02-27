using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;
using DateTime = System.DateTime;
using Metabase.Enumerations;

namespace Metabase.Data;

public sealed class Component
    : Entity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Component()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        // Parameterless constructor is needed by HotChocolate's `UseProjection`
    }

    public Component(
        string name,
        string? abbreviation,
        string description,
        NpgsqlRange<DateTime>? availability,
        ComponentCategory[] categories
    )
    {
        Name = name;
        Abbreviation = abbreviation;
        Description = description;
        Availability = availability;
        Categories = categories;
    }
    // Entity Framework Core Read-Only Properties https://docs.microsoft.com/en-us/ef/core/modeling/constructors#read-only-properties
    // Data Annotations https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations
    // Built-In Validation Attributes https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation#built-in-attributes

    [Required][MinLength(1)] public string Name { get; private set; }

    [MinLength(1)] public string? Abbreviation { get; private set; }

    [Required][MinLength(1)] public string Description { get; private set; }

    public NpgsqlRange<DateTime>?
        Availability
    {
        get;
        private set;
    } // Inifinite bounds: https://github.com/npgsql/efcore.pg/issues/570#issuecomment-437119937 and https://www.npgsql.org/doc/api/NpgsqlTypes.NpgsqlRange-1.html#NpgsqlTypes_NpgsqlRange_1__ctor__0_System_Boolean_System_Boolean__0_System_Boolean_System_Boolean_

    // https://www.npgsql.org/efcore/mapping/array.html
    [Required] public ComponentCategory[] Categories { get; private set; }

    public DescriptionOrReference? PrimeSurface { get; set; }
    public DescriptionOrReference? PrimeDirection { get; set; }
    public DescriptionOrReference? SwitchableLayers { get; set; }

    public ICollection<ComponentAssembly> PartOfEdges { get; } = [];
    public ICollection<Component> PartOf { get; } = [];

    public ICollection<ComponentAssembly> PartEdges { get; } = [];
    public ICollection<Component> Parts { get; } = [];

    public ICollection<ComponentConcretizationAndGeneralization> ConcretizationEdges { get; } =
        [];

    public ICollection<Component> Concretizations { get; } = [];

    public ICollection<ComponentConcretizationAndGeneralization> GeneralizationEdges { get; } =
        [];

    public ICollection<Component> Generalizations { get; } = [];

    public ICollection<ComponentVariant> VariantOfEdges { get; } = [];
    public ICollection<Component> VariantOf { get; } = [];

    public ICollection<ComponentVariant> VariantEdges { get; } = [];
    public ICollection<Component> Variants { get; } = [];

    public ICollection<ComponentManufacturer> ManufacturerEdges { get; } = [];
    public ICollection<Institution> Manufacturers { get; } = [];

    public void Update(
        string name,
        string? abbreviation,
        string description,
        NpgsqlRange<DateTime>? availability,
        ComponentCategory[] categories
    )
    {
        Name = name;
        Abbreviation = abbreviation;
        Description = description;
        Availability = availability;
        Categories = categories;
    }
}