using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Metabase.Data;

[Owned]
public sealed class Numeration(
    string? prefix,
    string mainNumber,
    string? suffix
    )
{
    [MinLength(1)] public string? Prefix { get; private set; } = prefix;

    [Required][MinLength(1)] public string MainNumber { get; private set; } = mainNumber;

    [MinLength(1)] public string? Suffix { get; private set; } = suffix;
}