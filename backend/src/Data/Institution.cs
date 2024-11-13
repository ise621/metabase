using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Metabase.Enumerations;

namespace Metabase.Data;

public sealed class Institution
    : Entity,
        IStakeholder
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Institution()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        // Parameterless constructor is needed by HotChocolate's `UseProjection`
    }

    public Institution(
        string name,
        string? abbreviation,
        string description,
        Uri? websiteLocator,
        string? publicKey,
        InstitutionState state,
        InstitutionOperatingState operatingState
    )
    {
        Name = name;
        Abbreviation = abbreviation;
        Description = description;
        WebsiteLocator = websiteLocator;
        PublicKey = publicKey;
        State = state;
        OperatingState = operatingState;
    }

    [MinLength(1)] public string? Abbreviation { get; private set; }

    [Required][MinLength(1)] public string Description { get; private set; }

    [Url] public Uri? WebsiteLocator { get; private set; }

    [MinLength(1)] public string? PublicKey { get; private set; }

    [Required] public InstitutionState State { get; private set; }

    public InstitutionOperatingState OperatingState { get; private set; }

    public ICollection<InstitutionMethodDeveloper> DevelopedMethodEdges { get; } =
        [];

    public ICollection<Method> DevelopedMethods { get; } = [];

    [InverseProperty(nameof(Method.Manager))]
    public ICollection<Method> ManagedMethods { get; } = [];

    [InverseProperty(nameof(DataFormat.Manager))]
    public ICollection<DataFormat> ManagedDataFormats { get; } = [];

    public ICollection<ComponentManufacturer> ManufacturedComponentEdges { get; } =
        [];

    public ICollection<Component> ManufacturedComponents { get; } = [];

    [InverseProperty(nameof(Database.Operator))]
    public ICollection<Database> OperatedDatabases { get; } = [];

    public Guid? ManagerId { get; set; }

    [InverseProperty(nameof(ManagedInstitutions))]
    public Institution? Manager { get; set; }

    [InverseProperty(nameof(Manager))]
    public ICollection<Institution> ManagedInstitutions { get; } = [];

    public ICollection<InstitutionRepresentative> RepresentativeEdges { get; } =
        [];

    public ICollection<User> Representatives { get; } = [];
    [Required][MinLength(1)] public string Name { get; private set; }

    public void Update(
        string name,
        string? abbreviation,
        string description,
        Uri? websiteLocator,
        string? publicKey
    )
    {
        Name = name;
        Abbreviation = abbreviation;
        Description = description;
        WebsiteLocator = websiteLocator;
        PublicKey = publicKey;
    }

    public void Verify()
    {
        State = InstitutionState.VERIFIED;
    }

    public void SwitchOperatingState(InstitutionOperatingState newState)
    {
        OperatingState = newState;
    }
}