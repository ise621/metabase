using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HotChocolate;
using Metabase.Enumerations;
using Microsoft.AspNetCore.Identity;
using Guid = System.Guid;

// TODO Make `User`, `Role`, ... subtype `Entity` and use `Version` to catch update conflicts. Add interface `IEntity`.
namespace Metabase.Data;

public sealed class User
    : IdentityUser<Guid>,
        IEntity,
        IStakeholder
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public User()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        // Parameterless constructor is needed by HotChocolate's `UseProjection`
    }

    public User(
        string name,
        string email,
        string? postalAddress,
        Uri? websiteLocator
    )
    {
        Email = email;
        UserName = email; // TODO Make `UserName`, `ConfirmedUserName`, ... of `IdentityUser` aliases for the respective `*Email` properties!
        Name = name;
        PostalAddress = postalAddress;
        WebsiteLocator = websiteLocator;
    }

    [GraphQLDescription("Full name")]
    [ProtectedPersonalData]
    [PersonalData]
    public string Name { get; private set; }

    [MinLength(1)]
    [ProtectedPersonalData]
    [PersonalData]
    public string? PostalAddress { get; private set; }

    [Url]
    [ProtectedPersonalData]
    [PersonalData]
    public Uri? WebsiteLocator { get; private set; }

    // public ICollection<UserClaim> Claims { get; } = new List<UserClaim>();
    // public ICollection<UserLogin> Logins { get; } = new List<UserLogin>();
    // public ICollection<UserToken> Tokens { get; } = new List<UserToken>();
    // public ICollection<UserRole> Roles { get; } = new List<UserRole>();

    public ICollection<UserMethodDeveloper> DevelopedMethodEdges { get; } = [];
    public ICollection<Method> DevelopedMethods { get; } = [];

    public ICollection<InstitutionRepresentative> RepresentedInstitutionEdges { get; } =
        [];

    public ICollection<Institution> RepresentedInstitutions { get; } = [];

    public DataSigningPermission DataSigningPermission { get; set; } = DataSigningPermission.NEVER;

    public uint Version { get; private set; } // https://www.npgsql.org/efcore/modeling/concurrency.html
}