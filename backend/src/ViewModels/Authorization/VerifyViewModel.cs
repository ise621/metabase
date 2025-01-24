using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OpenIddict.Abstractions;

namespace Metabase.ViewModels.Authorization;

public sealed record VerifyViewModel
{
    [Display(Name = "Application")] public string? ApplicationName { get; init; }

    [BindNever][Display(Name = "Error")] public string? Error { get; init; }

    [BindNever]
    [Display(Name = "Error description")]
    public string? ErrorDescription { get; init; }

    [Display(Name = "Scope")] public string? Scope { get; init; }

    [FromQuery(Name = OpenIddictConstants.Parameters.UserCode)]
    [Display(Name = "User code")]
    public string? UserCode { get; init; }
}