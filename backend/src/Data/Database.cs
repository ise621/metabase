using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using Metabase.Enumerations;

namespace Metabase.Data;

public sealed class Database
    : Entity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Database()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        // Parameterless constructor is needed by HotChocolate's `UseProjection`
    }

    public Database(
        string name,
        string description,
        Uri locator
    )
    {
        Name = name;
        Description = description;
        Locator = locator;
        VerificationState = DatabaseVerificationState.PENDING;
        VerificationCode = CreateSecureRandomString();
    }

    // private static string CreateSha512Hash(string value)
    // {
    //     using var sha512 = SHA512.Create();
    //     var bytes = Encoding.UTF8.GetBytes(value);
    //     var hashValue = sha512.ComputeHash(bytes);
    //     return Convert.ToBase64String(hashValue);
    // }

    [Required][MinLength(1)] public string Name { get; private set; }

    [Required][MinLength(1)] public string Description { get; private set; }

    [Required][Url] public Uri Locator { get; private set; }

    [Required] public DatabaseVerificationState VerificationState { get; private set; }

    [Required][MinLength(32)] public string VerificationCode { get; private set; }

    public Guid OperatorId { get; set; }

    [InverseProperty(nameof(Institution.OperatedDatabases))]
    public Institution? Operator { get; set; }

    // Inspired by https://jonathancrozier.com/blog/how-to-generate-a-cryptographically-secure-random-string-in-dot-net-with-c-sharp
    private static string CreateSecureRandomString(int count = 64)
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(count));
    }

    public void Update(
        string name,
        string description,
        Uri locator
    )
    {
        Name = name;
        Description = description;
        Locator = locator;
    }

    public void Verify()
    {
        VerificationState = DatabaseVerificationState.VERIFIED;
    }

    // Refute
    public void RevokeVerification()
    {
        VerificationState = DatabaseVerificationState.PENDING;
    }
}