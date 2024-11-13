using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// TODO Use it somewhere or remove it! :)
namespace Metabase.Data;

[Owned]
public sealed class ContactInformation(
    string? phoneNumber,
    string? postalAddress,
    string? emailAddress,
    Uri? websiteLocator
    )
{
    [Phone] public string? PhoneNumber { get; private set; } = phoneNumber;

    [MinLength(1)] public string? PostalAddress { get; private set; } = postalAddress;

    [EmailAddress] public string? EmailAddress { get; private set; } = emailAddress;

    [Url] public Uri? WebsiteLocator { get; private set; } = websiteLocator;
}